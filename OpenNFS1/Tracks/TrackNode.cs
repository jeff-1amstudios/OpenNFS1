using Microsoft.Xna.Framework;
using OpenNFS1;
using NfsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNFS1.Tracks
{
	class TrackNode
	{
		public int Number;
		public float DistanceToLeftVerge, DistanceToLeftBarrier;
		public float DistanceToRightVerge, DistanceToRightBarrier;
		public byte[] b;
		public Vector3 Position;
		public float Slope;
		public Int32 Slant, ZOrientation, XOrientation;
		public float Orientation;
		public Vector3 Up;
		public byte[] unk1, unk2;
		public TrackNode Next, Prev;

		const float SlantMultiplier = 1.028f;

		public Vector3 GetLeftBoundary()
		{
			return GetLeftOffset(DistanceToLeftBarrier);
		}
		public Vector3 GetLeftVerge()
		{
			return GetLeftOffset(DistanceToLeftVerge);
		}
		public Vector3 GetRightBoundary()
		{
			return GetRightOffset(DistanceToRightBarrier);
		}
		public Vector3 GetRightVerge()
		{
			return GetRightOffset(DistanceToRightVerge);
		}


		private Vector3 GetLeftOffset(float offset)
		{
			Vector3 position = Position + Utility.RotatePoint(new Vector2(-offset, 0), -Orientation);

			float slantAngle = (((float)Slant / short.MaxValue));

			var pos3 = Vector3.Transform(new Vector3(-offset, 0, 0), Matrix.CreateRotationZ(slantAngle * SlantMultiplier) * Matrix.CreateRotationY(MathHelper.ToRadians(Orientation)));
			return Position + pos3;
		}

		private Vector3 GetRightOffset(float offset)
		{
			float slantAngle = (((float)Slant / short.MaxValue)); // 32000f));
			var pos3 = Vector3.Transform(new Vector3(offset, 0, 0), Matrix.CreateRotationZ(slantAngle * SlantMultiplier) * Matrix.CreateRotationY(MathHelper.ToRadians(Orientation)));
			return Position + pos3;
		}
	}
}