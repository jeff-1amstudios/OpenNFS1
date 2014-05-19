using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Parsers.Track;

namespace OpenNFS1.Tracks
{
	// A terrainSegment holds a group of 4 TerrainRow's and defines properties common to those rows (fence, textures etc)
	class TerrainSegment
	{
		public Texture2D[] Textures = new Texture2D[10];
		public bool HasLeftFence, HasRightFence;
		public int FenceTextureId;
		public Texture2D FenceTexture;
		public byte[] TextureIds;
		public TerrainRow[] Rows = new TerrainRow[4];
		public int FenceBufferIndex;
		public int TerrainBufferIndex;
		public TerrainSegment Next, Prev;
	}
}
