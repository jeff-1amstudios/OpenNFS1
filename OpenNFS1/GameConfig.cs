using System;
using System.Collections.Generic;

using System.Text;
using NeedForSpeed.Physics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace NeedForSpeed
{
    static class GameConfig
    {
        public static Vehicle SelectedVehicle;
        public static TrackDescription SelectedTrack;

		public static string CdDataPath { get; set; }
        public static bool Render2dScenery = true, Render3dScenery = true;
        public static bool RenderOnlyPhysicalTrack = true;
        public static int DrawDistance { get; set; }
        public static bool ManualGearbox { get; set; }
		public static bool RespectOpenRoadCheckpoints { get; set; }
		public static float ScaleFactor = 0.000080f;

        static GameConfig()
        {
			
        }

		public static void Load()
		{
			JObject o1 = JObject.Parse(File.ReadAllText(@"gameconfig.json"));
			CdDataPath = o1.Value<string>("cdDataPath");
			DrawDistance = o1.Value<int>("drawDistance");
			RespectOpenRoadCheckpoints = o1.Value<bool>("respectOpenRoadCheckpoints");
		}
    }
}
