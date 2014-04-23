using System;
using System.Collections.Generic;
using System.Text;
using NeedForSpeed.Parsers.Track;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using NfsEngine;

namespace NeedForSpeed.Loaders
{
	class SceneryObjectDescriptor
	{
		public float Width, Height;
		public SceneryType Type;
		public int Image1Id, Image2Id;
		public int AnimationFrames;
	}

	class TrackAssembler
	{
		public static readonly Vector3 ScaleFactor = new Vector3(0.000080f); //, 0.000180f, 0.000180f);
		//public static readonly int TRIANGLES_PER_ROW = 12;
		public static readonly int TRIANGLES_PER_SEGMENT = 2; //48;
		public static readonly int OPEN_ROAD_CHECKPOINT_SCENERYITEM_ID = 124;

		private TrackTextureProvider _textureProvider;

		public Track Load(TrackDescription descriptor)
		{
			BinaryReader reader = new BinaryReader(File.Open(descriptor.FileName, FileMode.Open));

			byte trackVersion = reader.ReadByte();
			ITrackLoader loader = null;

			if (trackVersion == 0x10)
				loader = new Track10Loader();
			else if (trackVersion == 0x11)
				loader = new Track11Loader(descriptor);
			else
				throw new NotImplementedException();

			_textureProvider = descriptor.IsOpenRoad ? new OpenRoadTextureProvider(descriptor.FileName) : new TrackTextureProvider(descriptor.FileName);

			List<PhysicalRoadNode> nodes = new List<PhysicalRoadNode>();
			reader.BaseStream.Position = loader.PhysicalRoadOffset;
			ReadPhysicalRoadBlock(reader, nodes);

			Vector3 startPos = nodes[1].Position;
			if (!descriptor.IsOpenRoad)
				startPos = nodes[0].Position + new Vector3(0, 0, -2);

			Track track = new Track(startPos);

			reader.BaseStream.Position = loader.SceneryOffset;
			ReadSceneryObjectsBlock(reader, track, nodes);

			reader.BaseStream.Position = loader.TerrainOffset;
			ReadTerrainBlock(reader, loader, track, nodes);

			reader.Close();

			track.HorizonTexture = _textureProvider.HorizonTexture;
			track.FenceTexture = _textureProvider.GetFenceTexture(loader.FenceTextureId);
			if (descriptor.IsOpenRoad)
			{
				foreach (var sceneryItem in track.SceneryItems) {
					if (sceneryItem is BillboardScenery) {
						if ((sceneryItem as BillboardScenery).TextureId == OPEN_ROAD_CHECKPOINT_SCENERYITEM_ID)
							track.CheckpointSegment = sceneryItem.SegmentRef;
					}
				}
			}

			return track;
		}

		public void ReadPhysicalRoadBlock(BinaryReader reader, List<PhysicalRoadNode> nodes)
		{
			for (int i = 0; i < 2400; i++)
			{
				PhysicalRoadNode node = new PhysicalRoadNode();
				float scale = 8000;
				node.DistanceToLeftVerge = reader.ReadByte();
				node.DistanceToLeftVerge *= ScaleFactor.X * scale;

				node.DistanceToRightVerge = reader.ReadByte();
				node.DistanceToRightVerge *= ScaleFactor.X * scale;

				node.DistanceToLeftBarrier = reader.ReadByte();
				node.DistanceToLeftBarrier *= ScaleFactor.X * scale;

				node.DistanceToRightBarrier = reader.ReadByte();
				node.DistanceToRightBarrier *= ScaleFactor.X * scale;

				reader.BaseStream.Position += 4;

				node.Position = new Vector3(reader.ReadInt32(), reader.ReadInt32(), -reader.ReadInt32()) * ScaleFactor;

				Int16 sl = reader.ReadInt16();
				sl /= 0x3ff;
				if (sl > 8192)
					sl -= 16384;
				node.Slope = sl;

				node.Slant = reader.ReadInt16(); //weird slant-A

				float orientation = (float)reader.ReadInt16();
				if (orientation != 0)
				{
				}
				if (orientation > 8192)
				{
					orientation -= 16384;
				}
				node.Orientation = (Int16)((orientation / 0x3FFF) * -360);  //convert to degrees
				//Debug.WriteLine(orientation + ", " + node.Orientation);
				//if (node.Orientation < -180)
				//    node.Orientation = node.Orientation + 360;
				//if (node.Orientation < 0) node.Orientation = node.Orientation + 360;

				Int16 unk1 = reader.ReadInt16(); // 0
				node.ZOrientation = reader.ReadInt16();
				node.Slant = reader.ReadInt16(); // slant-B
				node.XOrientation = reader.ReadInt16();
				unk1 = reader.ReadInt16(); // 0

				if (node.Position != Vector3.Zero)
				{
					nodes.Add(node);
				}
			}
		}


		private void ReadSceneryObjectsBlock(BinaryReader reader, Track track, List<PhysicalRoadNode> roadNodes)
		{
			List<SceneryObject> billboards = new List<SceneryObject>();
			reader.BaseStream.Position += 1800; //jump over index
			reader.ReadInt32(); //0x40
			int objectRecordCount = reader.ReadInt32();
			reader.ReadChars(4); //'OBJS'
			reader.BaseStream.Position += 8;

			List<SceneryObjectDescriptor> sceneryDescriptors = new List<SceneryObjectDescriptor>();
			for (int i = 0; i < 64; i++)
			{
				SceneryObjectDescriptor sd = new SceneryObjectDescriptor();
				byte[] bytes = reader.ReadBytes(16);

				sd.Width = bytes[6];
				sd.Width *= ScaleFactor.X * 65000;
				sd.Height = bytes[14];
				sd.Height *= ScaleFactor.Y * 65000;
				sd.Image1Id = bytes[2];
				sd.Image2Id = bytes[3];
				sd.Type = (SceneryType)bytes[1];
				sd.AnimationFrames = bytes[8];

				sceneryDescriptors.Add(sd);

			}

			long currentPos = reader.BaseStream.Position;

			Random random = new Random();

			for (int i = 0; i < 1000; i++)
			{
				int refNode = reader.ReadInt32();
				if (refNode == -1)
				{
					break;
				}
				int sceneryId = reader.ReadByte();
				float flip = (float)reader.ReadByte();
				byte[] flags = reader.ReadBytes(4);
				Vector3 relativePos = new Vector3(reader.ReadInt16(), reader.ReadInt16(), -reader.ReadInt16());

				SceneryObjectDescriptor descriptor = sceneryDescriptors[sceneryId];

				SceneryObject sceneryObject;

				switch (descriptor.Type)
				{
					case SceneryType.Bitmap:
						if (descriptor.AnimationFrames > 1)
						{
							List<Texture2D> textures = new List<Texture2D>();
							for (int j = 0; j < descriptor.AnimationFrames; j++)
							{
								textures.Add(_textureProvider.GetSceneryTextureForId(sceneryDescriptors[sceneryId + j].Image1Id));
							}
							sceneryObject = new AnimatedBillboardScenery(textures, random.NextDouble());
						}
						else
						{
							sceneryObject = new BillboardScenery(_textureProvider.GetSceneryTextureForId(descriptor.Image1Id), descriptor.Image1Id);
							//FileStream fs = new FileStream("c:\\temp\\nfsse\\" + descriptor.Image1Id + ".jpg", FileMode.Create);
							//sceneryObject.Texture.SaveAsJpeg(fs, sceneryObject.Texture.Width, sceneryObject.Texture.Height);
							//fs.Close();
						}
						break;

					case SceneryType.TwoSidedBitmap:
						sceneryObject = new TwoSidedBillboardScenery(
	_textureProvider.GetSceneryTextureForId(descriptor.Image1Id),
	_textureProvider.GetSceneryTextureForId(descriptor.Image2Id));
						break;

					case SceneryType.Model:
						sceneryObject = new ModelScenery(_textureProvider.GetMesh(descriptor.Image1Id));
						break;

					default:
						throw new NotImplementedException();
				}

				refNode %= roadNodes.Count;
				sceneryObject.SegmentRef = refNode / 4;
				sceneryObject.Size = new Vector2(descriptor.Width, descriptor.Height);
				sceneryObject.Position = roadNodes[refNode].Position + ((relativePos * ScaleFactor) * 245);
				sceneryObject.Orientation = MathHelper.ToRadians((roadNodes[refNode].Orientation) - ((flip / 256) * 360));
				sceneryObject.Initialize();
				billboards.Add(sceneryObject);

			}
			reader.BaseStream.Position = currentPos + 16000; //jump to end of billboards            
			reader.BaseStream.Position += 492; //blank space

			track.SceneryItems = billboards;
		}


		private void ReadTerrainBlock(BinaryReader reader, ITrackLoader loader, Track track, List<PhysicalRoadNode> roadNodes)
		{
			List<Triangle> physicalRoad = new List<Triangle>();
			List<VertexPositionTexture> terrainVertices = new List<VertexPositionTexture>();

			List<Triangle> leftSideTerrain = new List<Triangle>(), rightSideTerrain = new List<Triangle>();

			List<TerrainSegment> terrainSegments = new List<TerrainSegment>();
			List<TerrainRow> rows = new List<TerrainRow>();
			loader.LoadTerrain(reader, terrainSegments, rows, roadNodes);

			long fileSize = reader.BaseStream.Length;
			Vector3 fenceHeight = new Vector3(0, ScaleFactor.Y * 80000, 0);

			for (int currentRow = 0; currentRow < rows.Count; currentRow += 5)
			{
				#region Create triangles
				for (int row = currentRow; row < currentRow + 4; row++)
				{
					//left side terrain

					leftSideTerrain.Add(new Triangle(rows[row].Points[7], rows[row].Points[6], rows[row + 1].Points[6]));
					leftSideTerrain.Add(new Triangle(rows[row + 1].Points[6], rows[row + 1].Points[7], rows[row].Points[7]));
					leftSideTerrain.Add(new Triangle(rows[row].Points[8], rows[row].Points[7], rows[row + 1].Points[7]));
					leftSideTerrain.Add(new Triangle(rows[row + 1].Points[7], rows[row + 1].Points[8], rows[row].Points[8]));
					leftSideTerrain.Add(new Triangle(rows[row].Points[9], rows[row].Points[8], rows[row + 1].Points[8]));
					leftSideTerrain.Add(new Triangle(rows[row + 1].Points[8], rows[row + 1].Points[9], rows[row].Points[9]));
					leftSideTerrain.Add(new Triangle(rows[row].Points[10], rows[row].Points[9], rows[row + 1].Points[9]));
					leftSideTerrain.Add(new Triangle(rows[row + 1].Points[9], rows[row + 1].Points[10], rows[row].Points[10]));


					//left side physical road

					physicalRoad.Add(new Triangle(rows[row].Points[6], rows[row].Points[0], rows[row + 1].Points[0]));
					physicalRoad.Add(new Triangle(rows[row + 1].Points[0], rows[row + 1].Points[6], rows[row].Points[6]));
					physicalRoad.Add(new Triangle(rows[row].Points[7], rows[row].Points[6], rows[row + 1].Points[6]));
					physicalRoad.Add(new Triangle(rows[row + 1].Points[6], rows[row + 1].Points[7], rows[row].Points[7]));
					physicalRoad.Add(new Triangle(rows[row].Points[8], rows[row].Points[7], rows[row + 1].Points[7]));
					physicalRoad.Add(new Triangle(rows[row + 1].Points[7], rows[row + 1].Points[8], rows[row].Points[8]));
					//right side physical road
					physicalRoad.Add(new Triangle(rows[row].Points[0], rows[row].Points[1], rows[row + 1].Points[1]));
					physicalRoad.Add(new Triangle(rows[row + 1].Points[1], rows[row + 1].Points[0], rows[row].Points[0]));
					physicalRoad.Add(new Triangle(rows[row].Points[1], rows[row].Points[2], rows[row + 1].Points[2]));
					physicalRoad.Add(new Triangle(rows[row + 1].Points[2], rows[row + 1].Points[1], rows[row].Points[1]));
					physicalRoad.Add(new Triangle(rows[row].Points[2], rows[row].Points[3], rows[row + 1].Points[3]));
					physicalRoad.Add(new Triangle(rows[row + 1].Points[3], rows[row + 1].Points[2], rows[row].Points[2]));

					//right side terrain

					rightSideTerrain.Add(new Triangle(rows[row].Points[1], rows[row].Points[2], rows[row + 1].Points[2]));
					rightSideTerrain.Add(new Triangle(rows[row + 1].Points[2], rows[row + 1].Points[1], rows[row].Points[1]));
					rightSideTerrain.Add(new Triangle(rows[row].Points[2], rows[row].Points[3], rows[row + 1].Points[3]));
					rightSideTerrain.Add(new Triangle(rows[row + 1].Points[3], rows[row + 1].Points[2], rows[row].Points[2]));
					rightSideTerrain.Add(new Triangle(rows[row].Points[3], rows[row].Points[4], rows[row + 1].Points[4]));
					rightSideTerrain.Add(new Triangle(rows[row + 1].Points[4], rows[row + 1].Points[3], rows[row].Points[3]));
					rightSideTerrain.Add(new Triangle(rows[row].Points[4], rows[row].Points[5], rows[row + 1].Points[5]));
					rightSideTerrain.Add(new Triangle(rows[row + 1].Points[5], rows[row + 1].Points[4], rows[row].Points[4]));
				}

				#endregion

				float tU = 0.0f;

				//Right side of road
				for (int j = 1; j < 6; j++)
				{
					tU = 0.0f;
					for (int row = currentRow; row < currentRow + 5; row++)
					{
						terrainVertices.Add(new VertexPositionTexture(rows[row].Points[j], new Vector2(tU, 0.0f)));
						terrainVertices.Add(new VertexPositionTexture(rows[row].Points[j - 1], new Vector2(tU, 1.0f)));
						tU += 0.25f;
					}
				}

				//Left side of road
				for (int j = 6; j < 11; j++)
				{
					tU = 0.0f;
					for (int row = currentRow; row < currentRow + 5; row++)
					{
						Vector3 position = rows[row].Points[j - 1];
						if (j == 6)
							position = rows[row].Points[0];

						terrainVertices.Add(new VertexPositionTexture(position, new Vector2(tU, 1.0f)));
						terrainVertices.Add(new VertexPositionTexture(rows[row].Points[j], new Vector2(tU, 0.0f)));

						tU += 0.25f;
					}
				}
			}

			// Insert Fence vertices from back to front (to preserve indexing)
			for (int currentRow = rows.Count - 5; currentRow >= 0; currentRow -= 5)
			{
				TerrainSegment terrainSegment = terrainSegments[currentRow / 5];

				Vector3 right4 = GetRoadOffsetPosition(rows[currentRow + 4].RoadNode, rows[currentRow + 4].RoadNode.DistanceToRightBarrier);
				Vector3 right2 = GetRoadOffsetPosition(rows[currentRow + 2].RoadNode, rows[currentRow + 2].RoadNode.DistanceToRightBarrier);
				Vector3 right0 = GetRoadOffsetPosition(rows[currentRow].RoadNode, rows[currentRow].RoadNode.DistanceToRightBarrier);

				Vector3 left4 = GetRoadOffsetPosition(rows[currentRow + 4].RoadNode, -rows[currentRow + 4].RoadNode.DistanceToLeftBarrier);
				Vector3 left2 = GetRoadOffsetPosition(rows[currentRow + 2].RoadNode, -rows[currentRow + 2].RoadNode.DistanceToLeftBarrier);
				Vector3 left0 = GetRoadOffsetPosition(rows[currentRow].RoadNode, -rows[currentRow].RoadNode.DistanceToLeftBarrier);

				if (terrainSegment.HasRightFence)
				{
					right0.Y = Utility.GetHeightAtPoint(rightSideTerrain, right0);
					right2.Y = Utility.GetHeightAtPoint(rightSideTerrain, right2);
					right4.Y = Utility.GetHeightAtPoint(rightSideTerrain, right4);

					terrainVertices.InsertRange(20 * (currentRow + 5), new VertexPositionTexture[] {
                        new VertexPositionTexture(right0, new Vector2(0, 1)),
                        new VertexPositionTexture(right0 + fenceHeight, new Vector2(0, 0)),
                        new VertexPositionTexture(right2, new Vector2(1.5f, 1)),
                        new VertexPositionTexture(right2 + fenceHeight, new Vector2(1.5f, 0)),
                        new VertexPositionTexture(right4, new Vector2(3, 1)),
                        new VertexPositionTexture(right4 + fenceHeight, new Vector2(3, 0))
                    });
				}

				if (terrainSegment.HasLeftFence)
				{
					left0.Y = Utility.GetHeightAtPoint(leftSideTerrain, left0);
					left2.Y = Utility.GetHeightAtPoint(leftSideTerrain, left2);
					left4.Y = Utility.GetHeightAtPoint(leftSideTerrain, left4);

					terrainVertices.InsertRange(20 * (currentRow + 5), new VertexPositionTexture[] {
                        new VertexPositionTexture(left0, new Vector2(0, 1)),
                        new VertexPositionTexture(left0 + fenceHeight, new Vector2(0, 0)),
                        new VertexPositionTexture(left2, new Vector2(1.5f, 1)),
                        new VertexPositionTexture(left2 + fenceHeight, new Vector2(1.5f, 0)),
                        new VertexPositionTexture(left4, new Vector2(3, 1)),
                        new VertexPositionTexture(left4 + fenceHeight, new Vector2(3, 0))
                    });
				}

				terrainSegment.LeftBoundary.Add(new Line(left0, left2));
				terrainSegment.LeftBoundary.Add(new Line(left2, left4));
				terrainSegment.RightBoundary.Add(new Line(right0, right2));
				terrainSegment.RightBoundary.Add(new Line(right2, right4));

				right4 = GetRoadOffsetPosition(rows[currentRow + 4].RoadNode, rows[currentRow + 4].RoadNode.DistanceToRightVerge);
				right2 = GetRoadOffsetPosition(rows[currentRow + 2].RoadNode, rows[currentRow + 2].RoadNode.DistanceToRightVerge);
				right0 = GetRoadOffsetPosition(rows[currentRow].RoadNode, rows[currentRow].RoadNode.DistanceToRightVerge);

				left4 = GetRoadOffsetPosition(rows[currentRow + 4].RoadNode, -rows[currentRow + 4].RoadNode.DistanceToLeftVerge);
				left2 = GetRoadOffsetPosition(rows[currentRow + 2].RoadNode, -rows[currentRow + 2].RoadNode.DistanceToLeftVerge);
				left0 = GetRoadOffsetPosition(rows[currentRow].RoadNode, -rows[currentRow].RoadNode.DistanceToLeftVerge);

				terrainSegment.LeftVerge.Add(new Line(left0, left2));
				terrainSegment.LeftVerge.Add(new Line(left2, left4));
				terrainSegment.RightVerge.Add(new Line(right0, right2));
				terrainSegment.RightVerge.Add(new Line(right2, right4));


				foreach (byte textureId in terrainSegment.TextureIds)
					terrainSegment.Textures.Add(_textureProvider.GetGroundTextureForNbr(textureId));
			}
			var physicalTris = GeneratePhysicalVertices(roadNodes);
			track.SetTerrainGeometry(terrainVertices, terrainSegments, physicalTris, roadNodes);
		}

		public List<Triangle> GeneratePhysicalVertices(List<PhysicalRoadNode> nodes)
		{
			List<VertexPositionColor> verts = new List<VertexPositionColor>();
			List<Triangle> roadVerts = new List<Triangle>();
			PhysicalRoadNode nextNode;
			//Vector3 prevLeft = GetRoadOffsetPosition(nodes[nodes.Count - 1], -nodes[nodes.Count - 1].DistanceToLeftBarrier); // nodes[0].Position + Utility.RotatePoint(new Vector2(-nodes[0].DistanceToLeftBarrier, 0), nodes[0].Orientation);
			//Vector3 prevRight = GetRoadOffsetPosition(nodes[nodes.Count - 1], nodes[nodes.Count - 1].DistanceToRightBarrier); // nodes[0].Position + Utility.RotatePoint(new Vector2(nodes[0].DistanceToRightBarrier, 0), nodes[0].Orientation);

			for (int i = 0; i < nodes.Count; i++)
			{
				PhysicalRoadNode node = nodes[i];

				if (i + 1 < nodes.Count)
					nextNode = nodes[i + 1];
				else
					nextNode = nodes[0];  //join up to start line


				float zPos = node.Position.Z - nextNode.Position.Z;
				
				Vector3 prevLeft = GetRoadOffsetPosition(node, -node.DistanceToLeftBarrier); // node.Position + Utility.RotatePoint(new Vector2(-node.DistanceToLeftBarrier, zPos), node.Orientation);
				Vector3 prevRight = GetRoadOffsetPosition(node, node.DistanceToRightBarrier); // node.Position + Utility.RotatePoint(new Vector2(node.DistanceToRightBarrier, zPos), node.Orientation);

				Vector3 currentLeft = nextNode.Position + Utility.RotatePoint(new Vector2(-node.DistanceToLeftBarrier, 0), -node.Orientation);
				Vector3 currentRight = nextNode.Position + Utility.RotatePoint(new Vector2(node.DistanceToRightBarrier, 0), -node.Orientation);
				

				//Road slanting
				float slantScale = 0.179f * ScaleFactor.X;
				if (Math.Abs(node.Slant) > 0)
				{
					currentLeft.Y -= node.Slant * node.DistanceToLeftBarrier * slantScale;
					currentRight.Y += node.Slant * node.DistanceToRightBarrier * slantScale;
					prevLeft.Y -= node.Slant * node.DistanceToLeftBarrier * slantScale;
					prevRight.Y += node.Slant * node.DistanceToRightBarrier * slantScale;
				}

				var t = new Triangle(currentLeft, prevLeft, prevRight);
				var t2 = new Triangle(currentLeft, prevRight, currentRight);
				roadVerts.Add(t);
				roadVerts.Add(t2);

				var normal = Vector3.Normalize(Vector3.Cross(t.V2 - t.V1, t.V3 - t.V1));
				var normal2 = Vector3.Normalize(Vector3.Cross(t2.V2 - t2.V1, t2.V3 - t2.V1));
				if (Math.Round(normal.X, 2) != Math.Round(normal2.X, 2) ||
					Math.Round(normal.Y, 2) != Math.Round(normal2.Y, 2) ||
					Math.Round(normal.Z, 2) != Math.Round(normal2.Z, 2))
				{

				}

				//roadVerts.Add(prevLeft);
				//roadVerts.Add(currentLeft);
				//roadVerts.Add(prevRight);
				//roadVerts.Add(currentLeft);
				//roadVerts.Add(prevRight);
				//roadVerts.Add(currentRight);

				prevLeft = currentLeft;
				prevRight = currentRight;
			}

			//_vertexBuffer = new VertexBuffer(Engine.Instance.Device, VertexPositionColor.SizeInBytes * verts.Count, BufferUsage.WriteOnly);
			//_vertexBuffer.SetData<VertexPositionColor>(verts.ToArray());
			return roadVerts;
		}

		public static Vector3 GetRoadOffsetPosition(PhysicalRoadNode roadNode, float offset)
		{
			Vector3 position = roadNode.Position + Utility.RotatePoint(new Vector2(offset, 0), -roadNode.Orientation);
			return position;
		}

	}
}