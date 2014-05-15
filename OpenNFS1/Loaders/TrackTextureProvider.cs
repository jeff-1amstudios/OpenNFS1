using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using NeedForSpeed.Parsers;
using OpenNFS1;

namespace NeedForSpeed.Loaders
{
    class TrackTextureProvider
    {
        protected HeaderChunk _root;

        public TrackTextureProvider(string trackFile)
        {
            string textureFilePath = "SIMDATA\\ETRACKFM\\" + Path.GetFileNameWithoutExtension(trackFile) + "_001.fam";
            ReadFamFile(textureFilePath);
        }

        private void ReadFamFile(string filename)
        {
			filename = Path.Combine(GameConfig.CdDataPath, filename);
            BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open));
            _root = new HeaderChunk();
            _root.Read(reader, false);  //we want to load everything up front
            reader.Close();
        }

        public virtual Texture2D GetGroundTextureForId(string textureId)
        {
            BitmapEntry found = _root.HeaderChunks[0].BitmapChunks[0].Bitmaps.Find(delegate(BitmapEntry entry)
            {
                return entry.Id.ToLower() == textureId;
            });

            if (found == null)
                return null;
            else
                return found.Texture;
        }

        public virtual Texture2D GetGroundTextureForNbr(int textureNbr)
        {
			if (textureNbr == 30)
			{

			}
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
			            
            BitmapEntry found = _root.HeaderChunks[0].BitmapChunks[0].Bitmaps.Find(delegate(BitmapEntry entry)
            {
				//return entry.Id == id;

                //0 is highest quality texture. Only use these.
                if (!entry.Id.EndsWith("0"))
                    return false;
				if (entry.Id.ToUpper() == "GA00") return false;

                int groupId = int.Parse(entry.Id.Substring(0, 2)) * 3;
                if (entry.Id[2] == 'B')
                    groupId++;
                else if (entry.Id[2] == 'C')
                    groupId+=2;

                return groupId == textureNbr;
            });

            if (found == null)
            {
                Debug.WriteLine("Ground texture not found: " + textureNbr);
                return null;
            }
            else
                return found.Texture;
        }

        public virtual Texture2D GetSceneryTextureForId(int textureId)
        {
            textureId /= 4;
            string texture = textureId.ToString("00") + "00";
            BitmapEntry found = _root.HeaderChunks[1].BitmapChunks[0].Bitmaps.Find(delegate(BitmapEntry entry)
            {
                //0 is highest quality texture. Only use these for now.
                if (!entry.Id.EndsWith("0"))
                    return false;

                return entry.Id == texture;
            });

            if (found == null)
                return null;
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
            return GetGroundTextureForId("ga00");
        }

        public Mesh GetMesh(int index)
        {
            var meshChunk = _root.HeaderChunks[2].HeaderChunks[index].MeshChunks[0];
			var bmpChunk = _root.HeaderChunks[2].HeaderChunks[index].BitmapChunks[0];
			return new Mesh(meshChunk, bmpChunk);
        }
    }
}
