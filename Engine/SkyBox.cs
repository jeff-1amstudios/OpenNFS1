using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GameEngine;
using System.IO;

namespace GameEngine
{
    public class SkyBox : IDrawableObject
    {

        Texture2D[] textures = new Texture2D[6];

        public Texture2D[] Textures
        {
            get { return textures; }
            set { textures = value; }
        }
		AlphaTestEffect _effect;

        VertexBuffer vertices;
        IndexBuffer indices;

		float YOffset = 15f;

        public SkyBox()
        {
            LoadResources();
        }

        
        public void LoadResources()
        {
			_effect = new AlphaTestEffect(Engine.Instance.Device);
           
            vertices = new VertexBuffer(Engine.Instance.Device,
                                typeof(VertexPositionTexture),
                                4 * 6,
                                BufferUsage.WriteOnly);

            VertexPositionTexture[] data = new VertexPositionTexture[4 * 6];

            float y = 0.3f;

            #region Define Vertexes
            Vector3 vExtents = new Vector3(1400, 450, 800);
            //back
            data[0].Position = new Vector3(vExtents.X, -vExtents.Y * y, -vExtents.Z);
            data[0].TextureCoordinate.X = 1.5f; data[0].TextureCoordinate.Y = 1.0f;
            data[1].Position = new Vector3(vExtents.X, vExtents.Y, -vExtents.Z);
            data[1].TextureCoordinate.X = 1.5f; data[1].TextureCoordinate.Y = 0.0f;
            data[2].Position = new Vector3(-vExtents.X, vExtents.Y, -vExtents.Z);
            data[2].TextureCoordinate.X = 0.5f; data[2].TextureCoordinate.Y = 0.0f;
            data[3].Position = new Vector3(-vExtents.X, -vExtents.Y * y, -vExtents.Z);
            data[3].TextureCoordinate.X = 0.5f; data[3].TextureCoordinate.Y = 1.0f;

            //front
            data[4].Position = new Vector3(-vExtents.X, -vExtents.Y * y, vExtents.Z);
            data[4].TextureCoordinate.X = 1.5f; data[4].TextureCoordinate.Y = 1.0f;
            data[5].Position = new Vector3(-vExtents.X, vExtents.Y, vExtents.Z);
            data[5].TextureCoordinate.X = 1.5f; data[5].TextureCoordinate.Y = 0.0f;
            data[6].Position = new Vector3(vExtents.X, vExtents.Y, vExtents.Z);
            data[6].TextureCoordinate.X = 0.5f; data[6].TextureCoordinate.Y = 0.0f;
            data[7].Position = new Vector3(vExtents.X, -vExtents.Y * y, vExtents.Z);
            data[7].TextureCoordinate.X = 0.5f; data[7].TextureCoordinate.Y = 1.0f;

            //bottom
            data[8].Position = new Vector3(-vExtents.X, -vExtents.Y * y, -vExtents.Z);
            data[8].TextureCoordinate.X = 1.5f; data[8].TextureCoordinate.Y = 0.0f;
            data[9].Position = new Vector3(-vExtents.X, -vExtents.Y * y, vExtents.Z);
            data[9].TextureCoordinate.X = 1.5f; data[9].TextureCoordinate.Y = 1.0f;
            data[10].Position = new Vector3(vExtents.X, -vExtents.Y * y, vExtents.Z);
            data[10].TextureCoordinate.X = 0.5f; data[10].TextureCoordinate.Y = 1.0f;
            data[11].Position = new Vector3(vExtents.X, -vExtents.Y * y, -vExtents.Z);
            data[11].TextureCoordinate.X = 0.5f; data[11].TextureCoordinate.Y = 0.0f;

            //top
            data[12].Position = new Vector3(vExtents.X, vExtents.Y, -vExtents.Z);
            data[12].TextureCoordinate.X = 0.5f; data[12].TextureCoordinate.Y = 0.0f;
            data[13].Position = new Vector3(vExtents.X, vExtents.Y, vExtents.Z);
            data[13].TextureCoordinate.X = 0.5f; data[13].TextureCoordinate.Y = 1.0f;
            data[14].Position = new Vector3(-vExtents.X, vExtents.Y, vExtents.Z);
            data[14].TextureCoordinate.X = 1.5f; data[14].TextureCoordinate.Y = 1.0f;
            data[15].Position = new Vector3(-vExtents.X, vExtents.Y, -vExtents.Z);
            data[15].TextureCoordinate.X = 1.5f; data[15].TextureCoordinate.Y = 0.0f;


            //left
            data[16].Position = new Vector3(-vExtents.X, vExtents.Y, -vExtents.Z);
            data[16].TextureCoordinate.X = 1.5f; data[16].TextureCoordinate.Y = 0.0f;
            data[17].Position = new Vector3(-vExtents.X, vExtents.Y, vExtents.Z);
            data[17].TextureCoordinate.X = 0.5f; data[17].TextureCoordinate.Y = 0.0f;
            data[18].Position = new Vector3(-vExtents.X, -vExtents.Y * y, vExtents.Z);
            data[18].TextureCoordinate.X = 0.5f; data[18].TextureCoordinate.Y = 1.0f;
            data[19].Position = new Vector3(-vExtents.X, -vExtents.Y * y, -vExtents.Z);
            data[19].TextureCoordinate.X = 1.5f; data[19].TextureCoordinate.Y = 1.0f;

            //right
            data[20].Position = new Vector3(vExtents.X, -vExtents.Y * y, -vExtents.Z);
            data[20].TextureCoordinate.X = 0.5f; data[20].TextureCoordinate.Y = 1.0f;
            data[21].Position = new Vector3(vExtents.X, -vExtents.Y * y, vExtents.Z);
            data[21].TextureCoordinate.X = 1.5f; data[21].TextureCoordinate.Y = 1.0f;
            data[22].Position = new Vector3(vExtents.X, vExtents.Y, vExtents.Z);
            data[22].TextureCoordinate.X = 1.5f; data[22].TextureCoordinate.Y = 0.0f;
            data[23].Position = new Vector3(vExtents.X, vExtents.Y, -vExtents.Z);
            data[23].TextureCoordinate.X = 0.5f; data[23].TextureCoordinate.Y = 0.0f;

            vertices.SetData<VertexPositionTexture>(data);


            indices = new IndexBuffer(Engine.Instance.Device,
                                typeof(short), 6 * 6,
                                BufferUsage.WriteOnly);

            short[] ib = new short[6 * 6];

            for (int x = 0; x < 6; x++)
            {
                ib[x * 6 + 0] = (short)(x * 4 + 0);
                ib[x * 6 + 2] = (short)(x * 4 + 1);
                ib[x * 6 + 1] = (short)(x * 4 + 2);

                ib[x * 6 + 3] = (short)(x * 4 + 2);
                ib[x * 6 + 5] = (short)(x * 4 + 3);
                ib[x * 6 + 4] = (short)(x * 4 + 0);
            }

            indices.SetData<short>(ib);
            #endregion

        }

        public void Update(GameTime gameTime)
        {
			var pos = Engine.Instance.Camera.Position;
			pos.Y += YOffset;
			_effect.World = Matrix.CreateTranslation(pos);
            _effect.Projection = Engine.Instance.Camera.Projection;
            _effect.View = Engine.Instance.Camera.View;
        }


        public void Draw()
        {
            if (vertices == null)
                return;

			GraphicsDevice device = Engine.Instance.Device;

			var ds = DepthStencilState.None;
			
			device.DepthStencilState = ds;
			device.SamplerStates[0] = SamplerState.LinearWrap;
			device.RasterizerState = RasterizerState.CullCounterClockwise;
			device.SetVertexBuffer(vertices);
			device.Indices = indices;
			_effect.CurrentTechnique.Passes[0].Apply();

			for (int x = 0; x < 6; x++)
			{
				if (textures[x] == null) continue;

				_effect.Texture = textures[x];
				_effect.Techniques[0].Passes[0].Apply();
				device.DrawIndexedPrimitives(PrimitiveType.TriangleList,
					0, 0, vertices.VertexCount, x * 6, 2);
			}
			
			device.DepthStencilState = DepthStencilState.Default;
        }
    }
}
