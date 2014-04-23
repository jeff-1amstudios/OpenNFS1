using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.Loaders;

namespace NeedForSpeed.Parsers.Track
{

	class Track
	{
		AlphaTestEffect _effect;
		Vector3 _startPosition;

		List<Triangle> _physicalRoadTriangles = new List<Triangle>();
		List<SceneryObject> _sceneryItems = new List<SceneryObject>();

		VertexBuffer _terrainVertexBuffer, _physicalRoadVertexBuffer;

		List<TerrainSegment> _terrainSegments;
		List<PhysicalRoadNode> _roadNodes;

		Texture2D _fenceTexture, _horizonTexture;
		BasicEffect _physicalRoadEffect;

		public List<SceneryObject> SceneryItems
		{
			set { _sceneryItems = value; }
			get { return _sceneryItems;  }
		}

		public Texture2D FenceTexture
		{
			set { _fenceTexture = value; }
		}

		public Texture2D HorizonTexture
		{
			get { return _horizonTexture; }
			set { _horizonTexture = value; }
		}

		public List<PhysicalRoadNode> RoadNodes
		{
			get { return _roadNodes; }
		}

		public List<Triangle> PhysicalRoadTriangles
		{
			get { return _physicalRoadTriangles; }
		}

		public List<TerrainSegment> TerrainSegments
		{
			get { return _terrainSegments; }
		}

		public Vector3 StartPosition
		{
			get { return _startPosition; }
		}

		public int CheckpointSegment;

		public Track(Vector3 startPosition)
		{
			_startPosition = startPosition;
			_effect = new AlphaTestEffect(Engine.Instance.Device);
			_effect.ReferenceAlpha = 100;
			_effect.AlphaFunction = CompareFunction.Greater;
			_effect.VertexColorEnabled = false;
			_effect.FogEnabled = false;
			_physicalRoadEffect = new BasicEffect(Engine.Instance.Device);
		}


		public void SetTerrainGeometry(List<VertexPositionTexture> verts, List<TerrainSegment> terrainNodes, List<Triangle> roadVertices, List<PhysicalRoadNode> roadNodes)
		{
			_terrainSegments = terrainNodes;
			_roadNodes = roadNodes;
			_terrainVertexBuffer = new VertexBuffer(Engine.Instance.Device, typeof(VertexPositionTexture), verts.Count, BufferUsage.WriteOnly);
			_terrainVertexBuffer.SetData<VertexPositionTexture>(verts.ToArray());

			_physicalRoadVertexBuffer = new VertexBuffer(Engine.Instance.Device, typeof(VertexPositionColor), roadVertices.Count * 3, BufferUsage.WriteOnly);
			List<VertexPositionColor> vpc = new List<VertexPositionColor>();

			Color c = Color.White;
			int i = 0;
			foreach (Triangle tri in roadVertices)
			{
				if (i == 2)
				{
					if (c == Color.Gray)
						c = Color.White;
					else
						c = Color.Gray;
					i = 0;
				}

				vpc.Add(new VertexPositionColor(tri.V1, c));
				vpc.Add(new VertexPositionColor(tri.V2, c));
				vpc.Add(new VertexPositionColor(tri.V3, c));
				i++;
			}
			_physicalRoadVertexBuffer.SetData<VertexPositionColor>(vpc.ToArray());

			_physicalRoadTriangles = roadVertices;
		}


		#region Physical Road Vertices
		//public void GenerateVertices()
		//{
		//    List<VertexPositionColor> verts = new List<VertexPositionColor>();

		//    Vector3 prevLeft = _nodes[0].Position + RotatePoint(new Vector2(-RoadWidth, 0), _nodes[0].Orientation);
		//    Vector3 prevRight = _nodes[0].Position + RotatePoint(new Vector2(RoadWidth, 0), _nodes[0].Orientation);

		//    for (int i = 0; i < _nodes.Count; i++)
		//    {
		//        RoadNode node = _nodes[i];
		//        RoadNode nextNode;

		//        if (i + 1 < _nodes.Count)
		//            nextNode = _nodes[i + 1];
		//        else
		//            nextNode = _nodes[0];  //join up to start line

		//        Color c = i % 2 == 0 ? Color.White : Color.LightGray;

		//        float zPos = node.Position.Z - nextNode.Position.Z;

		//        Vector3 currentLeft = node.Position + RotatePoint(new Vector2(-RoadWidth, zPos), node.Orientation);
		//        Vector3 currentRight = node.Position + RotatePoint(new Vector2(RoadWidth, zPos), node.Orientation);

		//        //Road slanting
		//        if (node.Slant < 0)
		//            currentLeft.Y -= node.Slant * 0.005f;
		//        else if (node.Slant > 0)
		//            currentRight.Y += node.Slant * 0.005f;

		//        verts.Add(new VertexPositionColor(prevLeft, c));
		//        verts.Add(new VertexPositionColor(currentLeft, c));
		//        verts.Add(new VertexPositionColor(prevRight, c));

		//        verts.Add(new VertexPositionColor(currentLeft, c));
		//        verts.Add(new VertexPositionColor(prevRight, c));
		//        verts.Add(new VertexPositionColor(currentRight, c));

		//        _roadVertices.Add(prevLeft);
		//        _roadVertices.Add(currentLeft);
		//        _roadVertices.Add(prevRight);
		//        _roadVertices.Add(currentLeft);
		//        _roadVertices.Add(prevRight);
		//        _roadVertices.Add(currentRight);

		//        prevLeft = currentLeft;
		//        prevRight = currentRight;
		//    }

		//    _vertexBuffer = new VertexBuffer(Engine.Instance.Device, VertexPositionColor.SizeInBytes * verts.Count, BufferUsage.WriteOnly);
		//    _vertexBuffer.SetData<VertexPositionColor>(verts.ToArray());
		//}
		#endregion


		public void Update(GameTime gameTime)
		{
			foreach (SceneryObject billboard in _sceneryItems)
			{
				billboard.Update(gameTime);
			}
		}

		public void RenderPhysicalRoad()
		{
			Engine.Instance.Device.SetVertexBuffer(_physicalRoadVertexBuffer);
			RasterizerState rs = new RasterizerState { CullMode = CullMode.None, FillMode = FillMode.Solid };
			Engine.Instance.Device.RasterizerState = rs;

			_physicalRoadEffect.LightingEnabled = false;
			_physicalRoadEffect.View = Engine.Instance.Camera.View;
			_physicalRoadEffect.Projection = Engine.Instance.Camera.Projection;
			_physicalRoadEffect.World = Matrix.Identity;
			_physicalRoadEffect.VertexColorEnabled = true;
			_physicalRoadEffect.TextureEnabled = false;
			_physicalRoadEffect.CurrentTechnique.Passes[0].Apply();

			foreach (EffectPass pass in _physicalRoadEffect.CurrentTechnique.Passes)
			{
				pass.Apply();
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, _physicalRoadTriangles.Count);
			}

			for (int i = 180; i < _roadNodes.Count; i++)
			{
				var node = _roadNodes[i];
				Vector3 left = TrackAssembler.GetRoadOffsetPosition(node, -node.DistanceToLeftBarrier); // node.Position + Utility.RotatePoint(new Vector2(-node.DistanceToLeftBarrier, zPos), node.Orientation);
				Vector3 right = TrackAssembler.GetRoadOffsetPosition(node, node.DistanceToRightBarrier); // node.Position + Utility.RotatePoint(new Vector2(node.DistanceToRightBarrier, zPos), node.Orientation);
				Engine.Instance.GraphicsUtils.AddWireframeCube(Matrix.CreateTranslation(left), Color.Yellow);
				Engine.Instance.GraphicsUtils.AddWireframeCube(Matrix.CreateTranslation(right), Color.LightSkyBlue);
			}

			//Engine.Instance.Device.RasterizerState = null;
		}

		public void Render(Vector3 cameraPosition, int currentTriangle)
		{
			_effect.View = Engine.Instance.Camera.View;
			_effect.Projection = Engine.Instance.Camera.Projection;
			_effect.World = Matrix.Identity;

			int drawDistance = GameConfig.DrawDistance;
			int currentSegment = GetRoadSegment(cameraPosition, currentTriangle) / 4;
			if (currentSegment < 0)
			{
				currentSegment = 0;
				drawDistance = _terrainSegments.Count - 2;
			}

			// Draw a little behind ourselves
			currentSegment -= 1;
			if (currentSegment < 0)
				currentSegment = _terrainSegments.Count + (currentSegment - 1);

			List<int> renderedSegments = new List<int>();
			List<int> vertexBufferIndexes = new List<int>(); //hack...

			Engine.Instance.Device.SetVertexBuffer(_terrainVertexBuffer);

			int vertexIndex = 0;

			for (int i = 0; i < currentSegment; i++)
			{
				vertexIndex += 100;
				vertexIndex += _terrainSegments[i].ExtraVerts;
			}

			_effect.CurrentTechnique.Passes[0].Apply();

			for (int i = currentSegment; i < currentSegment + drawDistance; i++)
			{
				int segmentNbr = i % _terrainSegments.Count;

				if (segmentNbr == 0)
					vertexIndex = 0;
				TerrainSegment segment = _terrainSegments[segmentNbr];

				Engine.Instance.Device.RasterizerState = RasterizerState.CullCounterClockwise;

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
				vertexIndex += segment.ExtraVerts;

				renderedSegments.Add(segmentNbr);
			}

			Engine.Instance.Device.RasterizerState = RasterizerState.CullNone;
			//var currentSampler = new SamplerState() { AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp }; // Engine.Instance.Device.SamplerStates[0];
			//var fenceSampler = new SamplerState() { AddressU = TextureAddressMode.Wrap };
			//Engine.Instance.Device.SamplerStates[0] = fenceSampler;
			//Engine.Instance.Device.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
			Engine.Instance.Device.Textures[0] = _fenceTexture;

			for (int i = 0; i < renderedSegments.Count; i++)
			{
				DrawFenceStrips(_terrainSegments[renderedSegments[i]], vertexBufferIndexes[i]);
			}

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
			vertexIndex += 10;
		}

		private void DrawFenceStrips(TerrainSegment segment, int vertexIndex)
		{
			if (segment.ExtraVerts == 0)
				return;

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

			if (GameConfig.Render2dScenery)
			{
				TrackBillboardModel.BeginBatch();
				foreach (SceneryObject billboard in _sceneryItems)
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
				foreach (SceneryObject billboard in _sceneryItems)
				{
					if (!renderedSegments.Contains(billboard.SegmentRef))
						continue;

					if (billboard is ModelScenery)
						billboard.Render(_effect);
				}
			}
		}

		public int GetRoadSegment(Vector3 position, int hintTriangle)
		{
			int triangle = GetRoadTriangle(position, hintTriangle);
			if (triangle > -1)
				triangle /= TrackAssembler.TRIANGLES_PER_SEGMENT;

			return triangle;
		}

		public int GetRoadTriangle(Vector3 position, int hintTriangle)
		{
			int triangleCount = hintTriangle <= 0 ? _physicalRoadTriangles.Count : TrackAssembler.TRIANGLES_PER_SEGMENT * 3;
			if (_physicalRoadTriangles.Count == triangleCount)
			{
			}
			int startTriangle = hintTriangle - TrackAssembler.TRIANGLES_PER_SEGMENT;
			if (startTriangle < 0)
				startTriangle = _physicalRoadTriangles.Count + startTriangle;

			int triangleIndex = startTriangle;
			for (int i = 0; i < triangleCount; i++)
			{
				if (triangleIndex >= _physicalRoadTriangles.Count)
					triangleIndex -= _physicalRoadTriangles.Count;

				Triangle t = _physicalRoadTriangles[triangleIndex];
				if (Utility.IsPointInsideTriangle(ref t.V1, ref t.V2, ref t.V3, ref position, 0.1f))
				{
					return triangleIndex;
				}
				triangleIndex++;
			}
			return -1;
		}
	}


	#region Helper classes

	class PhysicalRoadNode
	{
		public float DistanceToLeftVerge, DistanceToLeftBarrier;
		public float DistanceToRightVerge, DistanceToRightBarrier;
		public Vector3 Position;
		public float Slope;
		public Int32 Slant, ZOrientation, XOrientation;
		public float Orientation;

		public Vector3 GetLeftBoundary()
		{
			Vector3 position = Position + Utility.RotatePoint(new Vector2(-DistanceToLeftBarrier, 0), -Orientation);
			return position;
		}

		public Vector3 GetRightBoundary()
		{
			Vector3 position = Position + Utility.RotatePoint(new Vector2(DistanceToRightBarrier, 0), -Orientation);
			return position;
		}
	}

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
		public List<Texture2D> Textures = new List<Texture2D>();
		public bool HasLeftFence, HasRightFence;
		public List<Line> LeftBoundary, RightBoundary;
		public List<Line> LeftVerge, RightVerge;
		public byte[] TextureIds;

		public TerrainSegment()
		{
			LeftBoundary = new List<Line>();
			RightBoundary = new List<Line>();
			LeftVerge = new List<Line>();
			RightVerge = new List<Line>();
		}

		public int ExtraVerts
		{
			get { return (HasLeftFence ? 6 : 0) + (HasRightFence ? 6 : 0); }
		}
	}

	enum SceneryType
	{
		Model = 1,
		Bitmap = 4,
		TwoSidedBitmap = 6
	}

	class TerrainRow
	{
		public List<Vector3> Points;
		public PhysicalRoadNode RoadNode;
	}

	#endregion
}
