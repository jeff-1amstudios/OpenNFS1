using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.Parsers;
using NfsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNFS1.Vehicles
{
	class CarMesh : Mesh
	{
		Polygon _rightRearWheel, _leftRearWheel, _rightFrontWheel, _leftFrontWheel;
		Texture2D _wheelTexture;

		public CarMesh(MeshChunk meshChunk, BitmapChunk bmpChunk)
			: base(meshChunk, bmpChunk)
		{
			foreach (var poly in _polys)
			{
				switch (poly.Label)
				{
					case "rt_rear":
						_rightRearWheel = poly;
						break;
					case "lt_rear":
						_leftRearWheel = poly;
						break;
					case "rt_frnt":
						_rightFrontWheel = poly;
						break;
					case "lt_frnt":
						_leftFrontWheel = poly;
						break;
				}
			}

			_wheelTexture = bmpChunk.FindByName("tyr1").Texture;
		}

		public Vector3 LeftFrontWheelPos { get { return GetWheelAxlePoint(_leftFrontWheel); } }
		public Vector3 RightFrontWheelPos { get { return GetWheelAxlePoint(_rightFrontWheel); } }
		public Vector3 LeftRearWheelPos { get { return GetWheelAxlePoint(_leftRearWheel); } }
		public Vector3 RightRearWheelPos { get { return GetWheelAxlePoint(_rightRearWheel); } }

		private Vector3 GetWheelAxlePoint(Polygon wheelPoly)
		{
			float y = (wheelPoly.Vertices.Max(a => a.Y) + wheelPoly.Vertices.Min(a => a.Y)) / 2;
			float z = (wheelPoly.Vertices.Max(a => a.Z) + wheelPoly.Vertices.Min(a => a.Z)) / 2;

			// X value is always the same
			return new Vector3(wheelPoly.Vertices[0].X, y, z);
		}

		public float RearWheelSize
		{
			get
			{
				return _leftRearWheel.Vertices.Max(a => a.Y) - _leftRearWheel.Vertices.Min(a => a.Y);
			}
		}

		public float FrontWheelSize
		{
			get
			{
				return _leftFrontWheel.Vertices.Max(a => a.Y) - _leftFrontWheel.Vertices.Min(a => a.Y);
			}
		}

		public Texture2D WheelTexture { get { return _wheelTexture; } }

		public override void Render(Effect effect)
		{
			Engine.Instance.Device.SetVertexBuffer(_vertexBuffer);

			effect.CurrentTechnique.Passes[0].Apply();

			foreach (Polygon poly in _polys)
			{
				if (poly == _rightFrontWheel || poly == _leftFrontWheel || poly == _leftRearWheel || poly == _rightRearWheel)
				{
					continue;
					Engine.Instance.Device.Textures[0] = _wheelTexture;
					
				}
				else
				{
					Engine.Instance.Device.Textures[0] = poly.Texture;
				}
				Engine.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, poly.VertexBufferIndex, poly.VertexCount / 3);
			}
		}
	}
}
