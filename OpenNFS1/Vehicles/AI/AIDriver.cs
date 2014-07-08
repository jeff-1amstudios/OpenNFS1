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

	abstract class AIDriver : IDriver
	{
		public int VirtualLane { get; set; }
		public Vehicle Vehicle { get; protected set; }

		protected const int MaxVirtualLanes = 4;

		public virtual Vector3 GetNextTarget()
		{
			return Vector3.Lerp(Vehicle.CurrentNode.Next.Next.GetLeftVerge2(), Vehicle.CurrentNode.Next.Next.GetRightVerge2(), VirtualLane * (1f / MaxVirtualLanes));
		}
				
		protected void FollowTrack()
		{
			float angle = Utility.GetSignedAngleBetweenVectors(Vehicle.Direction, GetNextTarget() - Vehicle.Position, true);
			if (angle < -0.1f)
			{
				Vehicle.SteeringInput = 0.5f;
				GameConsole.WriteLine("turning right", 5);
			}
			else if (angle > 0.1f)
			{
				Vehicle.SteeringInput = -0.5f;
				GameConsole.WriteLine("turning left", 5);
			}
			else
			{
				Vehicle.SteeringInput = 0;
			}
		}

		public abstract void Update(List<IDriver> otherDrivers);

	}

	class RacingAIDriver : AIDriver
	{		
		private float _firstLaneChangeAllowed;  //avoid all racers changing lanes immediately
		private DrivableVehicle _vehicle;

		public RacingAIDriver(VehicleDescription vehicleDescriptor)
		{
			_vehicle = new DrivableVehicle(vehicleDescriptor);
			_vehicle.SteeringSpeed = 7;
			Vehicle = _vehicle;
			_firstLaneChangeAllowed = Engine.Instance.Random.Next(5, 40);
		}

		public override void Update(List<IDriver> otherDrivers)
		{
			_vehicle.ThrottlePedalInput = 0.7f;
			_vehicle.BrakePedalInput = 0;

			var node = _vehicle.CurrentNode;
			var pos = _vehicle.Position;

			if (node.Next == null || node.Next.Next == null)
			{
				_vehicle.ThrottlePedalInput = 0;
				_vehicle.BrakePedalInput = 1;
				return;
			}

			FollowTrack();
			
			foreach (var driver in otherDrivers)
			{
				if (driver == this) continue;
				if (!(driver is AIDriver)) continue;
				// if I am not in the same lane, ignore
				if (driver is AIDriver && ((AIDriver)driver).VirtualLane != VirtualLane) continue;
				// if I am going slower than the other driver, ignore
				if (Vehicle.Speed < driver.Vehicle.Speed) continue;

				var progressDist = driver.Vehicle.TrackProgress - _vehicle.TrackProgress;
				// if we are only slightly behind another driver (less than 2 nodes back) then consider them a possible danger
				if (progressDist > 0 && progressDist < 2f)
				{
					// pick a new lane
					if (Vehicle.CurrentNode.Number > _firstLaneChangeAllowed)
					{
						VirtualLane = Engine.Instance.Random.Next(Math.Max(0, VirtualLane - 1), Math.Min(MaxVirtualLanes - 1, VirtualLane + 1) + 1);
					}
				}
			}

			_vehicle.Update();
		}		

		public void Render()
		{
			
		}
	}
}
