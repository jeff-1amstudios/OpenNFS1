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
			Color shadowColor = new Color(10, 10, 10, 150);
			VertexPositionColor[] verts = new VertexPositionColor[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				verts[i] = new VertexPositionColor(points[i], shadowColor);
			}

			GraphicsDevice device = Engine.Instance.Device;

			_effect.World = world;
			_effect.View = Engine.Instance.Camera.View;
			_effect.Projection = Engine.Instance.Camera.Projection;
			_effect.TextureEnabled = false;
			_effect.VertexColorEnabled = true;

			device.DepthStencilState = DepthStencilState.None;
			device.BlendState = BlendState.AlphaBlend;

			_effect.CurrentTechnique.Passes[0].Apply();

			device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, verts, 0, 2);

			//device.RenderState.AlphaBlendEnable = false;
			device.BlendState = BlendState.Opaque;
			device.DepthStencilState = DepthStencilState.Default;
		}

		public static void Render(Vector3[] points)
		{
			Render(points, Matrix.Identity);
		}
	}
}
