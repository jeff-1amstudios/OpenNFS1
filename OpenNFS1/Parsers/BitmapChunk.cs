using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using NfsEngine;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace NeedForSpeed.Parsers
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
        public int Code;
        public Texture2D Texture;
        public BitmapEntryType Type;
        public int Unknown;
        public Vector2 DisplayAt;

        public override string ToString()
        {
            return Id;
        }
    }

    class BitmapChunk : BaseChunk
    {
        static byte[] _palette;
        
                
        List<BitmapEntry> _bitmaps = new List<BitmapEntry>();

        internal List<BitmapEntry> Bitmaps
        {
            get { return _bitmaps; }
        }

        public int Index { get; set; }

        public static void ResetPalette()
        {
            _palette = null;
        }
        
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
            int code = reader.ReadInt32();

            entry.Code = code & 0xFF;
            
            int length = code >> 8;

            int width = reader.ReadInt16();
            int height = reader.ReadInt16();
            entry.Unknown = reader.ReadInt32();
            entry.DisplayAt = new Vector2(reader.ReadInt16(), reader.ReadInt16());

            if (entry.Id.ToUpper() == "!PAL")
            {
                _palette = LoadPalette(reader, entry.Code);
            }
            else
            {
                ReadTexture(reader, entry, width, height, code);
            }
        }

        private void ReadTexture(BinaryReader reader, BitmapEntry entry, int width, int height, int length)
        {
            byte[] pixelData = reader.ReadBytes(width * height);

            if (_palette == null)
            {
                byte[] header = reader.ReadBytes(16);
                byte[] palette = LoadPalette(reader, header[0]);
                TextureGenerator tg = new TextureGenerator(palette);
                entry.Texture = tg.Generate(_bitmaps, entry, pixelData, width, height, Index.ToString("000_"));
                entry.Type = BitmapEntryType.Texture;
            }
            else
            {
                TextureGenerator tg = new TextureGenerator(_palette);
                entry.Texture = tg.Generate(_bitmaps, entry, pixelData, width, height, Index.ToString("000_"));
                entry.Type = BitmapEntryType.Texture;
            }
        }


        public byte[] LoadPalette(BinaryReader reader, int code)
        {
            byte[] paletteBuffer = reader.ReadBytes(3 * 256);
            
            if (code == 0x22)
            {
                for (int i = 0; i < 768; i++)
                {
                    paletteBuffer[i] = (byte)Math.Min(255, Math.Round((float)paletteBuffer[i] * 4));
                }
            }

            return paletteBuffer;
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
