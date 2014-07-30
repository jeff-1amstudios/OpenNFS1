using Microsoft.Xna.Framework;
using OpenNFS1;
using NfsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNFS1.Tracks
{
	class TrackNodeProperty
	{
		// Unimplemented:
		public const byte LANE_SPLIT = 0;  // Used in Alpine, Coastal tracks when the road widens. vertices should be moved to the right. 
		public const byte LANE_REJOIN = 2;  //Used in Alpine, Coastal tracks when the road narrows. vertices should be moved to the right
		public const byte TUNNEL = 4;  //plays tunnel sound
		public const byte COBBLED_ROAD = 5;  //plays different road noise (Last Vegas)
		public const byte WATERFALL_AUDIO_LEFT_CHANNEL = 14;  //plays waterfall audio in left channel
		public const byte WATERFALL_AUDIO_RIGHT_CHANNEL = 15;  //plays waterfall audio in right channel
		public const byte WATER_AUDIO_LEFT_CHANNEL = 18;  //plays water sound
		public const byte WATER_AUDIO_RIGHT_CHANNEL = 18;  //plays water sound

		// Implemented:
		// The next 3 properties are interesting!  If set, either the last terrain strip on the right or left is detached and 
		// placed between the specified points.  This is how the tunnels in Vertigo Ridge and Coastal (and Alpine 3) are constructed.
		// For the meaning of A2,A9 etc, refer to /NFSSpecs.txt
		public const byte RIGHT_TUNNEL_A2_A9 = 7;
		public const byte LEFT_TUNNEL_A9_A4 = 12;
		public const byte LEFT_TUNNEL_A9_A5 = 13;
	}
	
	class TrackNode
	{
		public int Number;
		public float DistanceToLeftVerge, DistanceToLeftBarrier;
		public float DistanceToRightVerge, DistanceToRightBarrier;
		public byte Flag1;
		public byte Flag2;
		public byte Flag3;
		public byte NodeProperty;
		public Vector3 Position;
		public float Slope;
		public Int32 Slant, ZOrientation, XOrientation;
		public float Orientation;
		public Vector3 Up;
		public byte[] unk1, unk2;
		public TrackNode Next, Prev;

		const float SlantMultiplier = 1.028f;
		const float RoadPadding = 7;

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

		public Vector3 GetLeftVerge2()
		{
			return GetLeftOffset(DistanceToLeftVerge - RoadPadding);
		}

		public Vector3 GetRightVerge2()
		{
			return GetRightOffset(DistanceToRightVerge - RoadPadding);
		}

		public Vector3 GetMiddlePoint()
		{
			return (GetLeftVerge() + GetRightVerge()) / 2;
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