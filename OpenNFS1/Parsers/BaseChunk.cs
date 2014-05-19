using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace OpenNFS1.Parsers
{
    class BaseChunk
    {
        private long _offset;

        public long Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        protected int _length;

        public void Load(BinaryReader reader)
        {
            reader.BaseStream.Position = _offset;
            Read(reader);

        }
        public virtual void Read(BinaryReader reader)
        {
            _offset = reader.BaseStream.Position - 4;
            _length = reader.ReadInt32();
        }

        public void SkipHeader(BinaryReader reader)
        {
            reader.BaseStream.Position += 4;
        }
    }
}
