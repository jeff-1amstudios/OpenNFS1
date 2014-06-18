using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using OpenNFS1;

namespace OpenNFS1.Parsers
{

	// .FAM files in the TRACKFAM folder contain textures and models for the track terrain and scenery objects
    class TrackfamFile
    {
        protected HeaderChunk _root;
		private Mesh[] _meshCache;

		protected const int TERRAIN_CHUNK = 0;
		protected const int SCENERY_CHUNK = 1;
		protected const int MESH_CHUNK = 2;

        public TrackfamFile(string trackFile)
        {
            string textureFilePath = "SIMDATA\\NTRACKFM\\" + Path.GetFileNameWithoutExtension(trackFile) + "_T01.fam";
            ReadFamFile(textureFilePath);

			_meshCache = new Mesh[_root.HeaderChunks[2].HeaderChunks.Count];
        }

        private void ReadFamFile(string filename)
        {
			filename = Path.Combine(GameConfig.CdDataPath, filename);
            BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open));
            _root = new HeaderChunk();
            _root.Read(reader, false);  //we want to load everything up front
            reader.Close();
        }


        public virtual Texture2D GetGroundTexture(int textureNbr)
        {
			int groupId2 = textureNbr / 3;
			int remainder = textureNbr % 3;

			string id = null;
			if (remainder == 0)
				id = groupId2.ToString("00") + "A0";
			else if (remainder == 1)
				id = groupId2.ToString("00") + "B0";
			else if (remainder == 2)
				id = groupId2.ToString("00") + "C0";
			else
				throw new NotImplementedException();

			BitmapEntry found = _root.HeaderChunks[TERRAIN_CHUNK].BitmapChunks[0].Bitmaps.Find(a => a.Id == id);

            if (found == null)
            {
                Debug.WriteLine("Warning: Ground texture not found: " + textureNbr);
                return null;
            }
            else
                return found.Texture;
        }

        public virtual Texture2D GetSceneryTexture(int textureNbr)
        {
            textureNbr /= 4;
            string texture = textureNbr.ToString("00") + "00";
			BitmapEntry found = _root.HeaderChunks[SCENERY_CHUNK].BitmapChunks[0].Bitmaps.Find(a => a.Id == texture);

			if (found == null)
			{
				Debug.WriteLine("Warning: Scenery texture not found: " + textureNbr);
				return null;
			}
			else
			{
				return found.Texture;
			}
        }

        public Texture2D HorizonTexture
        {
            get
            {
                return _root.BitmapChunks[0].FindByName("horz").Texture;
            }
        }

        public virtual Texture2D GetFenceTexture(int textureNbr)
        {
			return _root.HeaderChunks[TERRAIN_CHUNK].BitmapChunks[0].FindByName("ga00").Texture;
        }

		// Mesh data (vertices + textures) are stored in the 3rd chunk of a FAM file
		public Mesh GetMesh(int index)
		{
			if (_meshCache[index] == null)
			{
				var meshChunk = _root.HeaderChunks[MESH_CHUNK].HeaderChunks[index].MeshChunks[0];
				var bmpChunk = _root.HeaderChunks[MESH_CHUNK].HeaderChunks[index].BitmapChunks[0];
				_meshCache[index] = new Mesh(meshChunk, bmpChunk);
			}
			return _meshCache[index];
		}

		public void Dispose()
		{
			foreach (var bm in _root.HeaderChunks[TERRAIN_CHUNK].BitmapChunks[0].Bitmaps)
			{
				if (bm.Texture != null) bm.Texture.Dispose();
			}
			foreach (var bm in _root.HeaderChunks[SCENERY_CHUNK].BitmapChunks[0].Bitmaps)
			{
				if (bm.Texture != null) bm.Texture.Dispose();
			}

			foreach (var bm in _root.HeaderChunks[MESH_CHUNK].BitmapChunks)
			{
				foreach (var bm2 in bm.Bitmaps)
					if (bm2.Texture != null) bm2.Texture.Dispose();
			}

			foreach (var mesh in _meshCache)
			{
				if (mesh != null) mesh.Dispose();
			}
		}
    }
}
