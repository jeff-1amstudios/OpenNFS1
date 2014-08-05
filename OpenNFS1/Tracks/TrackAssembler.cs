using System;
using System.Collections.Generic;
using System.Text;
using OpenNFS1.Parsers.Track;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using NfsEngine;
using OpenNFS1.Parsers;
using OpenNFS1.Tracks;
using System.Linq;

namespace OpenNFS1.Loaders
{
	class TrackAssembler
	{
		//public static readonly int TRIANGLES_PER_ROW = 12;
		public static readonly int TRIANGLES_PER_SEGMENT = 2; //48;
		const int OPEN_ROAD_CHECKPOINT_SCENERYITEM_ID = 124;
		public const int NbrTrianglesPerTerrainStrip = 8;
		public const int NbrVerticesPerTerrainStrip = 10;
		public const int NbrVerticesPerSegment = 10 * NbrVerticesPerTerrainStrip;
		public const int NbrNodesPerSegment = 4;

		private TrackfamFile _trackFam;
		private TriFile _tri;

		public string ProgressMessage;

		private void AddProgress(string progress)
		{
			ProgressMessage += "\r\n" + progress;
		}

		public Track Assemble(TriFile tri)
		{
			_tri = tri;
			AddProgress("Reading track scenery file");
			_trackFam = tri.IsOpenRoad ? new OpenRoadTrackfamFile(tri.FileName, GameConfig.AlternativeTimeOfDay) : new TrackfamFile(tri.FileName, GameConfig.AlternativeTimeOfDay);
			AssembleTrackSegments();

			Track track = new Track();
			track.RoadNodes = _tri.Nodes;
			track.SceneryItems = AssembleSceneryItems();
			track.TerrainSegments = _tri.Segments;
			track.TerrainVertexBuffer = AssembleTerrainVertices();
			track.FenceVertexBuffer = AssembleFenceVertices();
			track.TrackFam = _trackFam;
			track.IsOpenRoad = _tri.IsOpenRoad;

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
				track.CheckpointNode = _tri.Nodes.Count - 2;
			}

			track.Initialize();
			return track;
		}

		private List<SceneryItem> AssembleSceneryItems()
		{
			AddProgress("Assembling scenery items");
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
								var texture = _trackFam.GetSceneryTexture(nextBitmap.ResourceId);
								textures.Add(texture);
							}
							renderObject = new AnimatedBillboardSceneryItem(textures);
						}
						else
						{
							bitmapId = obj.Descriptor.ResourceId;
							renderObject = new BillboardSceneryItem(_trackFam.GetSceneryTexture(bitmapId));
						}
						break;

					case SceneryType.TwoSidedBitmap:

						bitmapId = obj.Descriptor.ResourceId;
						int bitmap2Id = obj.Descriptor.Resource2Id;
						renderObject = new TwoSidedBillboardSceneryItem(_trackFam.GetSceneryTexture(bitmapId),
																	_trackFam.GetSceneryTexture(bitmap2Id));
						break;

					case SceneryType.Model:
						int modelId = obj.Descriptor.ResourceId;
						renderObject = new ModelSceneryItem(_trackFam.GetMesh(modelId));
						break;

					default:
						throw new NotImplementedException();
				}

				renderObject.Size = new Vector2(obj.Descriptor.Width, obj.Descriptor.Height);
				renderObject.SegmentRef = obj.ReferenceNode / 4;
				renderObject.Position = _tri.Nodes[obj.ReferenceNode].Position + ((obj.RelativePosition * GameConfig.TerrainScale) * 240);
				renderObject.Orientation = MathHelper.ToRadians((_tri.Nodes[obj.ReferenceNode].Orientation) - ((obj.Orientation / 256) * 360));
				renderObject.Initialize();
				renderObjects.Add(renderObject);

			}

			return renderObjects;
		}


		private VertexBuffer AssembleTerrainVertices()
		{
			AddProgress("Assembling terrain vertices");
			List<VertexPositionTexture> vertices = new List<VertexPositionTexture>();

			for (int segmentIndex = 0; segmentIndex < _tri.Segments.Count; segmentIndex++)
			{
				var segment = _tri.Segments[segmentIndex];
				segment.TerrainBufferIndex = vertices.Count;

				//Ground textures are rotated 90 degrees, so we swap u,v coordinates around
				float tU = 0.0f;

				//Right side of road
				for (int j = 1; j < TriFile.NbrTerrainPointsPerSide; j++)
				{
					tU = 0.0f;
					int index1, index2;
					GetPointIndicesForRightSide(segment, j, out index1, out index2);
					
					for (int row = 0; row < TriFile.NbrRowsPerSegment; row++)
					{
						vertices.Add(new VertexPositionTexture(segment.Rows[row].GetPoint(index1), new Vector2(tU, 0.0f)));
						vertices.Add(new VertexPositionTexture(segment.Rows[row].GetPoint(index2), new Vector2(tU, 1.0f)));
						tU += 0.5f;
					}

					//attach the end of this segment to the start of the next if its not the last
					if (segment.Next != null)
					{
						vertices.Add(new VertexPositionTexture(segment.Next.Rows[0].GetPoint(index1), new Vector2(tU, 0.0f)));
						vertices.Add(new VertexPositionTexture(segment.Next.Rows[0].GetPoint(index2), new Vector2(tU, 1.0f)));
					}
					else
					{
						vertices.Add(new VertexPositionTexture(segment.Rows[TriFile.NbrRowsPerSegment - 1].GetPoint(index1), new Vector2(tU, 0.0f)));
						vertices.Add(new VertexPositionTexture(segment.Rows[TriFile.NbrRowsPerSegment - 1].GetPoint(index2), new Vector2(tU, 1.0f)));
					}
				}

				//Left side of road
				for (int j = 1; j < TriFile.NbrTerrainPointsPerSide; j++)
				{
					tU = 0.0f;
					int index1, index2;
					GetPointIndicesForLeftSide(segment, j, out index1, out index2);
					for (int row = 0; row < TriFile.NbrRowsPerSegment; row++)
					{
						vertices.Add(new VertexPositionTexture(segment.Rows[row].GetPoint(index1), new Vector2(tU, 1.0f)));
						vertices.Add(new VertexPositionTexture(segment.Rows[row].GetPoint(index2), new Vector2(tU, 0.0f)));
						tU += 0.5f;
					}

					//attach the end of this segment to the start of the next if its not the last
					if (segment.Next != null)
					{
						vertices.Add(new VertexPositionTexture(segment.Next.Rows[0].GetPoint(index1), new Vector2(tU, 1.0f)));
						vertices.Add(new VertexPositionTexture(segment.Next.Rows[0].GetPoint(index2), new Vector2(tU, 0.0f)));
					}
					else
					{
						vertices.Add(new VertexPositionTexture(segment.Rows[TriFile.NbrRowsPerSegment - 1].GetPoint(index1), new Vector2(tU, 1.0f)));
						vertices.Add(new VertexPositionTexture(segment.Rows[TriFile.NbrRowsPerSegment - 1].GetPoint(index2), new Vector2(tU, 0.0f)));
					}
				}
			}

			var vertexBuffer = new VertexBuffer(Engine.Instance.Device, typeof(VertexPositionTexture), vertices.Count, BufferUsage.WriteOnly);
			vertexBuffer.SetData<VertexPositionTexture>(vertices.ToArray());
			return vertexBuffer;
		}

		void GetPointIndicesForRightSide(TerrainSegment segment, int terrainPointIndex, out int index1, out int index2)
		{
			// only care if this is the last terrain strip
			if (terrainPointIndex == TriFile.NbrTerrainPointsPerSide - 1)
			{
				var node = _tri.Nodes[segment.Number * TriFile.NbrRowsPerSegment];
				if (node.NodeProperty == TrackNodeProperty.RIGHT_TUNNEL_A2_A9)
				{
					index1 = 2;
					index2 = 10;
					return;
				}
			}

			index1 = terrainPointIndex;
			index2 = terrainPointIndex - 1;
		}

		void GetPointIndicesForLeftSide(TerrainSegment segment, int terrainPointIndex, out int index1, out int index2)
		{
			// only care if this is the last terrain strip
			if (terrainPointIndex == TriFile.NbrTerrainPointsPerSide - 1)
			{
				var node = _tri.Nodes[segment.Number * TriFile.NbrRowsPerSegment];
				if (node.NodeProperty == TrackNodeProperty.LEFT_TUNNEL_A9_A4)
				{
					index1 = 9;
					index2 = 4;
					return;
				}
				if (node.NodeProperty == TrackNodeProperty.LEFT_TUNNEL_A9_A5)
				{
					index1 = 9;
					index2 = 5;
					return;
				}
			}

			index1 = TriFile.NbrTerrainPointsPerSide + terrainPointIndex - 1;
			index2 = TriFile.NbrTerrainPointsPerSide + terrainPointIndex;
		}

		

		// Fences are defined by the TerrainSegment. If fence is enabled, we draw a fence from row0 to row2, then row2 to row4.
		VertexBuffer AssembleFenceVertices()
		{
			AddProgress("Assembling fence vertices");
			Vector3 fenceHeight = new Vector3(0, GameConfig.TerrainScale * 50000, 0);

			List<VertexPositionTexture> vertices = new List<VertexPositionTexture>();

			for (int i = 0; i < _tri.Segments.Count; i++)
			{
				var segment = _tri.Segments[i];
				if (!(segment.HasLeftFence | segment.HasRightFence)) continue;

				segment.FenceBufferIndex = vertices.Count;
				segment.FenceTexture = _trackFam.GetFenceTexture(segment.FenceTextureId);
				var node0 = _tri.Nodes[i * NbrNodesPerSegment];
				var node2 = node0.Next.Next;
				var node4 = node2.Next.Next;

				if (segment.HasLeftFence)
				{
					vertices.Add(new VertexPositionTexture(node0.GetLeftBoundary(), new Vector2(0, 1)));
					vertices.Add(new VertexPositionTexture(node0.GetLeftBoundary() + fenceHeight, new Vector2(0, 0)));
					vertices.Add(new VertexPositionTexture(node2.GetLeftBoundary(), new Vector2(1.5f, 1)));
					vertices.Add(new VertexPositionTexture(node2.GetLeftBoundary() + fenceHeight, new Vector2(1.5f, 0)));
					vertices.Add(new VertexPositionTexture(node4.GetLeftBoundary(), new Vector2(3, 1)));
					vertices.Add(new VertexPositionTexture(node4.GetLeftBoundary() + fenceHeight, new Vector2(3, 0)));
				}

				if (segment.HasRightFence)
				{
					vertices.Add(new VertexPositionTexture(node0.GetRightBoundary(), new Vector2(0, 1)));
					vertices.Add(new VertexPositionTexture(node0.GetRightBoundary() + fenceHeight, new Vector2(0, 0)));
					vertices.Add(new VertexPositionTexture(node2.GetRightBoundary(), new Vector2(1.5f, 1)));
					vertices.Add(new VertexPositionTexture(node2.GetRightBoundary() + fenceHeight, new Vector2(1.5f, 0)));
					vertices.Add(new VertexPositionTexture(node4.GetRightBoundary(), new Vector2(3, 1)));
					vertices.Add(new VertexPositionTexture(node4.GetRightBoundary() + fenceHeight, new Vector2(3, 0)));
				}
			}

			if (vertices.Count == 0) return null;
			var vertexBuffer = new VertexBuffer(Engine.Instance.Device, typeof(VertexPositionTexture), vertices.Count, BufferUsage.WriteOnly);
			vertexBuffer.SetData<VertexPositionTexture>(vertices.ToArray());
			return vertexBuffer;
		}

		void AssembleTrackSegments()
		{
			AddProgress("Assembling track segments");

			const int textureCount = 10;
			foreach (var segment in _tri.Segments)
			{
				segment.Textures = new Texture2D[textureCount];
				for (int i = 0; i < textureCount; i++)
				{
					segment.Textures[i] = _trackFam.GetGroundTexture(segment.TextureIds[i]);
				}

				// calculate bounding size for this segment
				var firstRow = segment.Rows[0];
				var lastRow = segment.Next != null ? segment.Next.Rows[0] : segment.Rows[TriFile.NbrRowsPerSegment - 1];
				Vector3 min = new Vector3(float.MaxValue), max = new Vector3();
				List<Vector3> allPoints = new List<Vector3>();
				allPoints.AddRange(firstRow.LeftPoints);
				allPoints.AddRange(firstRow.RightPoints);
				allPoints.AddRange(lastRow.LeftPoints);
				allPoints.AddRange(lastRow.RightPoints);

				foreach (var pos in allPoints)
				{
					if (pos.X < min.X) min.X = pos.X;
					if (pos.X > max.X) max.X = pos.X;
					if (pos.Y < min.Y) min.Y = pos.Y;
					if (pos.Y > max.Y) max.Y = pos.Y;
					if (pos.Z < min.Z) min.Z = pos.Z;
					if (pos.Z > max.Z) max.Z = pos.Z;
				}
				
				segment.BoundingBox = new BoundingBox(min, max);
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
				
				Vector3 prevLeft = GetRoadOffsetPosition(node, -node.DistanceToLeftBarrier);
				Vector3 prevRight = GetRoadOffsetPosition(node, node.DistanceToRightBarrier);

				Vector3 currentLeft = nextNode.Position + Utility.RotatePoint(new Vector2(-node.DistanceToLeftBarrier, 0), -node.Orientation);
				Vector3 currentRight = nextNode.Position + Utility.RotatePoint(new Vector2(node.DistanceToRightBarrier, 0), -node.Orientation);

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