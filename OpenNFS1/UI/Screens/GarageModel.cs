using System;
using System.Collections.Generic;

using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OpenNFS1.UI.Screens
{
    class GarageModel
    {
        private VertexBuffer _vertexBuffer;

        public GarageModel(int itemCount)
        {
            List<VertexPositionNormalTexture> verts = new List<VertexPositionNormalTexture>();
            
            Vector3 topLeftFront = new Vector3(-0.5f, 0.5f, 0.5f);
            Vector3 bottomLeftFront = new Vector3(-0.5f, -0.5f, 0.5f);
            Vector3 topRightFront = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 bottomRightFront = new Vector3(0.5f, -0.5f, 0.5f);
            Vector3 topLeftBack = new Vector3(-0.5f, 0.5f, -0.5f);
            Vector3 topRightBack = new Vector3(0.5f, 0.5f, -0.5f);
            Vector3 bottomLeftBack = new Vector3(-0.5f, -0.5f, -0.5f);
            Vector3 bottomRightBack = new Vector3(0.5f, -0.5f, -0.5f);

            Vector2 textureTopLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureTopRight = new Vector2(itemCount, 0.0f);
            Vector2 textureBottomLeft = new Vector2(0.0f, 3.0f);
            Vector2 textureBottomRight = new Vector2(itemCount, 3.0f);

            Vector3 frontNormal = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 backNormal = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);

            // Back face.
            verts.Add(new VertexPositionNormalTexture(topLeftBack, backNormal, textureTopRight));
            verts.Add(new VertexPositionNormalTexture(topRightBack, backNormal, textureTopLeft));
            verts.Add(new VertexPositionNormalTexture(bottomLeftBack, backNormal, textureBottomRight));
            verts.Add(new VertexPositionNormalTexture(bottomLeftBack, backNormal, textureBottomRight));
            verts.Add(new VertexPositionNormalTexture(topRightBack, backNormal, textureTopLeft));
            verts.Add(new VertexPositionNormalTexture(bottomRightBack, backNormal, textureBottomLeft));

            // Right face. 
            verts.Add(new VertexPositionNormalTexture(topRightFront, rightNormal, textureTopLeft));
            verts.Add(new VertexPositionNormalTexture(bottomRightFront, rightNormal, textureBottomLeft));
            verts.Add(new VertexPositionNormalTexture(bottomRightBack, rightNormal, textureBottomRight));
            verts.Add(new VertexPositionNormalTexture(topRightBack, rightNormal, textureTopRight));
            verts.Add(new VertexPositionNormalTexture(topRightFront, rightNormal, textureTopLeft));
            verts.Add(new VertexPositionNormalTexture(bottomRightBack, rightNormal, textureBottomRight));

            // Top face.
            verts.Add(new VertexPositionNormalTexture(topLeftFront, topNormal, textureBottomLeft));
            verts.Add( new VertexPositionNormalTexture(topRightBack, topNormal, textureTopRight));
            verts.Add( new VertexPositionNormalTexture(topLeftBack, topNormal, textureTopLeft));
            verts.Add( new VertexPositionNormalTexture(topLeftFront, topNormal, textureBottomLeft));
            verts.Add( new VertexPositionNormalTexture(topRightFront, topNormal, textureBottomRight));
            verts.Add(new VertexPositionNormalTexture(topRightBack, topNormal, textureTopRight));


            // Bottom face. 
            verts.Add(new VertexPositionNormalTexture(bottomLeftFront, bottomNormal, textureTopLeft));
            verts.Add(new VertexPositionNormalTexture(bottomLeftBack, bottomNormal, textureBottomLeft));
            verts.Add(new VertexPositionNormalTexture(bottomRightBack, bottomNormal, textureBottomRight));
            verts.Add(new VertexPositionNormalTexture(bottomLeftFront, bottomNormal, textureTopLeft));
            verts.Add(new VertexPositionNormalTexture(bottomRightBack, bottomNormal, textureBottomRight));
            verts.Add(new VertexPositionNormalTexture(bottomRightFront, bottomNormal, textureTopRight));
                        

            _vertexBuffer = new VertexBuffer(Engine.Instance.Device, typeof(VertexPositionNormalTexture), verts.Count, BufferUsage.WriteOnly);
            _vertexBuffer.SetData<VertexPositionNormalTexture>(verts.ToArray());
        }

        public void Render(AlphaTestEffect effect)
        {
			Engine.Instance.Device.SetVertexBuffer(_vertexBuffer);
            //Engine.Instance.Device.RasterizerState.CullMode = CullMode.CullCounterClockwiseFace;

            effect.Texture = Engine.Instance.ContentManager.Load<Texture2D>("Content/wall2");

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, 4);
                Engine.Instance.Device.Textures[0] = Engine.Instance.ContentManager.Load<Texture2D>("Content/blackceramic");
                Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 12, 2);
                Engine.Instance.Device.Textures[0] = Engine.Instance.ContentManager.Load<Texture2D>("Content/floor_maya");
                Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 18, 2);
            }
        }
    }
}
