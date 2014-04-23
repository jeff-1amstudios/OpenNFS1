using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NfsEngine;
using System.Diagnostics;

namespace NeedForSpeed.Parsers
{
	class MeshChunk : BaseChunk
	{
		List<Vector3> _vertices = new List<Vector3>();
		List<Vector2> _vertexTextureMap = new List<Vector2>();

		List<Polygon> _polygons = new List<Polygon>();
		private VertexBuffer _vertexBuffer;

		private List<string> _textureNames = new List<string>();

		public override void Read(BinaryReader reader)
		{
			base.Read(reader);

			Debug.WriteLine(">> Loading mesh chunk");

			int unk = reader.ReadInt32();
			unk = reader.ReadInt32();
			int vertexCount = reader.ReadInt32();
			unk = reader.ReadInt32();
			int vertexBlockOffset = reader.ReadInt32();
			int texturePointsCount = reader.ReadInt32();
			int texturePointsBlockOffset = reader.ReadInt32();
			int polygonCount = reader.ReadInt32();
			int polygonBlockOffset = reader.ReadInt32();

			string identifer = new string(reader.ReadChars(12));
			Debug.WriteLine("mesh id: " + identifer);
			int textureNameCount = reader.ReadInt32();
			int textureNameBlockOffset = reader.ReadInt32();

			reader.ReadBytes(16);  //section 4,5 

			int polygonVertexMapBlockOffset = reader.ReadInt32();

			reader.ReadBytes(8); //section 6

			int wheelCount = reader.ReadInt32();
			int wheelPolygonBlockOffset = reader.ReadInt32();

			reader.BaseStream.Position = Offset + polygonVertexMapBlockOffset;
			MemoryStream ms = new MemoryStream(reader.ReadBytes(_length - polygonVertexMapBlockOffset));
			BinaryReader polygonVertexMapReader = new BinaryReader(ms);

			reader.BaseStream.Position = Offset + vertexBlockOffset;
			ReadVertexBlock(reader, vertexCount);

			reader.BaseStream.Position = Offset + texturePointsBlockOffset;
			ReadTextureMapBlock(reader, texturePointsCount);

			reader.BaseStream.Position = Offset + textureNameBlockOffset;
			ReadTextureNameBlock(reader, textureNameCount);

			reader.BaseStream.Position = Offset + polygonBlockOffset;
			ReadPolygonBlock(reader, polygonCount, polygonVertexMapReader);
			polygonVertexMapReader.Close();

			reader.BaseStream.Position = Offset + wheelPolygonBlockOffset;
			ReadWheelPolygonBlock(reader, wheelCount);
		}

		private void ReadVertexBlock(BinaryReader reader, int vertexCount)
		{
			for (int i = 0; i < vertexCount; i++)
			{
				Vector3 vertex = new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32());
				_vertices.Add(vertex);
			}
		}

		private void ReadTextureMapBlock(BinaryReader reader, int texturePointsCount)
		{
			for (int i = 0; i < texturePointsCount; i++)
			{
				int tU = reader.ReadInt32();
				int tV = reader.ReadInt32();
				_vertexTextureMap.Add(new Vector2(tU, tV));
			}
		}

		private void ReadTextureNameBlock(BinaryReader reader, int textureNameCount)
		{
			for (int i = 0; i < textureNameCount; i++)
			{
				reader.ReadBytes(8);
				string id2 = new string(reader.ReadChars(4));
				if (id2 == "\0\0\0\0")
					id2 = null;
				_textureNames.Add(id2);
				reader.ReadBytes(8);
			}
		}

		private void ReadPolygonBlock(BinaryReader reader, int polygonCount, BinaryReader polygonVertexMap)
		{
			Polygon lastPoly = null;

			for (int i = 0; i < polygonCount; i++)
			{
				PolygonShape shape = (PolygonShape)reader.ReadByte();
				if (shape == PolygonShape.Triangle2)
					shape = PolygonShape.Triangle;
				if (shape == PolygonShape.Quad2)
					shape = PolygonShape.Quad;

				Polygon polygon = new Polygon(shape);

				byte b1 = reader.ReadByte();
				byte textureNumber = reader.ReadByte();

				polygon.TextureName = _textureNames[textureNumber];

				byte b2 = reader.ReadByte();
				int poly1Index = reader.ReadInt32();
				int poly2Index = reader.ReadInt32();

				if (!Enum.IsDefined(typeof(PolygonShape), shape))
				{
					polygon.Shape = PolygonShape.Triangle;
					continue;
				}

				if (poly1Index == poly2Index && polygon.Shape == PolygonShape.Quad)
				{
					Debug.WriteLine("Converting Quad to Untextured");
					polygon.Shape = PolygonShape.UnTexturedQuad;
				}
				else if (poly1Index != poly2Index && polygon.Shape == PolygonShape.UnTexturedQuad)
				{
					Debug.WriteLine("Converting UnTextured to Quad");
					polygon.Shape = PolygonShape.Quad;
				}

				//Vertices for polygon
				polygonVertexMap.BaseStream.Position = poly1Index * sizeof(int);
				int v1 = polygonVertexMap.ReadInt32();
				int v2 = polygonVertexMap.ReadInt32();
				int v3 = polygonVertexMap.ReadInt32();
				int v4 = polygonVertexMap.ReadInt32();

				//Texture co-ords for vertices
				polygonVertexMap.BaseStream.Position = poly2Index * sizeof(int);
				int t1 = polygonVertexMap.ReadInt32();
				int t2 = polygonVertexMap.ReadInt32();
				int t3 = polygonVertexMap.ReadInt32();
				int t4 = polygonVertexMap.ReadInt32();

				Debug.WriteLine(String.Format("Poly {8} {0} ({7}): {1},{2},{3},{4}, / {5} {6}", i, v1, v2, v3, v4, b1, b2, polygon.TextureName, polygon.Shape));

				if (polygon.Shape == PolygonShape.Triangle || polygon.Shape == PolygonShape.Quad)
				{
					polygon.Vertices.Add(_vertices[v1]);
					polygon.Vertices.Add(_vertices[v2]);
					polygon.Vertices.Add(_vertices[v3]);
					polygon.TextureCoords.Add(_vertexTextureMap[t1]);
					polygon.TextureCoords.Add(_vertexTextureMap[t2]);
					polygon.TextureCoords.Add(_vertexTextureMap[t3]);
				}
				if (polygon.Shape == PolygonShape.Quad)
				{
					polygon.Vertices.Add(_vertices[v1]);
					polygon.Vertices.Add(_vertices[v3]);
					polygon.Vertices.Add(_vertices[v4]);
					polygon.TextureCoords.Add(_vertexTextureMap[t1]);
					polygon.TextureCoords.Add(_vertexTextureMap[t3]);
					polygon.TextureCoords.Add(_vertexTextureMap[t4]);
				}
				else if (polygon.Shape == PolygonShape.UnTexturedQuad)
				{
					polygon.Vertices.Add(_vertices[v1]);
					polygon.Vertices.Add(_vertices[v2]);
					polygon.Vertices.Add(_vertices[v3]);
					polygon.Vertices.Add(_vertices[v1]);
					polygon.Vertices.Add(_vertices[v3]);
					polygon.Vertices.Add(_vertices[v4]);

					polygon.TextureCoords.Add(new Vector2(0, 0));
					polygon.TextureCoords.Add(new Vector2(1, 0));
					polygon.TextureCoords.Add(new Vector2(1, 1));
					polygon.TextureCoords.Add(new Vector2(0, 0));
					polygon.TextureCoords.Add(new Vector2(1, 1));
					polygon.TextureCoords.Add(new Vector2(0, 1));
				}
				else
				{
					//throw new NotImplementedException();
				}

				lastPoly = polygon;

				_polygons.Add(polygon);
			}
		}

		private void ReadWheelPolygonBlock(BinaryReader reader, int wheelCount)
		{
			for (int i = 0; i < wheelCount; i++)
			{
				string wheelName = new string(reader.ReadChars(8));
				wheelName = wheelName.Replace("\0", "");
				int polyIndex = reader.ReadInt32();

				switch (wheelName)
				{
					case "rt_rear":
						_polygons[polyIndex].Type = PolygonType.WheelRearRight;
						break;
					case "lt_rear":
						_polygons[polyIndex].Type = PolygonType.WheelRearLeft;
						break;
					case "rt_frnt":
						_polygons[polyIndex].Type = PolygonType.WheelFrontRight;
						break;
					case "lt_frnt":
						_polygons[polyIndex].Type = PolygonType.WheelFrontLeft;
						break;
				}
			}
		}

		public void Resolve(BitmapChunk bitmapChunk)
		{
			if (_vertexBuffer != null)
				return; //already resolved

			int vertCount = 0;

			List<VertexPositionTexture> allVerts = new List<VertexPositionTexture>();
			foreach (Polygon poly in _polygons)
			{
				if (poly.TextureName != null)
				{
					poly.ResolveTexture(bitmapChunk.FindByName(poly.TextureName));
				}
				/*
				if (poly.Type == PolygonType.WheelFrontLeft || poly.Type == PolygonType.WheelFrontRight || poly.Type == PolygonType.WheelRearLeft
						|| poly.Type == PolygonType.WheelRearRight
						|| poly.TextureName == "shad" || poly.TextureName == "circ"
						|| poly.TextureName == "tire" || poly.TextureName == "tir2")
				{
					//dont use this poly - we do the wheels ourselves
					poly.VertexBufferIndex = -1;
					continue;
				}
				*/
				poly.VertexBufferIndex = vertCount;
				vertCount += poly.VertexCount;
				allVerts.AddRange(poly.GetVertices());
			}

			_vertexBuffer = new VertexBuffer(Engine.Instance.Device, typeof(VertexPositionTexture), vertCount, BufferUsage.WriteOnly);
			_vertexBuffer.SetData<VertexPositionTexture>(allVerts.ToArray());
		}

		public void Render(Effect effect)
		{
			Engine.Instance.Device.SetVertexBuffer(_vertexBuffer);
			Engine.Instance.Device.RasterizerState = RasterizerState.CullNone;

			effect.CurrentTechnique.Passes[0].Apply();

			foreach (Polygon poly in _polygons)
			{
				if (poly.VertexBufferIndex < 0)
					continue;

				Engine.Instance.Device.Textures[0] = poly.Texture;
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, poly.VertexBufferIndex, poly.VertexCount / 3);
			}
		}

		public void ChangeTextures(string textureName, string changeTo, BitmapChunk bitmaps)
		{
			BitmapEntry replacement = bitmaps.FindByName(changeTo);
			if (replacement == null)
				return;

			foreach (Polygon p in _polygons)
			{
				if (p.TextureName == textureName)
				{
					p.Texture = replacement.Texture;
				}
			}
		}
	}
}
