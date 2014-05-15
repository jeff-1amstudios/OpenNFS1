using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.Parsers;
using NfsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNFS1
{
	class Mesh
	{
		protected List<Polygon> _polys { get; set; }
		protected VertexBuffer _vertexBuffer;

		public Mesh(MeshChunk meshChunk, BitmapChunk bmpChunk)
		{
			_polys = meshChunk.Polygons;

			Resolve(bmpChunk);
		}

		public void Resolve(BitmapChunk bitmapChunk)
		{
			if (_vertexBuffer != null)
				return; //already resolved

			int vertCount = 0;

			List<VertexPositionTexture> allVerts = new List<VertexPositionTexture>();
			foreach (Polygon poly in _polys)
			{
				if (poly.TextureName != null)
				{
					poly.ResolveTexture(bitmapChunk.FindByName(poly.TextureName));
				}

				//if (poly.Type == PolygonType.WheelFrontLeft || poly.Type == PolygonType.WheelFrontRight || poly.Type == PolygonType.WheelRearLeft
				//		|| poly.Type == PolygonType.WheelRearRight)
				//		//|| poly.TextureName == "shad" || poly.TextureName == "circ"
				//		//|| poly.TextureName == "tire" || poly.TextureName == "tir2")
				//{
				//	//dont use this poly - we do the wheels ourselves
				//	poly.VertexBufferIndex = -1;
				//	continue;
				//}

				poly.VertexBufferIndex = vertCount;
				vertCount += poly.VertexCount;
				allVerts.AddRange(poly.GetVertices());
			}

			_vertexBuffer = new VertexBuffer(Engine.Instance.Device, typeof(VertexPositionTexture), vertCount, BufferUsage.WriteOnly);
			_vertexBuffer.SetData<VertexPositionTexture>(allVerts.ToArray());
		}

		public virtual void Render(Effect effect)
		{
			Engine.Instance.Device.RasterizerState = new RasterizerState { FillMode = FillMode.Solid, CullMode = CullMode.None };
			Engine.Instance.Device.SetVertexBuffer(_vertexBuffer);

			effect.CurrentTechnique.Passes[0].Apply();

			foreach (Polygon poly in _polys)
			{
				if (poly.VertexBufferIndex < 0)
					continue;

				Engine.Instance.Device.Textures[0] = poly.Texture;
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, poly.VertexBufferIndex, poly.VertexCount / 3);
			}
		}
	}
}
