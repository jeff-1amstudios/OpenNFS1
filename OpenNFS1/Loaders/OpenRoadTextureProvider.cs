using System;
using System.Collections.Generic;

using System.Text;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Parsers;

namespace OpenNFS1.Loaders
{
	class OpenRoadTextureProvider : TrackTextureProvider
	{
		public OpenRoadTextureProvider(string trackFile)
			: base(trackFile)
		{
		}


        public override Texture2D GetGroundTextureForNbr(int textureNbr)
        {
            int groupId = textureNbr / 3;
            int remainder = textureNbr % 3;

            string id = null;
            if (remainder == 0)
                id = "A000";
            else if (remainder == 1)
                id = "B000";
            else if (remainder == 2)
                id = "C000";
            else
                throw new NotImplementedException();
            
            BitmapEntry found = _root.HeaderChunks[0].BitmapChunks[groupId].Bitmaps.Find(delegate(BitmapEntry entry)
            {
                return entry.Id == id;
            });

            if (found != null)
                return found.Texture;
            else
                return null;
        }

        public override Texture2D GetSceneryTextureForId(int textureId)
        {
            textureId /= 4;
            BitmapEntry found = _root.HeaderChunks[1].BitmapChunks[textureId].Bitmaps.Find(delegate(BitmapEntry entry)
            {
                return entry.Id == "0000";
            });

            if (found == null)
                return null;
            else
            {
                return found.Texture;
            }
        }

        public override Texture2D GetFenceTexture(int textureNbr)
        {
            return GetGroundTextureForNbr(textureNbr);
        }
	}
}
