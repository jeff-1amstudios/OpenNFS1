using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.Loaders;
using OpenNFS1.Tracks;

namespace NeedForSpeed.Parsers.Track
{

	class Track
	{
		AlphaTestEffect _effect;
		Vector3 _startPosition;
		VertexBuffer _physicalRoadVertexBuffer;
		TrackSkyBox _skybox;
		BasicEffect _physicalRoadEffect;

		public List<SceneryItem> SceneryItems { get; set; }
		public List<TrackNode> RoadNodes { get; set; }
		public List<TerrainSegment> TerrainSegments { get; set; }
		public VertexBuffer TerrainVertexBuffer { get; set; }
		public VertexBuffer FenceVertexBuffer { get; set; }

		public Vector3 StartPosition { get; set; }

		public int CheckpointNode { get; set; }

		public Track()
		{
			_effect = new AlphaTestEffect(Engine.Instance.Device);
			_effect.ReferenceAlpha = 100;
			_effect.AlphaFunction = CompareFunction.Greater;
			_effect.VertexColorEnabled = false;
			_effect.FogEnabled = false;
			_physicalRoadEffect = new BasicEffect(Engine.Instance.Device);
		}

		public void SetHorizonTexture(Texture2D horizon)
		{
			_skybox = new TrackSkyBox(horizon);
		}

		public void Update(GameTime gameTime)
		{
			_skybox.Update(gameTime);
			foreach (var item in SceneryItems)
			{
				item.Update(gameTime);
			}
		}

		public void Render(Vector3 cameraPosition, TrackNode currentNode)
		{
			_skybox.Render();
			_effect.View = Engine.Instance.Camera.View;
			_effect.Projection = Engine.Instance.Camera.Projection;
			_effect.World = Matrix.Identity;

			int segmentIndex = currentNode.Number / 4;

			// Draw from 1 segment behind ourselves
			segmentIndex -= 1;
			if (segmentIndex < 0)
				segmentIndex = TerrainSegments.Count + (segmentIndex - 1);

			var startSegment = TerrainSegments[segmentIndex];

			List<int> renderedSegments = new List<int>();

			Engine.Instance.Device.SetVertexBuffer(TerrainVertexBuffer);
			_effect.CurrentTechnique.Passes[0].Apply();
			Engine.Instance.Device.SamplerStates[0] = SamplerState.PointWrap;

			var segment = startSegment;
			for (int i = 0; i < GameConfig.DrawDistance; i++)
			{
				int vertexIndex = segment.TerrainBufferIndex;

				DrawTerrainStrip(ref vertexIndex, 0, segment.Textures[0]);
				DrawTerrainStrip(ref vertexIndex, 1, segment.Textures[1]);
				DrawTerrainStrip(ref vertexIndex, 2, segment.Textures[2]);
				DrawTerrainStrip(ref vertexIndex, 3, segment.Textures[3]);
				DrawTerrainStrip(ref vertexIndex, 4, segment.Textures[4]);
				DrawTerrainStrip(ref vertexIndex, 5, segment.Textures[5]);
				DrawTerrainStrip(ref vertexIndex, 6, segment.Textures[6]);
				DrawTerrainStrip(ref vertexIndex, 7, segment.Textures[7]);
				DrawTerrainStrip(ref vertexIndex, 8, segment.Textures[8]);
				DrawTerrainStrip(ref vertexIndex, 9, segment.Textures[9]);

				renderedSegments.Add((segmentIndex + i) % TerrainSegments.Count);
				segment = segment.Next;
				if (segment == null) break;
			}

			DrawScenery(renderedSegments);

			Engine.Instance.Device.SetVertexBuffer(FenceVertexBuffer);
			_effect.World = Matrix.Identity;
			_effect.CurrentTechnique.Passes[0].Apply();
			Engine.Instance.Device.SamplerStates[0] = SamplerState.PointWrap;
			Engine.Instance.Device.RasterizerState = RasterizerState.CullNone;
			segment = startSegment;
			for (int i = 0; i < GameConfig.DrawDistance; i++)
			{
				DrawFenceStrips(segment);
				segment = segment.Next;
				if (segment == null) break;
			}

			//var node = currentNode;
			//for (int i = 0; i < 30; i++)
			//{

			//	//Engine.Instance.GraphicsUtils.AddLine(RoadNodes[i].Position, RoadNodes[i].Position + RoadNodes[i].Up * 50, Color.Yellow);

			//	Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
			//		Matrix.CreateTranslation(node.Position), Color.Yellow,
			//		null);

			//	Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
			//		Matrix.CreateTranslation(node.GetLeftBoundary()), Color.Yellow,
			//		null);

			//	Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
			//		Matrix.CreateTranslation(node.GetRightBoundary()), Color.Yellow,
			//		null);

			//	node = node.Next;
			//	if (node == null) break;
			//}/*
		}

		private void DrawTerrainStrip(ref int vertexIndex, int stripNumber, Texture2D texture)
		{
			if (texture != null)
			{
				Engine.Instance.Device.Textures[0] = texture;
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, vertexIndex, TrackAssembler.NbrTrianglesPerTerrainStrip);
			}
			vertexIndex += TrackAssembler.NbrVerticesPerTerrainStrip;
		}

		private void DrawFenceStrips(TerrainSegment segment)
		{
			if (segment.HasLeftFence)
			{
				Engine.Instance.Device.Textures[0] = segment.FenceTexture;
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, segment.FenceBufferIndex, 4);
			}
			if (segment.HasRightFence)
			{
				Engine.Instance.Device.Textures[0] = segment.FenceTexture;
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, segment.FenceBufferIndex + 6, 4);
			}
		}

		private void DrawScenery(List<int> renderedSegments)
		{
			Engine.Instance.Device.RasterizerState = RasterizerState.CullNone;
			Engine.Instance.Device.SamplerStates[0] = SamplerState.PointClamp;

			if (GameConfig.Render2dScenery)
			{
				TrackBillboardModel.BeginBatch();
				foreach (SceneryItem billboard in SceneryItems)
				{
					if (!renderedSegments.Contains(billboard.SegmentRef))
						continue;

					if (billboard is BillboardScenery || billboard is TwoSidedBillboardScenery)
					{
						billboard.Render(_effect);
					}
				}
			}

			if (GameConfig.Render3dScenery)
			{
				foreach (SceneryItem billboard in SceneryItems)
				{
					if (!renderedSegments.Contains(billboard.SegmentRef))
						continue;

					if (billboard is ModelScenery)
					{
						billboard.Render(_effect);
					}
				}
			}
		}

		public float GetHeightAtPoint(TrackNode node, Vector3 point)
		{
			Vector3 a = node.GetLeftBoundary();
			Vector3 b = node.GetRightBoundary();
			Vector3 c = node.Next.GetRightBoundary();
			Vector3 d = node.Next.GetLeftBoundary();
			Vector3 hitLoc;
			float t;
			
			Vector3 pos = point; pos.Y += 100;
			if (Utility.FindRayTriangleIntersection(ref pos, Vector3.Down, 1000f, ref a, ref b, ref c, out hitLoc, out t))
			{
				return hitLoc.Y;
			}
			else if (Utility.FindRayTriangleIntersection(ref pos, Vector3.Down, 1000f, ref a, ref c, ref d, out hitLoc, out t))
			{
				return hitLoc.Y;
			}
			else
			{
				return -9999;
			}
		}
	}


	#region Helper classes

	

	class Line
	{
		public Vector3 Point1, Point2, Normal;

		public Line(Vector3 point1, Vector3 point2)
		{
			Point1 = point1;
			Point2 = point2;
		}
	}


	class TerrainSegment
	{
		public Texture2D[] Textures = new Texture2D[10];
		public bool HasLeftFence, HasRightFence;
		public int FenceTextureId;
		public Texture2D FenceTexture;
		public List<Line> LeftBoundary, RightBoundary;
		public List<Line> LeftVerge, RightVerge;
		public byte[] TextureIds;
		public TerrainRow[] Rows = new TerrainRow[4];
		public int FenceBufferIndex;
		public int TerrainBufferIndex;
		public TerrainSegment Next;

		public TerrainSegment()
		{
			LeftBoundary = new List<Line>();
			RightBoundary = new List<Line>();
			LeftVerge = new List<Line>();
			RightVerge = new List<Line>();
		}

	}

	
	
	class TerrainRow
	{
		const int TerrainPositionScale = 500;
		public Vector3 MiddlePoint;
		public Vector3[] LeftPoints, RightPoints;

		public void RelativizeTo(Vector3 position)
		{
			MiddlePoint = position + MiddlePoint * TerrainPositionScale;
			for (int i = 0; i < LeftPoints.Length; i++)
			{
				LeftPoints[i] = position + LeftPoints[i] * TerrainPositionScale;
			}

			for (int i = 0; i < RightPoints.Length; i++)
			{
				RightPoints[i] = position + RightPoints[i] * TerrainPositionScale;
			}
		}
	}

	#endregion
}
