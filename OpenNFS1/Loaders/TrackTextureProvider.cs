using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using NeedForSpeed.Parsers;

namespace NeedForSpeed.Loaders
{
    class TrackTextureProvider
    {
        protected HeaderChunk _root;

        public TrackTextureProvider(string trackFile)
        {
            string textureFilePath = Path.Combine(Path.GetDirectoryName(trackFile), "..\\TrackTextures\\" + Path.GetFileNameWithoutExtension(trackFile) + "_001.fam");
            ReadFamFile(textureFilePath);
        }

        private void ReadFamFile(string filename)
        {
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
            
            BitmapEntry found = _root.HeaderChunks[0].BitmapChunks[0].Bitmaps.Find(delegate(BitmapEntry entry)
            {
                //0 is highest quality texture. Only use these.
                if (!entry.Id.EndsWith("0"))
                    return false;

                if (entry.Id.ToUpper() == "GA00")
                    return false;

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

        public MeshChunk GetMesh(int index)
        {
            _root.HeaderChunks[2].HeaderChunks[index].MeshChunks[0].Resolve(_root.HeaderChunks[2].HeaderChunks[index].BitmapChunks[0]);
            return _root.HeaderChunks[2].HeaderChunks[index].MeshChunks[0];
        }
    }
}
