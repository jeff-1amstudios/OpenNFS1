using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace OpenNFS1.Parsers
{
    class HeaderChunk : BaseChunk
    {
        private List<MeshChunk> _meshChunks = new List<MeshChunk>();
        private List<BitmapChunk> _bitmapChunks = new List<BitmapChunk>();
        private List<HeaderChunk> _headerChunks = new List<HeaderChunk>();

        private static int _index;

        public override void Read(BinaryReader reader)
        {
            Read(reader, false, false);
        }
        public void Read(BinaryReader reader, bool readHeadersOnly)
        {
            Read(reader, true, readHeadersOnly);
        }

        public void Read(BinaryReader reader, bool readIdentifier, bool readHeadersOnly)
        {
            Debug.WriteLine(">> Reading header chunk");

            if (readIdentifier)
            {
                char[] identifier = reader.ReadChars(4);
            }
            long offset = reader.BaseStream.Position - 4;  //-4 for identifier

            int chunkCount = reader.ReadInt32();

            List<int> chunkOffsets = new List<int>();
            for (int i = 0; i < chunkCount; i++)
            {
                chunkOffsets.Add(reader.ReadInt32());
            }
            for (int i = 0; i < chunkCount; i++)
            {
                reader.BaseStream.Position = offset + chunkOffsets[i];
                ReadChunk(reader, readHeadersOnly);
            }
        }

        private void ReadChunk(BinaryReader reader, bool readHeaderOnly)
        {
            string identifier = new string(reader.ReadChars(4));

            if (identifier == "ORIP")
            {
                MeshChunk meshChunk = new MeshChunk();
                if (readHeaderOnly)
                    meshChunk.Offset = reader.BaseStream.Position;
                else
                {
                    BitmapChunk.ResetPalette();
                    meshChunk.Read(reader);
                }
                _meshChunks.Add(meshChunk);
            }
            else if (identifier == "SHPI")
            {
                BitmapChunk bitmapChunk = new BitmapChunk();
                bitmapChunk.Index = _index++;
                if (readHeaderOnly)
                    bitmapChunk.Offset = reader.BaseStream.Position;
                else
                    bitmapChunk.Read(reader);
                _bitmapChunks.Add(bitmapChunk);
            }
            else if (identifier == "wwww")
            {
                HeaderChunk header = new HeaderChunk();
                if (readHeaderOnly)
                    header.Offset = reader.BaseStream.Position;
                else
                    header.Read(reader, false, false);
                _headerChunks.Add(header);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal List<MeshChunk> MeshChunks
        {
            get { return _meshChunks; }
        }

        internal List<BitmapChunk> BitmapChunks
        {
            get { return _bitmapChunks; }
        }

        internal List<HeaderChunk> HeaderChunks
        {
            get { return _headerChunks; }
        }
    }
}
