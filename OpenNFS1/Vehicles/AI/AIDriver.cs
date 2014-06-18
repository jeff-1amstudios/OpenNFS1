using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NfsEngine;
using OpenNFS1.Parsers.Track;
using OpenNFS1.Physics;

namespace OpenNFS1.Vehicles.AI
{
	enum AIDriverState
	{
		Default,
		TooFarFromCenter
	}

	class AIDriver : IDriver
	{
		DrivableVehicle _vehicle;
		AIDriverState _state = AIDriverState.Default;
		public DrivableVehicle Vehicle { get { return _vehicle; } }

		public AIDriver(VehicleDescription vehicleDescriptor)
		{
			_vehicle = new DrivableVehicle(vehicleDescriptor);
			_vehicle.SteeringSpeed = 10;
		}

		public void Update(List<IDriver> otherDrivers)
		{
			_vehicle.ThrottlePedalInput = 1f;
			_vehicle.SteeringInput = 0;
			_vehicle.BrakePedalInput = 0;

			var node = _vehicle.CurrentNode;
			var pos = _vehicle.Position;
			
			var closestPoint = Utility.GetClosestPointOnLine(node.Position, node.Next.Position, _vehicle.Position);			

			// if we get too far off the racing line, bring us back quickly
			var distFromRoadCenter = Vector3.Distance(closestPoint, pos);
			if (distFromRoadCenter > 30)
			{
				//GameConsole.WriteLine("off_road", 4);
				float angle = Utility.GetSignedAngleBetweenVectors(_vehicle.Direction, node.Next.Next.Position - pos, true);
				if (angle < 0)
					_vehicle.SteeringInput = 0.3f;
				else if (angle > 0)
					_vehicle.SteeringInput = -0.3f;
				
				//_vehicle.SteeringInput *= 2;
				if (_vehicle.Speed > 100 && Math.Abs(angle) > 0.6f)
				{
					_vehicle.ThrottlePedalInput = 0.0f;
					_vehicle.BrakePedalInput = 0.8f;
				}
			}
			else
			{
				//GameConsole.WriteLine("NOT off_road", 4);

				float angle = Utility.GetSignedAngleBetweenVectors(_vehicle.Direction, node.Next.Position - node.Position, true);
				if (Math.Abs(angle) > 0.01f)
				{
					_vehicle.SteeringInput = -angle * 1.6f;
				}
			}
			

			if (Math.Abs(node.Orientation - node.Next.Next.Orientation) > 30 && _vehicle.ThrottlePedalInput == 0.8f)
			{
				//Debug.WriteLine("braking for upcoming corner");
				//GameConsole.WriteLine("braking for corner", 3);
				if (_vehicle.Speed > 100)
				{
					_vehicle.BrakePedalInput = 1;
				}
			}

			foreach (var driver in otherDrivers)
			{
				if (driver == this) continue;
				var progressDist = driver.Vehicle.TrackProgress - _vehicle.TrackProgress;
				// if we are slightly behind another driver (less than 2 nodes back) then consider them a possible danger
				if (progressDist > 0 && progressDist < 2f)
				{
					float positionDist = Vector3.Distance(driver.Vehicle.Position, _vehicle.Position);
					if (positionDist < 20)
					{
						_vehicle.Speed = driver.Vehicle.Speed * 0.8f;
					}
					if (positionDist < 50)
					{
						//_vehicle.SteeringInput = -0.2f;
						break;
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
