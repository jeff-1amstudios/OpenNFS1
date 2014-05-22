using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OpenNFS1.Parsers;

namespace OpenNFS1.Tracks
{
	// A terrain row maps to a TrackNode.  TerrainRow defines the vertex positions for that bit of track, the TrackNode defines the physical properties
	// that the vehicles follow (slope, slant, width...)
	class TerrainRow
	{

		const int TerrainPositionScale = 500;
		public Vector3 MiddlePoint;
		public Vector3[] LeftPoints = new Vector3[TriFile.NbrTerrainPointsPerSide];
		public Vector3[] RightPoints = new Vector3[TriFile.NbrTerrainPointsPerSide];

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
}
