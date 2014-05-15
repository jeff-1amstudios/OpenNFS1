using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using NfsEngine;
using Microsoft.Xna.Framework;
using OpenNFS1.Vehicles;

namespace NeedForSpeed.Parsers
{

	// .CFM files contain the vertices and textures for cars.
    class CfmFile
    {
		public CarMesh Mesh { get; private set; }

        public CfmFile(string filename)
        {
            Parse(filename);
        }

		private void Parse(string filename)
		{
			BitmapChunk.ResetPalette();
			string carFile = Path.Combine(GameConfig.CdDataPath, filename);
			BinaryReader br = new BinaryReader(File.Open(carFile, FileMode.Open));
			HeaderChunk rootChunk = new HeaderChunk();
			rootChunk.Read(br, true);

			// Cfm files contain a high-res model + bitmaps at index 0, and a low-res model + bitmaps at index 1.  We only use the high-res resources.
			rootChunk.MeshChunks[0].Load(br);
			rootChunk.BitmapChunks[0].Load(br);

			br.Close();

			Mesh = new CarMesh(rootChunk.MeshChunks[0], rootChunk.BitmapChunks[0]);
		}
    }
}
