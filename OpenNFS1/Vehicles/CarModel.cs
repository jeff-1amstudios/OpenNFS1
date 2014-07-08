using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Parsers;
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
		Polygon _leftBrakeLight, _rightBrakeLight;
		Texture2D _wheelTexture, _brakeOnTexture, _brakeOffTexture;
		
		static Color BrakeOffColor = new Color(88, 10, 5); //color of brake-off light color...
		static Color BrakeOnColor = new Color(158, 110, 6); //color of brake-off light color...

		public CarMesh(MeshChunk meshChunk, BitmapChunk bmpChunk, Color brakeColor)
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
					case "bkll":
						_leftBrakeLight = poly;
						break;
					case "bklr":
						_rightBrakeLight = poly;
						break;
				}
			}
			var tyreEntry = bmpChunk.FindByName("tyr1");
			if (tyreEntry != null)
				_wheelTexture = tyreEntry.Texture;

			// This seems like it should be done in a shader but I couldn't get it to work well enough 
			// (dealing with original paletted colors doesn't work so well in a texture stretched over a polygon)
			
			var rsidPoly = _polys.FirstOrDefault(a => a.TextureName == "rsid");
			if (rsidPoly != null)
			{
				// Generate a new texture for brake lights on.  
				Color[] pixels = new Color[rsidPoly.Texture.Width * rsidPoly.Texture.Height];
				rsidPoly.Texture.GetData<Color>(pixels);
				for (int i = 0; i < pixels.Length; i++)
				{
					if (pixels[i] == brakeColor)
						pixels[i] = BrakeOnColor;
				}

				_brakeOnTexture = new Texture2D(Engine.Instance.Device, rsidPoly.Texture.Width, rsidPoly.Texture.Height);
				_brakeOnTexture.SetData<Color>(pixels);

				// Generate a new texture for brake lights off.
				for (int i = 0; i < pixels.Length; i++)
				{
					if (pixels[i] == BrakeOnColor)
						pixels[i] = BrakeOffColor;
				}

				_brakeOffTexture = new Texture2D(Engine.Instance.Device, _leftBrakeLight.Texture.Width, _leftBrakeLight.Texture.Height);
				_brakeOffTexture.SetData<Color>(pixels);
			}
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

		public void Render(Effect effect, bool enableBrakeLights)
		{
			Engine.Instance.Device.SetVertexBuffer(_vertexBuffer);

			effect.CurrentTechnique.Passes[0].Apply();

			foreach (Polygon poly in _polys)
			{
				if (poly == _rightFrontWheel || poly == _leftFrontWheel || poly == _leftRearWheel || poly == _rightRearWheel)
				{
					continue;
				}
				else if (_brakeOnTexture != null && poly.TextureName == "rsid")
				{
					Engine.Instance.Device.Textures[0] = enableBrakeLights ? _brakeOnTexture : _brakeOffTexture;
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
