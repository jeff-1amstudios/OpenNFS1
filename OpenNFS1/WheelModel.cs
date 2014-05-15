using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NfsEngine;
using NeedForSpeed.Parsers;

namespace NeedForSpeed
{
    static class WheelModel
    {
        static VertexPositionTexture[] _cylinderVertices;
        static VertexBuffer _cylinderVertexBuffer;
        static AlphaTestEffect _effect;
		static Texture2D _rubberTexture;

        static WheelModel()
        {
            CreateGeometry();

            _effect = new AlphaTestEffect(Engine.Instance.Device);
            _effect.VertexColorEnabled = false;
			_rubberTexture = new Texture2D(Engine.Instance.Device, 1, 1);
			_rubberTexture.SetData<Color>(new Color[] { new Color(0.1f, 0.1f, 0.1f) });
        }

        public static void BeginBatch()
        {
            Engine.Instance.Device.SetVertexBuffer(_cylinderVertexBuffer);

            _effect.View = Engine.Instance.Camera.View;
            _effect.Projection = Engine.Instance.Camera.Projection;
        }

		public static void Render(Matrix world, Texture2D texture)
		{
			// render rubber tire part
			_effect.World = world;
			_effect.Texture = _rubberTexture;
			_effect.CurrentTechnique.Passes[0].Apply();
			Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, 288 / 3);

			// render the sides (hubs)
			_effect.Texture = texture;
			_effect.VertexColorEnabled = false;
			_effect.CurrentTechnique.Passes[0].Apply();
			Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 288, 4);

		}

        private static void CreateGeometry()
        {
            int numSides = 32;
            Vector3 bottomCenter = new Vector3(0, -.5f, 0);
            Vector3 topCenter = new Vector3(0, .5f, 0);
            Vector3 currentVector;
            Vector3 nextVector;
            float xPos1, xPos2, zPos1, zPos2;
            float radius = 0.51f;
            _cylinderVertices = new VertexPositionTexture[numSides * 9 + 12];
            float angleChange = (float)Math.PI / (numSides / 2);
            float angle;
			
			Color c = new Color(0.1f, 0.1f, 0.1f); //rubber color

            for (int k = 0; k < numSides; k++)
            {
                angle = k * angleChange;
                xPos1 = (float)Math.Cos(angle);
                xPos2 = (float)Math.Cos(angle + angleChange);
                zPos1 = (float)Math.Sin(angle);
                zPos2 = (float)Math.Sin(angle + angleChange);
                currentVector = new Vector3(xPos1, 0, zPos1);
                nextVector = new Vector3(xPos2, 0, zPos2);

                _cylinderVertices[k * 9 + 0] = new VertexPositionTexture(topCenter + currentVector * radius, Vector2.Zero);
				_cylinderVertices[k * 9 + 1] = new VertexPositionTexture(bottomCenter + currentVector * radius, Vector2.Zero);
				_cylinderVertices[k * 9 + 2] = new VertexPositionTexture(bottomCenter + nextVector * radius, Vector2.Zero);

				_cylinderVertices[k * 9 + 3] = new VertexPositionTexture(bottomCenter + nextVector * radius, Vector2.Zero);
				_cylinderVertices[k * 9 + 5] = new VertexPositionTexture(topCenter + currentVector * radius, Vector2.Zero);
				_cylinderVertices[k * 9 + 4] = new VertexPositionTexture(topCenter + nextVector * radius, Vector2.Zero);
            }

			// Add the sides of the wheel to the cylinder

            float y = 0.505f;
            Vector3 bottomLeftFront = new Vector3(-0.5f, y, 0.5f);
            Vector3 bottomRightFront = new Vector3(0.5f, y, 0.5f);
            Vector3 bottomLeftBack = new Vector3(-0.5f, y, -0.5f);
            Vector3 bottomRightBack = new Vector3(0.5f, y, -0.5f);

			c = Color.White;
			_cylinderVertices[288] = new VertexPositionTexture(bottomLeftFront, new Vector2(0.0f, 0.0f));
			_cylinderVertices[289] = new VertexPositionTexture(bottomLeftBack, new Vector2(0.0f, 1.0f));
			_cylinderVertices[290] = new VertexPositionTexture(bottomRightBack, new Vector2(1.0f, 1.0f));
			_cylinderVertices[291] = new VertexPositionTexture(bottomLeftFront, new Vector2(0.0f, 0.0f));
			_cylinderVertices[292] = new VertexPositionTexture(bottomRightBack, new Vector2(1.0f, 1.0f));
			_cylinderVertices[293] = new VertexPositionTexture(bottomRightFront, new Vector2(1.0f, 0.0f));

			_cylinderVertices[294] = new VertexPositionTexture(new Vector3(-0.5f, -y, 0.5f), new Vector2(0.0f, 1.0f));
			_cylinderVertices[295] = new VertexPositionTexture(new Vector3(0.5f, -y, -0.5f), new Vector2(1.0f, 0.0f));
			_cylinderVertices[296] = new VertexPositionTexture(new Vector3(-0.5f, -y, -0.5f), new Vector2(0.0f, 0.0f));
			_cylinderVertices[297] = new VertexPositionTexture(new Vector3(-0.5f, -y, 0.5f), new Vector2(0.0f, 1.0f));
			_cylinderVertices[298] = new VertexPositionTexture(new Vector3(0.5f, -y, 0.5f), new Vector2(1.0f, 1.0f));
			_cylinderVertices[299] = new VertexPositionTexture(new Vector3(0.5f, -y, -0.5f), new Vector2(1.0f, 0.0f));

            _cylinderVertexBuffer = new VertexBuffer(Engine.Instance.Device,
                                                 typeof(VertexPositionTexture), _cylinderVertices.Length,
                                                 BufferUsage.WriteOnly);

            _cylinderVertexBuffer.SetData<VertexPositionTexture>(_cylinderVertices);
        }

    }


}
