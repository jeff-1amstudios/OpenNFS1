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
		VertexBuffer _terrainVertexBuffer, _physicalRoadVertexBuffer;
		TrackSkyBox _skybox;
		BasicEffect _physicalRoadEffect;

		public List<SceneryItem> SceneryItems { get; set; }
		public List<TrackNode> RoadNodes { get; set; }
		public List<TerrainSegment> TerrainSegments { get; set; }

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

		public void SetTerrainVertices(List<VertexPositionTexture> vertices)
		{
			_terrainVertexBuffer = new VertexBuffer(Engine.Instance.Device, typeof(VertexPositionTexture), vertices.Count, BufferUsage.WriteOnly);
			_terrainVertexBuffer.SetData<VertexPositionTexture>(vertices.ToArray());
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

		public void Render(Vector3 cameraPosition, int currentNode)
		{
			_skybox.Render();
			_effect.View = Engine.Instance.Camera.View;
			_effect.Projection = Engine.Instance.Camera.Projection;
			_effect.World = Matrix.Identity;

			int drawDistance = GameConfig.DrawDistance;
			int currentSegment = currentNode / 4;
			if (currentSegment < 0)
			{
				currentSegment = 0;
				drawDistance = TerrainSegments.Count - 2;
			}

			// Draw a little behind ourselves
			currentSegment -= 1;
			if (currentSegment < 0)
				currentSegment = TerrainSegments.Count + (currentSegment - 1);

			List<int> renderedSegments = new List<int>();
			List<int> vertexBufferIndexes = new List<int>(); //hack...

			Engine.Instance.Device.SetVertexBuffer(_terrainVertexBuffer);

			int vertexIndex = 0;

			for (int i = 1; i < currentSegment+1; i++)
			{
				vertexIndex += TrackAssembler.NbrVerticesPerSegment;
			}

			_effect.CurrentTechnique.Passes[0].Apply();

			//var terrainSampler = new SamplerState() { AddressU = TextureAddressMode.Wrap, AddressV = TextureAddressMode.Wrap, Filter = TextureFilter.Point };
			Engine.Instance.Device.SamplerStates[0] = SamplerState.PointWrap;

			//RasterizerState rs = new RasterizerState { CullMode = CullMode.None, FillMode = FillMode.WireFrame };
			//Engine.Instance.Device.RasterizerState = rs; 

			for (int i = currentSegment; i < currentSegment + drawDistance; i++)
			{
				int segmentNbr = i % TerrainSegments.Count;

				if (segmentNbr == 0)
					vertexIndex = 0;
				TerrainSegment segment = TerrainSegments[segmentNbr];

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
				vertexBufferIndexes.Add(vertexIndex);

				renderedSegments.Add(segmentNbr);
			}

			Engine.Instance.Device.RasterizerState = RasterizerState.CullNone;

			for (int i = currentNode; i < currentNode + 30; i++)
			{
				if (i >= RoadNodes.Count) break;

				//Engine.Instance.GraphicsUtils.AddLine(RoadNodes[i].Position, RoadNodes[i].Position + RoadNodes[i].Up * 50, Color.Yellow);

				Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
					Matrix.CreateTranslation(RoadNodes[i].Position), Color.Yellow,
					null);

				Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
					Matrix.CreateTranslation(RoadNodes[i].GetLeftBoundary()), Color.Yellow,
					null);

				Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
					Matrix.CreateTranslation(RoadNodes[i].GetRightBoundary()), Color.Yellow,
					null);
			}/*
			for (int i = currentSegment; i < currentSegment+1; i++)
			{	
				Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
					Matrix.CreateTranslation(TerrainSegments[i].Rows[0].LeftPoints[0]), Color.Yellow,
					null);
				Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
					Matrix.CreateTranslation(TerrainSegments[i].Rows[0].LeftPoints[1]), Color.Blue,
					null);
				Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
					Matrix.CreateTranslation(TerrainSegments[i].Rows[1].LeftPoints[0]), Color.Green,
					null);
				Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
					Matrix.CreateTranslation(TerrainSegments[i].Rows[1].LeftPoints[1]), Color.Red,
					null);
			}*/
			
			
			//var currentSampler = new SamplerState() { AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp }; // Engine.Instance.Device.SamplerStates[0];
			//var fenceSampler = new SamplerState() { AddressU = TextureAddressMode.Wrap };
			//Engine.Instance.Device.SamplerStates[0] = fenceSampler;
			//Engine.Instance.Device.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
			//Engine.Instance.Device.Textures[0] = _fenceTexture;

			//for (int i = 0; i < renderedSegments.Count; i++)
			//{
			//	DrawFenceStrips(TerrainSegments[renderedSegments[i]], vertexBufferIndexes[i]);
			//}

			//Engine.Instance.Device.SamplerStates[0] = currentSampler;

			DrawScenery(renderedSegments);
		}

		private void DrawTerrainStrip(ref int vertexIndex, int stripNumber, Texture2D texture)
		{
			if (texture != null)
			{
				Engine.Instance.Device.Textures[0] = texture;
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, vertexIndex, 8);
			}
			vertexIndex += TrackAssembler.NbrVerticesPerTerrainStrip;
		}

		private void DrawFenceStrips(TerrainSegment segment, int vertexIndex)
		{
			if (segment.HasLeftFence)
			{
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, vertexIndex, 4);
				vertexIndex += 6;
			}
			if (segment.HasRightFence)
			{
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, vertexIndex, 4);
				vertexIndex += 6;
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

		public float GetHeightAtPoint(int node, Vector3 point)
		{
			Vector3 a = RoadNodes[node].GetLeftBoundary();
			Vector3 b = RoadNodes[node].GetRightBoundary();
			Vector3 c = RoadNodes[node + 1].GetRightBoundary();
			Vector3 d = RoadNodes[node + 1].GetLeftBoundary();
			Vector3 hitLoc;
			float t;
			
			Vector3 pos = point; pos.Y += 100;
			if (Utility.FindRayTriangleIntersection(ref pos, Vector3.Down, 1000f, ref a, ref b, ref c, out hitLoc, out t))
			{
				Engine.Instance.GraphicsUtils.AddLine(a, b, Color.Yellow);
				Engine.Instance.GraphicsUtils.AddLine(b, c, Color.Yellow);
				Engine.Instance.GraphicsUtils.AddLine(c, a, Color.Yellow);
				return hitLoc.Y;
			}
			else if (Utility.FindRayTriangleIntersection(ref pos, Vector3.Down, 1000f, ref a, ref c, ref d, out hitLoc, out t))
			{
				Engine.Instance.GraphicsUtils.AddLine(a, c, Color.Yellow);
				Engine.Instance.GraphicsUtils.AddLine(c, d, Color.Yellow);
				Engine.Instance.GraphicsUtils.AddLine(d, a, Color.Yellow);
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
		public List<Line> LeftBoundary, RightBoundary;
		public List<Line> LeftVerge, RightVerge;
		public byte[] TextureIds;
		public TerrainRow[] Rows = new TerrainRow[4];

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
