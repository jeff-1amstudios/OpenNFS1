using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NeedForSpeed
{
	class ObjectShadow
	{
		static BasicEffect _effect;
		static ObjectShadow()
		{
			_effect = new BasicEffect(Engine.Instance.Device);
		}

		public static void Render(Vector3[] points, Matrix world)
		{
			Color shadowColor = new Color(10, 10, 10, 100);
			VertexPositionColor[] verts = new VertexPositionColor[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				verts[i] = new VertexPositionColor(points[i], shadowColor);
			}

			GraphicsDevice device = Engine.Instance.Device;

			_effect.World = world;
			_effect.TextureEnabled = false;
			_effect.VertexColorEnabled = true;

			device.DepthStencilState = DepthStencilState.None;

			_effect.CurrentTechnique.Passes[0].Apply();

			device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, verts, 0, 2);

			//device.RenderState.AlphaBlendEnable = false;
			device.DepthStencilState = DepthStencilState.Default;
		}

		public static void Render(Vector3[] points)
		{
			Render(points, Matrix.Identity);
		}
	}
}
