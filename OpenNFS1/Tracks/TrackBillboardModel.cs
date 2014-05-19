using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NfsEngine;

namespace OpenNFS1
{
	static class TrackBillboardModel
	{
		static VertexPositionTexture[] _vertices;
		static VertexBuffer _vertexBuffer;

		static TrackBillboardModel()
		{
			CreateGeometry();

			//_renderEffect = new AlphaTestEffect(Engine.Instance.Device);
			//_renderEffect.AlphaFunction = CompareFunction.Greater;
			//_renderEffect.ReferenceAlpha = 5;
		}

		public static void BeginBatch()
		{
			Engine.Instance.Device.SetVertexBuffer(_vertexBuffer);

			//_renderEffect.View = Engine.Instance.Camera.View;
			//_renderEffect.Projection = Engine.Instance.Camera.Projection;
			//_renderEffect.CurrentTechnique.Passes[0].Apply();
		}

		//public static void Render(AlphaTestEffect effect, Texture2D texture)
		//{
		//	_renderEffect.World = world;
		//	_renderEffect.Texture = texture;
		//	_renderEffect.CurrentTechnique.Passes[0].Apply();

		//	Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
		//}

		private static void CreateGeometry()
		{
			Vector3 topLeftFront = new Vector3(-0.5f, 1.0f, 0.5f);
			Vector3 bottomLeftFront = new Vector3(-0.5f, 0.0f, 0.5f);
			Vector3 topRightFront = new Vector3(0.5f, 1.0f, 0.5f);
			Vector3 bottomRightFront = new Vector3(0.5f, 0.0f, 0.5f);

			Vector2 textureTopLeft = new Vector2(0.0f, 0.0f);
			Vector2 textureTopRight = new Vector2(1.0f, 0.0f);
			Vector2 textureBottomLeft = new Vector2(0.0f, 1.0f);
			Vector2 textureBottomRight = new Vector2(1.0f, 1.0f);

			_vertices = new VertexPositionTexture[4];
			_vertices[0] = new VertexPositionTexture(topLeftFront, textureTopLeft);
			_vertices[1] = new VertexPositionTexture(bottomLeftFront, textureBottomLeft);
			_vertices[2] = new VertexPositionTexture(topRightFront, textureTopRight);
			_vertices[3] = new VertexPositionTexture(bottomRightFront, textureBottomRight);

			_vertexBuffer = new VertexBuffer(Engine.Instance.Device,
																					 typeof(VertexPositionTexture), _vertices.Length,
																					 BufferUsage.WriteOnly);

			_vertexBuffer.SetData<VertexPositionTexture>(_vertices);
		}

	}


}
