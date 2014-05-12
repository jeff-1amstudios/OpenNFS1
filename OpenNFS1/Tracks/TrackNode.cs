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
		public float DistanceToLeftVerge, DistanceToLeftBarrier;
		public float DistanceToRightVerge, DistanceToRightBarrier;
		public byte[] b;
		public Vector3 Position;
		public float Slope;
		public Int32 Slant, ZOrientation, XOrientation;
		public float Orientation;
		public Vector3 Up;
		public byte[] unk1, unk2;

		static float SlopeMultiplier = 0.45f;

		public Vector3 GetLeftBoundary()
		{
			Vector3 position = Position + Utility.RotatePoint(new Vector2(-DistanceToLeftBarrier, 0), -Orientation);

			if (Math.Abs(Slant) > 0)
			{
				float slantScale = SlopeMultiplier * GameConfig.ScaleFactor;
				if (Math.Abs(Slant) > 0)
				{
					position.Y -= Slant * DistanceToLeftBarrier * slantScale;
				}
			}
			return position;
		}

		public Vector3 GetRightBoundary()
		{
			Vector3 position = Position + Utility.RotatePoint(new Vector2(DistanceToRightBarrier, 0), -Orientation);

			if (Math.Abs(Slant) > 0)
			{
				float slantScale = SlopeMultiplier * GameConfig.ScaleFactor;
				position.Y += Slant * DistanceToRightBarrier * slantScale;
			}
			return position;
		}
	}
}