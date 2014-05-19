using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenNFS1;
using OpenNFS1.Parsers;

namespace OpenNFS1.Parsers
{

	// A FSH file holds 1 or many bitmap entries
	class FshFile
	{
		public BitmapChunk Header { get; private set; }

		public FshFile(string filename)
		{
			filename = Path.Combine(GameConfig.CdDataPath, filename);
			Parse(File.Open(filename, FileMode.Open));
		}

		public FshFile(byte[] contents)
		{
			Parse(new MemoryStream(contents));
		}

		private void Parse(Stream contents)
		{
			BinaryReader br = new BinaryReader(contents);
			BitmapChunk.ResetPalette();
			Header = new BitmapChunk();
			Header.SkipHeader(br);
			Header.Read(br);
			br.Close();
		}
	}
}
