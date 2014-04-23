using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using NeedForSpeed.Parsers.Track;

namespace NeedForSpeed.Loaders
{
	interface ITrackLoader
	{
		long PhysicalRoadOffset { get; }
		long SceneryOffset { get; }
		long TerrainOffset { get; }
        

        int FenceTextureId { get; }

        void LoadTerrain(BinaryReader reader, List<TerrainSegment> terrainSegments, List<TerrainRow> terrainRows, List<PhysicalRoadNode> roadNodes);
	}
}
