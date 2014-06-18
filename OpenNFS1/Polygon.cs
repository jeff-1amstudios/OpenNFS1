using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OpenNFS1.Parsers
{
    enum PolygonShape
    {
		Triangle = 3,
        Quad = 4,
    }


    class Polygon
    {
		public PolygonShape Shape { get; set; }
		public string Label { get; set; }
		public string TextureName { get; set; }
        public Vector3[] Vertices {get ;set; }
        public Vector2[] TextureUVs {get; set;}
        public Texture2D Texture {get; set;}

        public int VertexCount
        {
            get
            {
                if (Shape == PolygonShape.Triangle)
                    return 3;
                else
                    return 6;
            }
        }

		public int VertexBufferIndex { get; set; }

		bool _computeUvs;

        public Polygon(PolygonShape type, bool computeUVs)
        {
            Shape = type;
			_computeUvs = computeUVs;
			Vertices = new Vector3[VertexCount];
			TextureUVs = new Vector2[VertexCount];

			if (_computeUvs)
			{
				TextureUVs[0] = new Vector2(0, 0);
				TextureUVs[1] = new Vector2(1, 0);
				TextureUVs[2] = new Vector2(1, 1);
				TextureUVs[3] = new Vector2(0, 0);
				TextureUVs[4] = new Vector2(1, 1);
				TextureUVs[5] = new Vector2(0, 1);
			}
        }

        public void ResolveTexture(BitmapEntry bmpEntry)
        {
            if (bmpEntry == null)
            {
                return;
            }
			
            Texture = bmpEntry.Texture;
            if (_computeUvs)  //don't need to scale our uvs based on the texture size
                return;

			// otherwise, because the uvs are in the range 0,0,tex_width,tex_height we need to scale them to the 0,1 range
            for (int i =0; i < VertexCount; i++)
            {
                Vector2 coord = TextureUVs[i];
                coord.X /= bmpEntry.Texture.Width;
                coord.Y /= bmpEntry.Texture.Height;
                TextureUVs[i] = coord;
            }
        }

		public VertexPositionTexture[] GetVertices()
		{
			VertexPositionTexture[] verts = new VertexPositionTexture[VertexCount];
			for (int i = 0; i < VertexCount; i++ )
			{
				verts[i] = new VertexPositionTexture(Vertices[i], TextureUVs[i]);
			}
			return verts;
		}
    }
}
