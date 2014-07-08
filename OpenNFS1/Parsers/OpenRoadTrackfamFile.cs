using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace OpenNFS1.Parsers
{
	class OpenRoadTrackfamFile : TrackfamFile
	{
		public OpenRoadTrackfamFile(string trackFile, bool alternateTimeOfDay)
			: base(trackFile, alternateTimeOfDay)
		{
		}

        public override Texture2D GetGroundTexture(int textureNbr)
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

			BitmapEntry found = _root.HeaderChunks[TERRAIN_CHUNK].BitmapChunks[groupId].Bitmaps.Find(a => a.Id == id);

			if (found != null)
			{
				Debug.WriteLine("Warning: Ground texture not found: " + textureNbr);
				return found.Texture;
			}
			else
				return null;
        }

        public override Texture2D GetSceneryTexture(int textureNbr)
        {
            textureNbr /= 4;
			BitmapEntry found = _root.HeaderChunks[SCENERY_CHUNK].BitmapChunks[textureNbr].Bitmaps.Find(a => a.Id == "0000");

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

        public override Texture2D GetFenceTexture(int textureNbr)
        {
			var tex = GetGroundTexture(textureNbr);
			if (tex == null)
			{  
				//nasty hack: handle Alpine 1's initial fence textureIds which don't seem to match any other tracks...
				tex = GetGroundTexture(textureNbr * 3);
			}
			return tex;
        }
	}
}
