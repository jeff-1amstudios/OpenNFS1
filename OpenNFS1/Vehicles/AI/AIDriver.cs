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

		public const int MaxVirtualLanes = 4;

		public virtual Vector3 GetNextTarget()
		{
			return Vector3.Lerp(Vehicle.CurrentNode.Next.Next.GetLeftVerge2(), Vehicle.CurrentNode.Next.Next.GetRightVerge2(), (float)VirtualLane / (MaxVirtualLanes));
		}
				
		protected virtual void FollowTrack()
		{
			var target = GetNextTarget();
			if (GameConfig.DrawDebugInfo)
			{
				Engine.Instance.GraphicsUtils.AddCube(Matrix.CreateTranslation(target), Color.Yellow);
			}
			float angle = Utility.GetSignedAngleBetweenVectors(Vehicle.Direction, target - Vehicle.Position, true);
			if (angle < -0.1f)
			{
				Vehicle.SteeringInput = 0.5f;
			}
			else if (angle > 0.1f)
			{
				Vehicle.SteeringInput = -0.5f;
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
			_vehicle.SteeringSpeed = 6;
			_vehicle.AutoDrift = false;
			Vehicle = _vehicle;
			_firstLaneChangeAllowed = Engine.Instance.Random.Next(5, 40);
		}

		public override void Update(List<IDriver> otherDrivers)
		{
			_vehicle.ThrottlePedalInput = 0.9f;
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
			
			foreach (var otherDriver in otherDrivers)
			{
				if (otherDriver == this) continue;
				//if (!(otherDriver is AIDriver)) continue;
				// if I am not in the same lane, ignore
				if (otherDriver is AIDriver && ((AIDriver)otherDriver).VirtualLane != VirtualLane) continue;
				if (otherDriver is PlayerDriver) continue;  //ignore player for now
				
				// if I am going slower than the other driver, ignore
				if (Vehicle.Speed < otherDriver.Vehicle.Speed) continue;

				var progressDist = otherDriver.Vehicle.TrackPosition - _vehicle.TrackPosition;
				// if we are only slightly behind another driver (less than 2 nodes back) then consider them a possible danger
				if (progressDist > 0 && progressDist < 2f)
				{
					// pick a new lane if we're far enough from the start of the race.
					if (Vehicle.CurrentNode.Number > _firstLaneChangeAllowed)
					{
						if (Engine.Instance.Random.Next() % 2 == 0)
							VirtualLane = Math.Max(0, VirtualLane - 1);
						else
							VirtualLane = Math.Min(MaxVirtualLanes, VirtualLane + 1);
					}
					else
					{
						Vehicle.Speed = Math.Max(0, otherDriver.Vehicle.Speed * 0.8f);
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
