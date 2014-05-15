using System;
using System.Collections.Generic;
using System.Text;
using NeedForSpeed.Physics;
using Microsoft.Xna.Framework;
using NeedForSpeed.Parsers;
using NfsEngine;
using NeedForSpeed.Dashboards;
using NfsEngine;

namespace NeedForSpeed.Vehicles
{
    class TrafficVehicle
    {
        private CfmFile _model;
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public int LastNode { get; set; } 
        public int NextNode { get; set; }
        public float DistanceBetweenNodes { get; set; }
        public int TravelDirection { get; set; }
        public int DistanceFromPlayer { get; set; }

        static List<string> _trafficModels;

        static TrafficVehicle()
        {
            _trafficModels = new List<string>();
            _trafficModels.Add(@"SIMDATA\CARFAMS\axxess.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\vandura.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\bmw.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\copmust.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\crx.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\jeep.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\jetta.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\lemans.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\pickup.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\probe.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\rodeo.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\sunbird.CFM");
            _trafficModels.Add(@"SIMDATA\CARFAMS\wagon.CFM");
        }

        public TrafficVehicle(int startNode, int travelDirection)
        {
            _model = CarModelCache.GetCfm(_trafficModels[Utility.RandomGenerator.Next(12)]);

            LastNode = startNode;
            NextNode = startNode + travelDirection;
            TravelDirection = travelDirection;
        }


        public void Render()
        {
            float turnAround = TravelDirection == -1 ? MathHelper.Pi : 0;
            Matrix matrix =
                Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(Direction.Y) + turnAround, Direction.X * TravelDirection, Direction.Z * TravelDirection) *
                Matrix.CreateTranslation(Position);

            if (DistanceFromPlayer < 20)
            {
                Vector3[] points = new Vector3[4];

                points[1] = new Vector3(-12, 0, 35);
                points[0] = new Vector3(12, 0, 35);
                points[3] = new Vector3(-12, 0, -35);
                points[2] = new Vector3(12, 0, -35);
                ObjectShadow.Render(points, matrix);
            }
            
            //_model.Render(Matrix.CreateScale(0.09f) * matrix, false);
        }
    }
}