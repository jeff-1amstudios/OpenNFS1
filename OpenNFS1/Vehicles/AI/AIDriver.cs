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
	class AIDriver : IDriver
	{
		DrivableVehicle _vehicle;
		public DrivableVehicle Vehicle { get { return _vehicle; } }

		public AIDriver(VehicleDescription vehicleDescriptor)
		{
			_vehicle = new DrivableVehicle(vehicleDescriptor);
			_vehicle.SteeringSpeed = 10;
		}

		public void Update()
		{
			_vehicle.ThrottlePedalInput = 0.8f;
			_vehicle.SteeringInput = 0;
			_vehicle.BrakePedalInput = 0;

			var node = _vehicle.CurrentNode;
			var pos = _vehicle.Position;
			
			var closestPoint = Utility.GetClosestPointOnLine(node.Position, node.Next.Position, _vehicle.Position);

			float angle = Utility.GetSignedAngleBetweenVectors(_vehicle.Direction, node.Next.Position - node.Position, true);
			if (Math.Abs(angle) > 0.01f)
			{
				_vehicle.SteeringInput = -angle * 1.4f;

				if (Math.Abs(angle) > 0.5f)
				{
					_vehicle.ThrottlePedalInput = 0.4f;
					_vehicle.BrakePedalInput = 0.2f;
				}
				//_vehicle.ThrottlePedalInput = 0;
				//Debug.WriteLine("track_angle: " + angle);
				
			}

			// if we get too far off the racing line, bring us back quickly
			var distFromRoadCenter = Vector3.Distance(closestPoint, pos);
			if (distFromRoadCenter > 20)
			{
				angle = Utility.GetSignedAngleBetweenVectors(_vehicle.Direction, node.Next.Next.Position - pos, true);
				if (angle > 0) angle = 1f;
				if (angle < 0) angle = -1f;
				angle *= distFromRoadCenter * 0.0005f;
				_vehicle.SteeringInput = -angle;
				if (_vehicle.Speed > 60)
				{
					_vehicle.ThrottlePedalInput = 0.4f;
					_vehicle.BrakePedalInput = 0.2f;
				}
				GameConsole.WriteLine("dist_from_road: " + distFromRoadCenter + " , " + angle, 0);
			}

			if (Math.Abs(node.Orientation - node.Next.Next.Orientation) > 30 && _vehicle.ThrottlePedalInput == 0.8f)
			{
				//Debug.WriteLine("braking for upcoming corner");
				if (_vehicle.Speed > 100)
				{
					_vehicle.BrakePedalInput = 1;
				}
			}

			_vehicle.Update();
		}

		public void Render()
		{
			
		}
	}
}
