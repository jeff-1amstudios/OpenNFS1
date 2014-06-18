using System;
using System.Collections.Generic;

using System.Text;
using OpenNFS1.Physics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using OpenNFS1.Vehicles;
using OpenNFS1.Parsers.Track;

namespace OpenNFS1
{
    static class GameConfig
    {
        public static DrivableVehicle SelectedVehicle;
        public static TrackDescription SelectedTrackDescription;
		public static Track CurrentTrack;

		public static string CdDataPath { get; set; }
        public static bool Render2dScenery = true, Render3dScenery = true;
        public static bool RenderOnlyPhysicalTrack = true;
        public static int DrawDistance { get; set; }
        public static bool ManualGearbox { get; set; }
		public static bool RespectOpenRoadCheckpoints { get; set; }
		public static bool DrawDebugInfo { get; set; }
		public static int ScreenWidth { get; set; }

		public const float MeshScale = 0.040f;
		public const float TerrainScale = 0.000080f;
		public static float FOV = MathHelper.ToRadians(65);

        static GameConfig()
        {
			
        }

		public static void Load()
		{
			JObject o1 = JObject.Parse(File.ReadAllText(@"gameconfig.json"));
			CdDataPath = o1.Value<string>("cdDataPath");
			DrawDistance = o1.Value<int>("drawDistance");
			RespectOpenRoadCheckpoints = o1.Value<bool>("respectOpenRoadCheckpoints");
			DrawDebugInfo = o1.Value<bool>("drawDebugInfo");
		}
    }
}
