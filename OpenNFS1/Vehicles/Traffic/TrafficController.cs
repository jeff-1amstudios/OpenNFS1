using System;
using System.Collections.Generic;
using System.Text;
using NeedForSpeed.Parsers.Track;
using Microsoft.Xna.Framework;
using NfsEngine;
using NeedForSpeed.Physics;

namespace NeedForSpeed.Vehicles
{
    class TrafficController
    {
        Track _track;
        Vehicle _player;
        List<TrafficVehicle> _traffic = new List<TrafficVehicle>();
        public bool Enabled { get; set; }

        public TrafficController(Track track, Vehicle player)
        {
            _track = track;
            _player = player;
            Enabled = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!Enabled) return;

            for (int i = _traffic.Count - 1; i >= 0; i--)
            {
                TrafficVehicle vehicle = _traffic[i];
                if (vehicle.LastNode < _player.CurrentTrackNode - 10 || vehicle.LastNode > _player.CurrentTrackNode + 140)
                {
                    _traffic.Remove(vehicle);
                    continue;
                }

                if (vehicle.DistanceBetweenNodes >= 1)
                {
                    vehicle.DistanceBetweenNodes = vehicle.DistanceBetweenNodes - 1;
                    vehicle.LastNode = vehicle.NextNode;
                    vehicle.NextNode += vehicle.TravelDirection;
                    vehicle.DistanceFromPlayer = Math.Abs(vehicle.LastNode - _player.CurrentTrackNode);
                    if (vehicle.NextNode >= _track.RoadNodes.Count || vehicle.NextNode < 0)
                    {
                        _traffic.Remove(vehicle);
                        continue;
                    }
                }

                Vector3 direction = new Vector3();
                PhysicalRoadNode lastNode = _track.RoadNodes[vehicle.LastNode];
                PhysicalRoadNode nextNode = _track.RoadNodes[vehicle.NextNode];

                if (Math.Abs(lastNode.Orientation - nextNode.Orientation) > 100)
                {
                    direction.Y = lastNode.Orientation;
                }
                else
                    direction.Y = MathHelper.Lerp(lastNode.Orientation, nextNode.Orientation, vehicle.DistanceBetweenNodes);

                direction.X = MathHelper.Lerp(lastNode.Slope, nextNode.Slope, vehicle.DistanceBetweenNodes) * 0.00036f;
                direction.Z = MathHelper.Lerp(lastNode.Slant, nextNode.Slant, vehicle.DistanceBetweenNodes) * 0.00003f;
                vehicle.Direction = direction;

                float laneOffset = vehicle.TravelDirection == -1 ? MathHelper.Lerp(lastNode.DistanceToLeftVerge, nextNode.DistanceToLeftVerge, vehicle.DistanceBetweenNodes)
                    : MathHelper.Lerp(lastNode.DistanceToRightVerge, nextNode.DistanceToRightVerge, vehicle.DistanceBetweenNodes);
                laneOffset -= 30;
                //laneOffset = 25;
                Vector3 offset = Utility.RotatePoint(new Vector2(vehicle.TravelDirection * laneOffset, 0), -_track.RoadNodes[vehicle.LastNode].Orientation);
                vehicle.Position = Vector3.Lerp(_track.RoadNodes[vehicle.LastNode].Position, nextNode.Position, vehicle.DistanceBetweenNodes) + offset;
                vehicle.DistanceBetweenNodes += (float)gameTime.ElapsedGameTime.TotalSeconds * 5f;
            }

            while (_traffic.Count < 5)
            {
                int direction = Utility.RandomGenerator.Next() % 2 == 0 ? 1 : -1;
                int node = Utility.RandomGenerator.Next(40, 120);
                TrafficVehicle vehicle = new TrafficVehicle((_player.CurrentTrackNode + node) % _track.RoadNodes.Count-1, direction);
                _traffic.Add(vehicle);
            }
        }

        public void Render()
        {
            if (!Enabled) return;

            foreach (TrafficVehicle t in _traffic)
            {
                if (t.Position != Vector3.Zero && t.DistanceFromPlayer < GameConfig.DrawDistance * 3)
                    t.Render();
            }
        }
    }
}
