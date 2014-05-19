using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace OpenNFS1.Parsers.Audio
{
	class BnkFileParser
	{
		public BnkFileParser(string filename)
		{
			BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open));
			Read(reader);
			reader.Close();
		}

		private void Read(BinaryReader reader)
		{
			List<int> sampleOffsets = new List<int>();
			for (int i = 0; i < 128; i++)
			{
				int sampleOffset = reader.ReadInt32();
				if (sampleOffset != 0)
					sampleOffsets.Add(sampleOffset);
			}

			int count = 0;
			foreach (int sampleOffset in sampleOffsets)
			{
				reader.BaseStream.Position = sampleOffset;
				ReadSampleHeader(reader, ++count);
			}
		}

		private void ReadSampleHeader(BinaryReader reader, int headerIndex)
		{
			reader.BaseStream.Position += 0x30;
			byte bitsFlag = reader.ReadByte();
			Int16 channels = reader.ReadByte();
			reader.BaseStream.Position += 2;
			int nbrSamples = reader.ReadInt32();
			int unk1 = reader.ReadInt32();
			int unk2 = reader.ReadInt32();
			int dataOffset = reader.ReadInt32();
			int unk3 = reader.ReadInt32();

			reader.BaseStream.Position = dataOffset;
			byte[] soundData = reader.ReadBytes(bitsFlag * channels * nbrSamples);
			WavWriter.Write(String.Format("c:\\temp\\test{0}.wav", headerIndex),
				nbrSamples, channels, soundData);
			
		}
	}
}
