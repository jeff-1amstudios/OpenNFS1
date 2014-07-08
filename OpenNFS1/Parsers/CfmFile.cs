using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using NfsEngine;
using Microsoft.Xna.Framework;
using OpenNFS1.Vehicles;

namespace OpenNFS1.Parsers
{

	// .CFM files contain the vertices and textures for cars.
    class CfmFile
    {
		public CarMesh Mesh { get; private set; }
		private Color _brakeColor;


		// A CFM file can contain either a 'full' model (drivable, has wheel definitions etc), or a traffic model which is 
		// only a static model and not drivable
        public CfmFile(string filename)
        {
			Parse(filename);
        }

		private void Parse(string filename)
		{
			string carFile = Path.Combine(GameConfig.CdDataPath, filename);
			BinaryReader br = new BinaryReader(File.Open(carFile, FileMode.Open));
			HeaderChunk rootChunk = new HeaderChunk();
			rootChunk.Read(br, true);

			// Cfm files contain a high-res model + bitmaps at index 0, and a low-res model + bitmaps at index 1.  We only use the high-res resources.
			rootChunk.MeshChunks[0].Load(br);
			rootChunk.BitmapChunks[0].TextureGenerated += CfmFile_TextureGenerated;
			rootChunk.BitmapChunks[0].Load(br);

			br.Close();

			Mesh = new CarMesh(rootChunk.MeshChunks[0], rootChunk.BitmapChunks[0], _brakeColor);
		}


		// The brakes are painted with palette color #254. Remember what color that maps to now so we can generate brake on/off textures later
		void CfmFile_TextureGenerated(BitmapEntry entry, byte[] palette, byte[] pixelData)
		{
			if (entry.Id == "rsid")
			{
				_brakeColor = new Color(palette[254 * 3], palette[254 * 3 + 1], palette[254 * 3 + 2]);
			}
		}
    }
}
