using Microsoft.Xna.Framework;
using NeedForSpeed;
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
		public TrackNode Next;

		static float SlopeMultiplier = 0.440f;

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

			if (Math.Abs(Slant) > 0)
			{
				float slantScale = SlopeMultiplier * GameConfig.ScaleFactor;
				if (Math.Abs(Slant) > 0)
				{
					position.Y -= Slant * offset * slantScale;
				}
			}
			return position;
		}

		private Vector3 GetRightOffset(float offset)
		{
			Vector3 position = Position + Utility.RotatePoint(new Vector2(offset, 0), -Orientation);

			if (Math.Abs(Slant) > 0)
			{
				float slantScale = SlopeMultiplier * GameConfig.ScaleFactor;
				position.Y += Slant * offset * slantScale;
			}
			return position;
		}
	}
}