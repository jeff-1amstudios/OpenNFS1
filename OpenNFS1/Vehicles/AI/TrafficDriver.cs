using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NfsEngine;
using OpenNFS1.Parsers.Track;
using OpenNFS1.Physics;
using OpenNFS1.Tracks;

namespace OpenNFS1.Vehicles.AI
{
	enum TrafficDriverDirection 
	{
		Forward,
		Backward
	}

	class TrafficDriver : AIDriver
	{
		TrafficDriverDirection _direction;

		public TrafficDriver(string cfmFile, TrafficDriverDirection direction)
		{
			Vehicle = new Vehicle(cfmFile);
			Vehicle.SteeringSpeed = 7;
			_direction = direction;
			if (direction == TrafficDriverDirection.Forward)
			{
				VirtualLane = 4;
			}
			else
			{
				VirtualLane = 0;
			}
		}

		public override Vector3 GetNextTarget()
		{
			if (_direction == TrafficDriverDirection.Forward)
				return Vector3.Lerp(Vehicle.CurrentNode.Next.Next.GetLeftVerge2(), Vehicle.CurrentNode.Next.Next.GetRightVerge2(), VirtualLane * (1f / MaxVirtualLanes));
			else
				return Vector3.Lerp(Vehicle.CurrentNode.Prev.Prev.GetLeftVerge2(), Vehicle.CurrentNode.Prev.Prev.GetRightVerge2(), VirtualLane * (1f / MaxVirtualLanes));
		}

		public override void Update(List<IDriver> otherDrivers)
		{
			Vehicle.Speed = 50;

			var node = Vehicle.CurrentNode;
			
			if (node.Next == null || node.Next.Next == null || node.Prev == null || node.Prev.Prev == null)
			{
				return;
			}

			FollowTrack();

			foreach (var otherDriver in otherDrivers)
			{
				if (otherDriver == this) continue;
				//if (!(otherDriver is AIDriver)) continue;
				// if I am not in the same lane, ignore
				if (otherDriver is AIDriver && ((AIDriver)otherDriver).VirtualLane != VirtualLane) continue;
				// if I am going slower than the other driver, ignore
				if (Vehicle.Speed < otherDriver.Vehicle.Speed) continue;

				var progressDist = otherDriver.Vehicle.TrackProgress - Vehicle.TrackProgress;
				// if we are only slightly behind another driver (less than 2 nodes back) then consider them a possible danger
				if (progressDist > 0 && progressDist < 2f)
				{
					// slow down immediately and pick a new lane
					Vehicle.Speed = Math.Max(0, otherDriver.Vehicle.Speed * 0.8f);
				}
			}

			Vehicle.Update();
		}

	}
}
