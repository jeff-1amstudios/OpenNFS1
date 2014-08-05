using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine;
using System.Diagnostics;
using OpenNFS1;

namespace OpenNFS1.Parsers
{
	class MeshChunk : BaseChunk
	{
		List<Vector3> _vertices = new List<Vector3>();
		List<Vector2> _vertexTextureMap = new List<Vector2>();
		List<Polygon> _polygons = new List<Polygon>();
		private List<string> _textureNames = new List<string>();
		public string Identifier;

		public List<Polygon> Polygons
		{
			get { return _polygons;  }
		}

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
			Identifier = identifer.Substring(0, identifer.IndexOf('\0'));
			Debug.WriteLine("Loading mesh " + Identifier);
			int textureNameCount = reader.ReadInt32();
			int textureNameBlockOffset = reader.ReadInt32();

			reader.ReadBytes(16);  //section 4,5 

			int polygonVertexMapBlockOffset = reader.ReadInt32();

			reader.ReadBytes(8); //section 6

			int labelCount = reader.ReadInt32();
			int polygonLabelBlockOffset = reader.ReadInt32();

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

			reader.BaseStream.Position = Offset + polygonLabelBlockOffset;
			ReadPolygonLabelBlock(reader, labelCount);
		}

		private void ReadVertexBlock(BinaryReader reader, int vertexCount)
		{
			for (int i = 0; i < vertexCount; i++)
			{
				float x = reader.ReadInt32();
				float y = reader.ReadInt32();
				float z = reader.ReadInt32(); 
				Vector3 vertex = new Vector3(x, y, -z) * GameConfig.MeshScale;
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
			for (int i = 0; i < polygonCount; i++)
			{
				byte typeFlag = reader.ReadByte();
				int shapeId = typeFlag & (0xff >> 5); // type = 3 or 4.  Held in the first 3 bits
				//bool computeTextureUVs = (typeFlag & (0x1 << 3)) != 0; // bit 3 is set if there are *no* texture co-ords (and we should infer them?)

				byte b1 = reader.ReadByte();
				byte textureNumber = reader.ReadByte();
				byte b2 = reader.ReadByte();
				int verticesIndex = reader.ReadInt32();
				int textureCoordsIndex = reader.ReadInt32();

				if (shapeId != 3 && shapeId != 4)
				{
					// something in Burnt Sienna has a value of 2.  Haven't investigated yet.
					continue;
				}	

				// if these 2 indexes are the same, there are no texture coords
				bool computeTextureUVs = verticesIndex == textureCoordsIndex;

				Polygon polygon = new Polygon((PolygonShape)shapeId, computeTextureUVs);
				polygon.TextureName = _textureNames[textureNumber];		
				
				if (polygon.TextureName == "dkfr")
				{

				}

				//Vertices for polygon
				polygonVertexMap.BaseStream.Position = verticesIndex * sizeof(int);
				int v1 = polygonVertexMap.ReadInt32();
				int v2 = polygonVertexMap.ReadInt32();
				int v3 = polygonVertexMap.ReadInt32();
				int v4 = polygonVertexMap.ReadInt32();

				//Debug.WriteLine(String.Format("Poly {8} {0} {9} ({7}): {1},{2},{3},{4}, / {5} {6} computeUvs: {10}", i, v1, v2, v3, v4, b1, b2, polygon.TextureName, polygon.Shape, typeFlag.ToString("X"), computeTextureUVs));

				//Texture co-ords for vertices
				if (!computeTextureUVs)
				{
					polygonVertexMap.BaseStream.Position = textureCoordsIndex * sizeof(int);
					int t1 = polygonVertexMap.ReadInt32();
					int t2 = polygonVertexMap.ReadInt32();
					int t3 = polygonVertexMap.ReadInt32();
					int t4 = polygonVertexMap.ReadInt32();

					polygon.Vertices[0] = _vertices[v1];
					polygon.Vertices[1] = _vertices[v2];
					polygon.Vertices[2] = _vertices[v3];
					polygon.TextureUVs[0] = _vertexTextureMap[t1];
					polygon.TextureUVs[1] = _vertexTextureMap[t2];
					polygon.TextureUVs[2] = _vertexTextureMap[t3];
					
					if (polygon.Shape == PolygonShape.Quad)
					{
						polygon.Vertices[3] = _vertices[v1];
						polygon.Vertices[4] = _vertices[v3];
						polygon.Vertices[5] = _vertices[v4];
						polygon.TextureUVs[3] = _vertexTextureMap[t1];
						polygon.TextureUVs[4] = _vertexTextureMap[t3];
						polygon.TextureUVs[5] = _vertexTextureMap[t4];
					}
				}
				else
				{
					if (polygon.Shape == PolygonShape.Quad)
					{
						polygon.Vertices[0] = _vertices[v1];
						polygon.Vertices[1] = _vertices[v2];
						polygon.Vertices[2] = _vertices[v3];
						polygon.Vertices[3] = _vertices[v1];
						polygon.Vertices[4] = _vertices[v3];
						polygon.Vertices[5] = _vertices[v4];
					}
					else
					{
						throw new NotImplementedException();
					}
				}

				_polygons.Add(polygon);
			}
		}

		private void ReadPolygonLabelBlock(BinaryReader reader, int labelCount)
		{
			for (int i = 0; i < labelCount; i++)
			{
				string label = new string(reader.ReadChars(8));
				label = label.Substring(0, label.IndexOf("\0"));
				int polyIndex = reader.ReadInt32();
				_polygons[polyIndex].Label = label;
			}
		}
	}
}
