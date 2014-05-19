using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenNFS1;

namespace OpenNFS1.Parsers
{
	// A QFS file is a compressed FSH file.  A FSH file holds bitmaps
	class QfsFile
	{
		public FshFile Fsh { get; private set; }

		public QfsFile(string filename)
		{
			filename = Path.Combine(GameConfig.CdDataPath, filename);
			byte[] decompressedData = Decompress(filename);
			File.WriteAllBytes("c:\\temp\\output.fsh", decompressedData);

			Fsh = new FshFile(decompressedData);
		}
		
		
		// Apparently this LZ77 compression variant was figured out by an 'Ian Brown' (can't find any contact info for him).
		// This method was ported from Dennis Auroux's fshtool.c source. (http://www-math.mit.edu/~auroux/software/index.html)
		private byte[] Decompress(string filename)
		{
			byte[] inbuf = File.ReadAllBytes(filename);
			int buflen = inbuf.Length;
			byte[] outbuf;
						
			byte packcode;
			int a,b,c,len,offset;
			int inlen,outlen,inpos,outpos;
  
			/* length of data */
			inlen=buflen;
			outlen=(inbuf[2]<<16)+(inbuf[3]<<8)+inbuf[4];
			outbuf= new byte[outlen];
  
			/* position in file */
			if ((inbuf[0] & 0x01) != 0) inpos=8; else inpos=5;
			outpos=0;
  
			/* main decoding loop */
			while ((inpos<inlen)&&(inbuf[inpos]<0xFC))
			{
			packcode=inbuf[inpos];
			a=inbuf[inpos+1];
			b=inbuf[inpos+2];
    
			if ((packcode&0x80) == 0) {
				len=packcode&3;
				mmemcpy(outbuf,outpos,inbuf,inpos+2,len);
				inpos+=len+2;
				outpos+=len;
				len=((packcode&0x1c)>>2)+3;
				offset=((packcode>>5)<<8)+a+1;
				mmemcpy(outbuf,outpos,outbuf,outpos-offset,len);
				outpos+=len;
			}
			else if ((packcode&0x40) == 0) {
				len=(a>>6)&3; 
				mmemcpy(outbuf,outpos,inbuf,inpos+3,len);
				inpos+=len+3;
				outpos+=len;
				len=(packcode&0x3f)+4;
				offset=(a&0x3f)*256+b+1;
				mmemcpy(outbuf,outpos,outbuf,outpos-offset,len);
				outpos+=len;
			}  
			else if ((packcode&0x20) == 0) {
				c=inbuf[inpos+3];
				len=packcode&3; 
				mmemcpy(outbuf,outpos,inbuf,inpos+4,len);
				inpos+=len+4;
				outpos+=len;
				len=((packcode>>2)&3)*256+c+5;
				offset=((packcode&0x10)<<12)+256*a+b+1;
				mmemcpy(outbuf,outpos,outbuf,outpos-offset,len);
				outpos+=len;
			}  
			else {
				len=(packcode&0x1f)*4+4;
				mmemcpy(outbuf,outpos,inbuf,inpos+1,len);
				inpos+=len+1;
				outpos+=len;
			}
			}
  
			/* trailing bytes */
			if ((inpos<inlen)&&(outpos<outlen)) {
			mmemcpy(outbuf,outpos,inbuf,inpos+1,inbuf[inpos]&3);
			outpos+=inbuf[inpos]&3;
			}

			if (outpos != outlen)
			{
				//printf("Warning: bad length ? %d instead of %d\n", outpos, outlen);
			}
			
			return outbuf;
		}

		void mmemcpy(byte[] dest, int pDest, byte[] src, int pSrc, int len) /* LZ-compatible memcopy */
		{
			//while (len--) *(dest++) = *(src++);
			while (len-- != 0)
			{
				dest[pDest++] = src[pSrc++];
			}
		}
	}
}
