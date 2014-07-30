using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace OpenNFS1.Parsers.Audio
{
	class BnkSample
	{
		public int NbrSamples;
		public Int16 NbrChannels;
		public byte[] PCMData;
		public int SampleRate;
		public int LoopStart;
	}

	// Bnk files (in SOUNDBNK folder) contain audio samples
	class BnkFile
	{
		string _filename;
		public bool ForceMono = true;
		public List<BnkSample> Samples { get; private set; }

		public BnkFile(string filename)
		{
			_filename = filename;
			Samples = new List<BnkSample>();
			filename = Path.Combine(GameConfig.CdDataPath, @"Simdata\Soundbnk\" + filename);
			BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open));
			Parse(reader);
			reader.Close();
		}

		private void Parse(BinaryReader reader)
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
				ReadSample(reader, count++);
			}
		}

		private void ReadSample(BinaryReader reader, int sampleIndex)
		{
			BnkSample sample = new BnkSample();
			reader.BaseStream.Position += 0x28;
			string id = new string(reader.ReadChars(4));
			sample.SampleRate = reader.ReadInt32();
			byte bytesPerSample = reader.ReadByte();
			sample.NbrChannels = reader.ReadByte();
			byte compression = reader.ReadByte();
			byte type = reader.ReadByte();

			int totalLength = reader.ReadInt32();
			sample.LoopStart = reader.ReadInt32();
			int loopLength = reader.ReadInt32();
			int dataOffset = reader.ReadInt32();
			int unk3 = reader.ReadInt32();

			// HACKS to avoid popping sounds.. Not sure why we need to do this.. maybe NFS finds values near
			// loop start/end which are the best match
			//if (_filename == "SUPRA_SW.bnk" && sampleIndex == 0)
			//	loopLength += 10;

			//Debug.WriteLine(String.Format("{0}: sampleRate:{5} bps: {1} loopStart: {2} loopLen: {3} totalLen: {4}, offset: {6}, channels: {7}",
			//	headerIndex, bytesPerSample, sample.LoopStart, loopLength, sample.NbrSamples, sample.SampleRate, dataOffset, sample.NbrChannels));

			reader.BaseStream.Position = dataOffset + (bytesPerSample * sample.NbrChannels * sample.LoopStart);
			sample.NbrSamples = loopLength == 0 ? totalLength : loopLength;
			byte[] soundData = reader.ReadBytes(bytesPerSample * sample.NbrChannels * sample.NbrSamples);

			if (bytesPerSample == 1)  //convert 8 bit signed PCM data to unsigned
			{
				for (int i = 0; i < soundData.Length; i++)
					soundData[i] = (byte)(soundData[i] + 0x80);
			}

			if (ForceMono && sample.NbrChannels == 2)
			{
				MemoryStream ms = new MemoryStream();
				for (int i = 0; i < soundData.Length; i += bytesPerSample * 2)
				{
					ms.Write(soundData, i, bytesPerSample);
				}
				soundData = ms.ToArray();
				sample.NbrChannels = 1;
			}

			/*
			if (sample.NbrChannels == 2)
			{
				MemoryStream ms = new MemoryStream();
				for (int i = 0; i < soundData.Length - 4; i += 4)
				{
					ms.Write(soundData, i, 2);
				}
				soundData = ms.ToArray();
				sample.NbrChannels = 1;

				short[] sdata = new short[soundData.Length / 2];
				Buffer.BlockCopy(soundData, 0, sdata, 0, soundData.Length);
				soundData = new byte[sdata.Length * 2];
				Buffer.BlockCopy(sdata, 0, soundData, 0, loopLength * 2);
			}
			
			if (sample.NbrChannels == 3)
			{
				short lowestValue = short.MaxValue;
				int lowestValueSample = 0;
				short[] sdata = new short[soundData.Length / 2];
				Buffer.BlockCopy(soundData, 0, sdata, 0, soundData.Length);
				for (int i = loopLength; i < loopLength + 20; i++)
				{
					if (Math.Abs(sdata[i]) < Math.Abs(lowestValue))
					{
						lowestValue = sdata[i];
						lowestValueSample = i;
					}
					Debug.WriteLine(sdata[i]);
				}
				Debug.WriteLine("was " + loopLength + ", now " + lowestValueSample);
				loopLength = lowestValueSample;
				soundData = new byte[lowestValueSample * 2];
				Buffer.BlockCopy(sdata, 0, soundData, 0, lowestValueSample * 2);
			}*/

			sample.PCMData = soundData;
			Samples.Add(sample);
			
			//WavWriter.Write(String.Format("c:\\temp\\test{0}.wav", headerIndex),
			//	nbrSamples, channels, soundData);
			
		}
	}
}
