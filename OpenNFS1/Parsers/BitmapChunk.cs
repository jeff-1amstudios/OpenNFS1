using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using NfsEngine;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace OpenNFS1.Parsers
{
    enum BitmapEntryType
    {
        Unknown,
        Texture,
        Palette
    }

    class BitmapEntry
    {
        public string Id;
        public int Offset;
		public int Length;
        public int Code;
        public Texture2D Texture;
        public BitmapEntryType Type;
		public short[] Misc = new short[4];  //different purposes for different types of bitmaps

        public override string ToString()
        {
            return Id;
        }

		// these misc fields are often used by UI bitmaps to define where they should be displayed
		public Vector2 GetDisplayAt()
		{
			return new Vector2(Misc[2], Misc[3]);
		}
    }

    class BitmapChunk : BaseChunk
    {
        List<BitmapEntry> _bitmaps = new List<BitmapEntry>();

        internal List<BitmapEntry> Bitmaps
        {
            get { return _bitmaps; }
        }

        public int Index { get; set; }

        public static void ResetPalette()
        {
			_lastPalette = _palette;
            _palette = null;
        }

		public static byte[] _palette;
		private static byte[] _lastPalette;  //some files don't have their own palettes, in that case, use the last palette we loaded
        
        public override void Read(BinaryReader reader)
        {            
            base.Read(reader);

            int itemCount = reader.ReadInt32();
            string directoryName = new string(reader.ReadChars(4));

            Debug.WriteLine(">> Loading bitmap chunk " + directoryName);

            for (int i = 0; i < itemCount; i++)
            {
                BitmapEntry entry = new BitmapEntry();
                entry.Id = new string(reader.ReadChars(4));
                entry.Offset = reader.ReadInt32();
                _bitmaps.Add(entry);
            }

            //Load palette first
            foreach (BitmapEntry entry in _bitmaps)
            {
                if (entry.Id.ToUpper() == "!PAL")
                {
                    reader.BaseStream.Position = Offset + entry.Offset;
                    ReadEntry(reader, entry);
                    break;
                }
            }

            for (int i = 0; i < _bitmaps.Count; i++)
            {
                BitmapEntry entry = _bitmaps[i];
                if (entry.Type == BitmapEntryType.Unknown)
                {
                    reader.BaseStream.Position = Offset + entry.Offset;
                    ReadEntry(reader, entry);
                    Debug.WriteLine("\tLoaded bitmap " + entry.Id);
                }
            }
        }

        private void ReadEntry(BinaryReader reader, BitmapEntry entry)
        {
            int iCode = reader.ReadInt32();

            entry.Code = (int)(iCode & 0x7F);            
            entry.Length = iCode >> 8;

            int width = reader.ReadInt16();
            int height = reader.ReadInt16();

			entry.Misc[0] = reader.ReadInt16();
			entry.Misc[1] = reader.ReadInt16();
			entry.Misc[2] = reader.ReadInt16();
			entry.Misc[3] = reader.ReadInt16();

			if (entry.Code == 0x7b)
			{
				//regular bitmap
				int code = entry.Code;
				int nattach = 0;
				int auxoffs = 0;
				int nxoffs = 0;
				while (code >> 8 != 0)
				{
					nattach++;
					auxoffs += (code >> 8);
					if (auxoffs > nxoffs)
					{
					}
				}
			}

            if (entry.Id.ToUpper() == "!PAL")
            {
                _palette = LoadPalette(reader, entry.Code);
				entry.Type = BitmapEntryType.Palette;
            }
            else
            {
                ReadTexture(reader, entry, width, height);
            }
        }

		private void ReadTexture(BinaryReader reader, BitmapEntry entry, int width, int height)
		{
			byte[] pixelData = reader.ReadBytes(width * height);

			if (_palette == null)
			{
				byte[] header = reader.ReadBytes(16);
				byte[] palette = LoadPalette(reader, header[0]);
				_palette = _lastPalette;
			}

			TextureGenerator tg = new TextureGenerator(_palette);
			entry.Texture = tg.Generate(_bitmaps, entry, pixelData, width, height, Index.ToString("000_"));
			entry.Type = BitmapEntryType.Texture;
		}


        public byte[] LoadPalette(BinaryReader reader, int code)
        {
			byte[] pal = reader.ReadBytes(3 * 256);

			// 0x22 palettes are DOS-style. r,g,b values in range 0..64
			if (code == 0x22)
			{
				for (int i = 0; i < 768; i++)
				{
					pal[i] = (byte)(pal[i] << 2);
				}
			}

			// 0x24 palettes have r,g,b in range 0..255
			else if (code == 0x24)
			{
			}
			else
			{

			}

			return pal;
        }

        public BitmapEntry FindByName(string name)
        {
            return _bitmaps.Find(delegate(BitmapEntry entry)
            {
                return entry.Id == name;
            });
        }
    }
}
