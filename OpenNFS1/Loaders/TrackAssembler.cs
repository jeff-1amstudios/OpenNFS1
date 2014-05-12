using System;
using System.Collections.Generic;
using System.Text;
using NeedForSpeed.Parsers.Track;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using NfsEngine;
using OpenNFS1.Parsers;
using OpenNFS1.Tracks;

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
		//public static readonly int TRIANGLES_PER_ROW = 12;
		public static readonly int TRIANGLES_PER_SEGMENT = 2; //48;
		const int OPEN_ROAD_CHECKPOINT_SCENERYITEM_ID = 124;
		public const int NbrVerticesPerTerrainStrip = 10;
		public const int NbrVerticesPerSegment = 10 * NbrVerticesPerTerrainStrip;

		private TrackTextureProvider _textureProvider;
		private TriFile _tri;

		public Track Assemble(TriFile tri)
		{
			_tri = tri;
			_textureProvider = tri.IsOpenRoad ? new OpenRoadTextureProvider(tri.FileName) : new TrackTextureProvider(tri.FileName);
			AssembleTrackSegmentTextures();

			Vector3 startPos = tri.Nodes[0].Position + new Vector3(0, 0, -2);
			
			Track track = new Track();
			track.StartPosition = startPos;
			track.RoadNodes = _tri.Nodes;
			track.SceneryItems = AssembleSceneryItems();
			track.TerrainSegments = _tri.Terrain;
			track.SetTerrainVertices(AssembleTerrainVertices());
			track.SetHorizonTexture(_textureProvider.HorizonTexture);

			// Find checkpoint scenery object and use it to mark the end of the track.
			// Or ignore it and mark the end of the track as the last node to allow us to drive past the checkpoint!
			if (tri.IsOpenRoad && GameConfig.RespectOpenRoadCheckpoints)
			{
				foreach (var obj in _tri.Scenery)
				{
					if (obj.Descriptor.ResourceId == OPEN_ROAD_CHECKPOINT_SCENERYITEM_ID)
					{
						track.CheckpointNode = obj.ReferenceNode;
						break;
					}
				}
			}
			else
			{
				track.CheckpointNode = _tri.Nodes.Count - 1;
			}			

			return track;
		}

		private List<SceneryItem> AssembleSceneryItems()
		{
			List<SceneryItem> renderObjects = new List<SceneryItem>();

			foreach (var obj in _tri.Scenery)
			{
				SceneryItem renderObject;
				int bitmapId;

				switch (obj.Descriptor.Type)
				{
					case SceneryType.Bitmap:
						if ((obj.Descriptor.Flags & SceneryFlags.Animated) == SceneryFlags.Animated)
						{
							// Successive bitmaps are referenced by incrementing the scenery descriptor
							List<Texture2D> textures = new List<Texture2D>();
							for (int i = 0; i < obj.Descriptor.AnimationFrameCount; i++)
							{
								var nextBitmap = _tri.ObjectDescriptors[obj.Descriptor.Id + i];
								var texture = _textureProvider.GetSceneryTextureForId(nextBitmap.ResourceId);
								if (texture != null)
								{
									textures.Add(texture);
								}
								else
								{

								}
							}
							renderObject = new AnimatedBillboardScenery(textures);
						}
						else
						{
							bitmapId = obj.Descriptor.ResourceId;
							renderObject = new BillboardScenery(_textureProvider.GetSceneryTextureForId(bitmapId));
						}
						break;

					case SceneryType.TwoSidedBitmap:

						bitmapId = obj.Descriptor.ResourceId;
						int bitmap2Id = obj.Descriptor.Resource2Id;
						renderObject = new TwoSidedBillboardScenery(_textureProvider.GetSceneryTextureForId(bitmapId),
																	_textureProvider.GetSceneryTextureForId(bitmap2Id));
						break;

					case SceneryType.Model:
						int modelId = obj.Descriptor.ResourceId;
						renderObject = new ModelScenery(_textureProvider.GetMesh(modelId));
						break;

					default:
						throw new NotImplementedException();
				}

				renderObject.SceneryObject2 = obj;
				if (obj.Descriptor.Width == 0 && obj.Descriptor.Height == 0)
				{

				}
				renderObject.Size = new Vector2(obj.Descriptor.Width, obj.Descriptor.Height);
				renderObject.SegmentRef = obj.ReferenceNode / 4;
				renderObject.Position = _tri.Nodes[obj.ReferenceNode].Position + ((obj.RelativePosition * GameConfig.ScaleFactor) * 245);
				renderObject.Orientation = MathHelper.ToRadians((_tri.Nodes[obj.ReferenceNode].Orientation) - ((obj.Orientation / 256) * 360));
				renderObject.Initialize();
				renderObjects.Add(renderObject);

			}

			return renderObjects;
		}


		private List<VertexPositionTexture> AssembleTerrainVertices()
		{
			List<VertexPositionTexture> terrainVertices = new List<VertexPositionTexture>();

			for (int segmentIndex = 0; segmentIndex < _tri.Terrain.Count; segmentIndex++)
			{
				var segment = _tri.Terrain[segmentIndex];
				var nextSegment = _tri.Terrain[(segmentIndex + 1) % _tri.Terrain.Count];

				//Ground textures are rotated 90 degrees, so we swap u,v coordinates around
				float tU = 0.0f;

				//Right side of road
				for (int j = 1; j < TriFile.NbrTerrainPointsPerSide; j++)
				{
					tU = 0.0f;
					for (int row = 0; row < TriFile.NbrRowsPerSegment; row++)
					{
						terrainVertices.Add(new VertexPositionTexture(segment.Rows[row].RightPoints[j], new Vector2(tU, 0.0f)));
						terrainVertices.Add(new VertexPositionTexture(segment.Rows[row].RightPoints[j - 1], new Vector2(tU, 1.0f)));
						tU += 0.5f;
					}

					//attach the end of this segment to the start of the next
					terrainVertices.Add(new VertexPositionTexture(nextSegment.Rows[0].RightPoints[j], new Vector2(tU, 0.0f)));
					terrainVertices.Add(new VertexPositionTexture(nextSegment.Rows[0].RightPoints[j - 1], new Vector2(tU, 1.0f)));
				}

				//Left side of road
				for (int j = 1; j < TriFile.NbrTerrainPointsPerSide; j++)
				{
					tU = 0.0f;
					for (int row = 0; row < TriFile.NbrRowsPerSegment; row++)
					{
						terrainVertices.Add(new VertexPositionTexture(segment.Rows[row].LeftPoints[j - 1], new Vector2(tU, 1.0f)));
						terrainVertices.Add(new VertexPositionTexture(segment.Rows[row].LeftPoints[j], new Vector2(tU, 0.0f)));
						tU += 0.5f;
					}

					//attach the end of this segment to the start of the next
					terrainVertices.Add(new VertexPositionTexture(nextSegment.Rows[0].LeftPoints[j - 1], new Vector2(tU, 1.0f)));
					terrainVertices.Add(new VertexPositionTexture(nextSegment.Rows[0].LeftPoints[j], new Vector2(tU, 0.0f)));
				}
			}
			
			//for (int currentRow = rows.Count - 5; currentRow >= 0; currentRow -= 5)
			{
				// Insert Fence vertices from back to front (to preserve indexing)
				//TerrainSegment terrainSegment = terrainSegments[currentRow / 5];
				/*
				Vector3 right4 = GetRoadOffsetPosition(rows[currentRow + 4].RefNode, rows[currentRow + 4].RefNode.DistanceToRightBarrier);
				Vector3 right2 = GetRoadOffsetPosition(rows[currentRow + 2].RefNode, rows[currentRow + 2].RefNode.DistanceToRightBarrier);
				Vector3 right0 = GetRoadOffsetPosition(rows[currentRow].RefNode, rows[currentRow].RefNode.DistanceToRightBarrier);

				Vector3 left4 = GetRoadOffsetPosition(rows[currentRow + 4].RefNode, -rows[currentRow + 4].RefNode.DistanceToLeftBarrier);
				Vector3 left2 = GetRoadOffsetPosition(rows[currentRow + 2].RefNode, -rows[currentRow + 2].RefNode.DistanceToLeftBarrier);
				Vector3 left0 = GetRoadOffsetPosition(rows[currentRow].RefNode, -rows[currentRow].RefNode.DistanceToLeftBarrier);

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

				right4 = GetRoadOffsetPosition(rows[currentRow + 4].RefNode, rows[currentRow + 4].RefNode.DistanceToRightVerge);
				right2 = GetRoadOffsetPosition(rows[currentRow + 2].RefNode, rows[currentRow + 2].RefNode.DistanceToRightVerge);
				right0 = GetRoadOffsetPosition(rows[currentRow].RefNode, rows[currentRow].RefNode.DistanceToRightVerge);

				left4 = GetRoadOffsetPosition(rows[currentRow + 4].RefNode, -rows[currentRow + 4].RefNode.DistanceToLeftVerge);
				left2 = GetRoadOffsetPosition(rows[currentRow + 2].RefNode, -rows[currentRow + 2].RefNode.DistanceToLeftVerge);
				left0 = GetRoadOffsetPosition(rows[currentRow].RefNode, -rows[currentRow].RefNode.DistanceToLeftVerge);

				terrainSegment.LeftVerge.Add(new Line(left0, left2));
				terrainSegment.LeftVerge.Add(new Line(left2, left4));
				terrainSegment.RightVerge.Add(new Line(right0, right2));
				terrainSegment.RightVerge.Add(new Line(right2, right4));
				*/

				//foreach (byte textureId in terrainSegment.TextureIds)
				//	terrainSegment.Textures.Add(_textureProvider.GetGroundTextureForNbr(textureId));
			}
			//var physicalTris = GeneratePhysicalVertices(roadNodes);
			//track.SetTerrainGeometry(terrainVertices, terrainSegments, physicalTris, roadNodes);

			return terrainVertices;
		}

		void AssembleTrackSegmentTextures()
		{
			const int textureCount = 10;
			foreach (var segment in _tri.Terrain)
			{
				segment.Textures = new Texture2D[textureCount];
				for (int i = 0; i < textureCount; i++)
				{
					segment.Textures[i] = _textureProvider.GetGroundTextureForNbr(segment.TextureIds[i]);
				}
			}
		}

		public List<Triangle> GeneratePhysicalVertices(List<TrackNode> nodes)
		{
			List<VertexPositionColor> verts = new List<VertexPositionColor>();
			List<Triangle> roadVerts = new List<Triangle>();
			TrackNode nextNode;
			//Vector3 prevLeft = GetRoadOffsetPosition(nodes[nodes.Count - 1], -nodes[nodes.Count - 1].DistanceToLeftBarrier); // nodes[0].Position + Utility.RotatePoint(new Vector2(-nodes[0].DistanceToLeftBarrier, 0), nodes[0].Orientation);
			//Vector3 prevRight = GetRoadOffsetPosition(nodes[nodes.Count - 1], nodes[nodes.Count - 1].DistanceToRightBarrier); // nodes[0].Position + Utility.RotatePoint(new Vector2(nodes[0].DistanceToRightBarrier, 0), nodes[0].Orientation);

			for (int i = 0; i < nodes.Count; i++)
			{
				TrackNode node = nodes[i];

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
				float slantScale = 0.179f * GameConfig.ScaleFactor;
				if (Math.Abs(node.Slant) > 0)
				{
					//currentLeft.Y -= node.Slant * node.DistanceToLeftBarrier * slantScale;
					//currentRight.Y += node.Slant * node.DistanceToRightBarrier * slantScale;
					//prevLeft.Y -= node.Slant * node.DistanceToLeftBarrier * slantScale;
					//prevRight.Y += node.Slant * node.DistanceToRightBarrier * slantScale;
				}

				var t = new Triangle(nextNode.GetLeftBoundary(), node.GetLeftBoundary(), node.GetRightBoundary());
				var t2 = new Triangle(nextNode.GetLeftBoundary(), node.GetRightBoundary(), nextNode.GetRightBoundary());
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

		public static Vector3 GetRoadOffsetPosition(TrackNode roadNode, float offset)
		{
			Vector3 position = roadNode.Position + Utility.RotatePoint(new Vector2(offset, 0), -roadNode.Orientation);
			return position;
		}

	}
}