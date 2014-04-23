using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NeedForSpeed.Parsers.Audio
{
	class WavWriter
	{
		public WavWriter()
		{
		}

		public static void Write(string filename, int samples, short channels, byte[] soundData)
		{
			FileStream stream = new FileStream(filename, FileMode.Create);
			BinaryWriter writer = new BinaryWriter(stream);
			int RIFF = 0x46464952;
			int WAVE = 0x45564157;
			int DATA = 0x61746164;

			int formatChunkSize = 16;
			int headerSize = 8;
			int format = 0x20746D66;
			short formatType = 1;

			int samplesPerSecond = 16000; // 44100;
			short bitsPerSample = 16;
			short frameSize = (short)(channels * ((bitsPerSample + 7) / 8));
			int bytesPerSecond = samplesPerSecond * frameSize;
			int waveSize = 4;			
			int dataChunkSize = samples * frameSize;
			int fileSize = waveSize + headerSize + formatChunkSize + headerSize + dataChunkSize;

			writer.Write(RIFF);
			writer.Write(fileSize);
			writer.Write(WAVE);
			writer.Write(format);
			writer.Write(formatChunkSize);
			writer.Write(formatType);
			writer.Write(channels);
			writer.Write(samplesPerSecond);
			writer.Write(bytesPerSecond);
			writer.Write(frameSize);
			writer.Write(bitsPerSample);
			writer.Write(DATA);
			writer.Write(dataChunkSize);

			writer.Write(soundData);
			
			writer.Close();			
		}
	}
}