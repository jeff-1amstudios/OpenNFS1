using System;
using System.Collections.Generic;

using System.Text;
using NeedForSpeed.Physics;

namespace NeedForSpeed
{
    static class GameConfig
    {
        public static Vehicle SelectedVehicle;
        public static TrackDescription SelectedTrack;
        public static bool Render2dScenery = true, Render3dScenery = true;
        public static bool RenderOnlyPhysicalTrack = true;
        public static int DrawDistance { get; set; }
        public static bool ManualGearbox { get; set; }

        static GameConfig()
        {
            DrawDistance = 30;
        }
    }
}
