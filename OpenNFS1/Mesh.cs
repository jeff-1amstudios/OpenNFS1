using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Parsers;
using NfsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OpenNFS1
{
	public static class MeshCache
	{

	}

	class Mesh
	{
		protected List<Polygon> _polys { get; set; }
		protected VertexBuffer _vertexBuffer;
		public string Identifier { get; private set; }

		public BoundingBox BoundingBox {get; private set;}

		public Mesh(MeshChunk meshChunk, BitmapChunk bmpChunk)
		{
			_polys = meshChunk.Polygons;
			Identifier = meshChunk.Identifier;
			Resolve(bmpChunk);
			BoundingBox = GetBoundingBox();			
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

				poly.VertexBufferIndex = vertCount;
				vertCount += poly.VertexCount;
				allVerts.AddRange(poly.GetVertices());
			}

			_vertexBuffer = new VertexBuffer(Engine.Instance.Device, typeof(VertexPositionTexture), vertCount, BufferUsage.WriteOnly);
			_vertexBuffer.SetData<VertexPositionTexture>(allVerts.ToArray());
		}

		public virtual void Render(Effect effect)
		{
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

		private BoundingBox GetBoundingBox()
		{
			BoundingBox bb = new BoundingBox();
			bb.Min = new Microsoft.Xna.Framework.Vector3(float.MaxValue);
			bb.Max = new Microsoft.Xna.Framework.Vector3(-float.MaxValue);
			foreach (var poly in _polys)
			{
				foreach (var vert in poly.Vertices)
				{
					if (vert.X < bb.Min.X) bb.Min.X = vert.X;
					if (vert.Y < bb.Min.Y) bb.Min.Y = vert.Y;
					if (vert.Z < bb.Min.Z) bb.Min.Z = vert.Z;

					if (vert.X > bb.Max.X) bb.Max.X = vert.X;
					if (vert.Y > bb.Max.Y) bb.Max.Y = vert.Y;
					if (vert.Z > bb.Max.Z) bb.Max.Z = vert.Z;
				}
			}
			return bb;
		}

		public void Dispose()
		{
			_vertexBuffer.Dispose();
		}
	}
}
