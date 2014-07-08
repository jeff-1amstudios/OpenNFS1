using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OpenNFS1
{
	class ObjectShadow
	{
		static BasicEffect _effect;
		static VertexPositionColor[] _verts = new VertexPositionColor[4];
		static ObjectShadow()
		{
			_effect = new BasicEffect(Engine.Instance.Device);
			_effect.TextureEnabled = false;
			_effect.VertexColorEnabled = true;

			Color shadowColor = new Color(10, 10, 10, 150);
			for (int i = 0; i < 4; i++)
			{
				_verts[i] = new VertexPositionColor(Vector3.Zero, shadowColor);
			}
		}

		public static void Render(Vector3[] points, Matrix world)
		{
			for (int i = 0; i < 4; i++)
			{
				_verts[i].Position = points[i];
			}

			GraphicsDevice device = Engine.Instance.Device;

			_effect.World = world;
			_effect.View = Engine.Instance.Camera.View;
			_effect.Projection = Engine.Instance.Camera.Projection;

			device.DepthStencilState = DepthStencilState.None;
			device.BlendState = BlendState.AlphaBlend;
			device.RasterizerState = RasterizerState.CullCounterClockwise;

			_effect.CurrentTechnique.Passes[0].Apply();

			device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, _verts, 0, 2);

			//device.RenderState.AlphaBlendEnable = false;
			device.BlendState = BlendState.Opaque;
			device.DepthStencilState = DepthStencilState.Default;
			device.RasterizerState = RasterizerState.CullNone;
		}

		public static void Render(Vector3[] points)
		{
			Render(points, Matrix.Identity);
		}
	}
}
