using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Loaders;
using OpenNFS1.Tracks;
using Microsoft.Xna.Framework.Input;
using OpenNFS1.Vehicles;
using OpenNFS1.Vehicles.AI;
using OpenNFS1.Physics;

namespace OpenNFS1.Parsers.Track
{

	class Track
	{
		AlphaTestEffect _effect;
		TrackSkyBox _skybox;
		BasicEffect _physicalRoadEffect;

		public TrackDescription Description { get; set; }
		public List<SceneryItem> SceneryItems { get; set; }
		public List<TrackNode> RoadNodes { get; set; }
		public List<TerrainSegment> TerrainSegments { get; set; }
		public VertexBuffer TerrainVertexBuffer { get; set; }
		public VertexBuffer FenceVertexBuffer { get; set; }
		public TrackfamFile TrackFam { get; set; }
		public int CheckpointNode { get; set; }
		public bool IsOpenRoad { get; set; }
		

		public Track()
		{
			_effect = new AlphaTestEffect(Engine.Instance.Device);
			_effect.ReferenceAlpha = 100;
			_effect.AlphaFunction = CompareFunction.Greater;
			_effect.VertexColorEnabled = false;
			_effect.FogEnabled = false;
			_physicalRoadEffect = new BasicEffect(Engine.Instance.Device);
		}

		public void Initialize()
		{
			_skybox = new TrackSkyBox(TrackFam.HorizonTexture);
		}

		public void Update()
		{
			_skybox.Update();
			foreach (var item in SceneryItems)
			{
				item.Update();
			}
		}

		public void Render(Vector3 cameraPosition, TrackNode currentNode)
		{
			_skybox.Render();
			_effect.View = Engine.Instance.Camera.View;
			_effect.Projection = Engine.Instance.Camera.Projection;
			_effect.World = Matrix.Identity;

			int segmentIndex = currentNode.Number / 4;
			var startSegment = TerrainSegments[segmentIndex];

			var renderedSegments = new List<TerrainSegment>();

			Engine.Instance.Device.SetVertexBuffer(TerrainVertexBuffer);
			_effect.CurrentTechnique.Passes[0].Apply();
			Engine.Instance.Device.RasterizerState = RasterizerState.CullNone;
			Engine.Instance.Device.SamplerStates[0] = SamplerState.PointWrap;

			var frustum = new BoundingFrustum(Engine.Instance.Camera.View * Engine.Instance.Camera.Projection);
			
			// draw segments from the player vehicle forwards. Stop when a segment is out of view
			var segment = startSegment;
			for (int i = 0; i < GameConfig.DrawDistance; i++)
			{
				if (segment == null) break;
				if (frustum.Intersects(segment.BoundingBox))
				{
					RenderSegment(segment);
					renderedSegments.Add(segment);
				}
				segment = segment.Next;
			}

			// draw segments from the player vehicle backwards. Stop when a segment is out of view
			segment = startSegment.Prev;
			for (int i = 0; i < GameConfig.DrawDistance; i++)
			{
				if (segment == null) break;
				if (frustum.Intersects(segment.BoundingBox))
				{
					RenderSegment(segment);
					renderedSegments.Add(segment);
				}
				segment = segment.Prev;
			}

			DrawScenery(renderedSegments);

			if (FenceVertexBuffer != null)
			{
				Engine.Instance.Device.SetVertexBuffer(FenceVertexBuffer);
				_effect.World = Matrix.Identity;
				_effect.CurrentTechnique.Passes[0].Apply();
				Engine.Instance.Device.SamplerStates[0] = SamplerState.PointWrap;
				foreach (var renderedSegment in renderedSegments)
				{
					DrawFenceStrips(renderedSegment);
				}
			}

			if (GameConfig.DrawDebugInfo)
			{
				var node = currentNode;
				for (int i = 0; i < GameConfig.DrawDistance; i++)
				{
					Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
						Matrix.CreateTranslation(node.Position), Color.Yellow,
						null);

					Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
						Matrix.CreateTranslation(node.GetLeftBoundary()), Color.Yellow,
						null);

					Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
						Matrix.CreateTranslation(node.GetRightBoundary()), Color.Yellow,
						null);

					Engine.Instance.GraphicsUtils.AddLine(node.GetLeftBoundary(), node.GetRightBoundary(), Color.Yellow);

					node = node.Next;
					if (node == null) break;
				}
			}			
		}

		private void RenderSegment(TerrainSegment segment)
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
			int offset = segment.FenceBufferIndex;
			if (segment.HasLeftFence)
			{
				Engine.Instance.Device.Textures[0] = segment.FenceTexture;
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, offset, 4);
				offset += 6;
			}
			if (segment.HasRightFence)
			{
				Engine.Instance.Device.Textures[0] = segment.FenceTexture;
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleStrip, offset, 4);
			}
		}

		private void DrawScenery(List<TerrainSegment> renderedSegments)
		{
			Engine.Instance.Device.RasterizerState = RasterizerState.CullNone;
			Engine.Instance.Device.SamplerStates[0] = SamplerState.PointClamp;

			if (GameConfig.Render2dScenery)
			{
				TrackBillboardModel.BeginBatch();
				foreach (SceneryItem scenery in SceneryItems)
				{
					if (!renderedSegments.Exists(a=> a.Number == scenery.SegmentRef))
						continue;

					if (scenery is BillboardSceneryItem || scenery is TwoSidedBillboardSceneryItem)
					{
						scenery.Render(_effect);
					}
				}
			}

			if (GameConfig.Render3dScenery)
			{
				foreach (SceneryItem model in SceneryItems)
				{
					if (model is ModelSceneryItem)
					{
						if (!renderedSegments.Exists(a => a.Number == model.SegmentRef))
							continue;

						model.Render(_effect);
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

		public void Dispose()
		{
			_effect.Dispose();
			TerrainVertexBuffer.Dispose();
			if (FenceVertexBuffer != null) FenceVertexBuffer.Dispose();
			TrackFam.Dispose();
		}
	}
}
