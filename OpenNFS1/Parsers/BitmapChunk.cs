using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using GameEngine;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace OpenNFS1.Parsers
{
	enum BitmapEntryType
	{
		Unknown = 0,
		Texture = 0x7b,
		DosPalette = 0x22,
		Palette = 0x24,
		TextureTrailer = 0x7c
	}

	class BitmapEntry
	{
		public string Id;
		public int Offset;
		public int Length;
		public BitmapEntryType Type;
		public Texture2D Texture;
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
		const int EntryHeaderLength = 16;

		List<BitmapEntry> _bitmaps = new List<BitmapEntry>();

		internal List<BitmapEntry> Bitmaps
		{
			get { return _bitmaps; }
		}

		public int Index { get; set; }
		
		public bool EnableTextureAttachments { get; set; }  //fsh files use an extension to store detail alongside bitmaps, such as palettes etc.

		public delegate void TextureGeneratedHandler(BitmapEntry entry, byte[] palette, byte[] pixelData);
		public event TextureGeneratedHandler TextureGenerated;

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

			//Load global palette first
			foreach (BitmapEntry entry in _bitmaps)
			{
				if (entry.Id.ToUpper() == "!PAL")
				{
					reader.BaseStream.Position = Offset + entry.Offset;
					ReadEntry(reader, entry);
					_palette = _lastPalette;
					Debug.WriteLine("\tLoaded global palette");
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

			entry.Type = (BitmapEntryType)(iCode & 0x7F);

			entry.Length = iCode >> 8;

			int width = reader.ReadInt16();
			int height = reader.ReadInt16();

			if (entry.Type == BitmapEntryType.TextureTrailer)
				return;

			entry.Misc[0] = reader.ReadInt16();
			entry.Misc[1] = reader.ReadInt16();
			entry.Misc[2] = reader.ReadInt16();
			entry.Misc[3] = reader.ReadInt16();

			switch (entry.Type)
			{
				case BitmapEntryType.DosPalette:
				case BitmapEntryType.Palette:
					_lastPalette = LoadPalette(reader, entry.Type);
					entry.Type = BitmapEntryType.Palette;
					break;
				case BitmapEntryType.Texture:
					ReadTexture(reader, entry, width, height);
					break;
			}
		}

		private void ReadTexture(BinaryReader reader, BitmapEntry entry, int width, int height)
		{
			byte[] pixelData = reader.ReadBytes(width * height);

			if (EnableTextureAttachments)
			{				
				while (entry.Length > 0)
				{
					var childEntry = new BitmapEntry();
					ReadEntry(reader, childEntry);
					if (childEntry.Type == BitmapEntryType.DosPalette || childEntry.Type == BitmapEntryType.Palette)
					{
						_palette = _lastPalette;
					}
					if (childEntry.Length == 0)
						break;
				}
			}

			if (_palette == null)
			{
				_palette = _lastPalette;
			}
			TextureGenerator tg = new TextureGenerator(_palette);
			entry.Texture = tg.Generate(pixelData, width, height);
			entry.Type = BitmapEntryType.Texture;
			if (TextureGenerated != null) TextureGenerated(entry, _palette, pixelData);
		}


		byte[] LoadPalette(BinaryReader reader, BitmapEntryType type)
		{
			byte[] pal = reader.ReadBytes(3 * 256);

			switch (type)
			{
				case BitmapEntryType.DosPalette:
					// 0x22 palettes are DOS-style. r,g,b values in range 0..64
					for (int i = 0; i < 768; i++)
					{
						pal[i] = (byte)(pal[i] << 2);
					}
					return pal;


				case BitmapEntryType.Palette:
					// 0x24 palettes have r,g,b in range 0..255.  Nothing to do
					return pal;

				default:
					//throw new NotImplementedException();
					return null;
			}
		}

		public BitmapEntry FindByName(string name)
		{
			return _bitmaps.Find(a => a.Id.Equals(name, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}
