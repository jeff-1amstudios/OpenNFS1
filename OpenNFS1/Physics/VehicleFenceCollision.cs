using System;
using Microsoft.Xna.Framework;
using NfsEngine;
using OpenNFS1.Audio;

namespace OpenNFS1.Physics
{
    
    class VehicleFenceCollision
    {
        public static void Handle(DrivableVehicle car)
        {
			var leftBound1 = car.CurrentNode.GetLeftBoundary();
			var leftBound2 = car.CurrentNode.Next.GetLeftBoundary();

			var rightBound1 = car.CurrentNode.GetRightBoundary();
			var rightBound2 = car.CurrentNode.Next.GetRightBoundary();
            

			for (int wheelNumber = 0; wheelNumber < 4; wheelNumber++)
			{
				VehicleWheel wheel = car.Wheels[wheelNumber];
				Vector3 fenceNormal = Vector3.Zero;
				bool collision = false;
				float collisionAngle = 0;
				float direction = 0;
				if (Utility.IsLeftOfLine(leftBound2, leftBound1, wheel.WorldPosition))
				{
					fenceNormal = Vector3.Cross(Vector3.Normalize(leftBound2 - leftBound1), car.CurrentNode.Up);
					collisionAngle = Vector3Helper.GetAngleBetweenVectors(car.Right, fenceNormal);
					collision = true;
					direction = -1;
				}
				else if (!Utility.IsLeftOfLine(rightBound2, rightBound1, wheel.WorldPosition))
				{
					fenceNormal = Vector3.Cross(car.CurrentNode.Up, Vector3.Normalize(rightBound2 - rightBound1));
					collisionAngle = Vector3Helper.GetAngleBetweenVectors(-car.Right, fenceNormal);
					collision = true;
					direction = 1;
				}

				if (!collision) continue;

				Vector3 collisionDir = Vector3.Reflect(car.Direction, fenceNormal);
				// Force car back on the road, for that calculate impulse and collision direction
				
				// Flip at 180 degrees (if driving in wrong direction)
				if (collisionAngle > MathHelper.Pi / 2)
					collisionAngle -= MathHelper.Pi;

				// Just correct rotation if we hit the fence at a shallow angle
				if (Math.Abs(collisionAngle) < MathHelper.ToRadians(45))
				{
					SlideAlongFence(car, wheelNumber, collisionAngle, direction);
				}

				// If 90-45 degrees (in either direction), make frontal crash
				// + stop car + wobble camera
				else if (Math.Abs(collisionAngle) < MathHelper.Pi * 3.0f / 4.0f)
				{
					HandleHeadOnCrash(car, wheelNumber, collisionAngle);
				}

				// move away from the way slightly.  We should be a bit smarter and along more of a slide along the wall..
				car.Position += fenceNormal * 1.5f;
				break;
			}
        }

        public static int GetWheelsOutsideRoadVerge(DrivableVehicle car)
        {
			var leftVerge1 = car.CurrentNode.GetLeftVerge();
			var leftVerge2 = car.CurrentNode.Next.GetLeftVerge();

			var rightVerge1 = car.CurrentNode.GetRightVerge();
			var rightVerge2 = car.CurrentNode.Next.GetRightVerge();
		
            int wheelsOutsideVerge = 0;

            for (int wheelNumber = 0; wheelNumber < 4; wheelNumber++)
            {
                VehicleWheel wheel = car.Wheels[wheelNumber];

				if (Utility.IsLeftOfLine(leftVerge2, leftVerge1, wheel.WorldPosition))
				{
					wheelsOutsideVerge++;
				}
				else if (!Utility.IsLeftOfLine(rightVerge2, rightVerge1, wheel.WorldPosition))
				{
					wheelsOutsideVerge++;
				}
            }

            return wheelsOutsideVerge;
        }
    

        private static void SlideAlongFence(DrivableVehicle car, int wheel, float collisionAngle, float direction)
        {
			if (car.AudioEnabled && Math.Abs(collisionAngle) > MathHelper.ToRadians(30))
			{
				EnvironmentAudioProvider.Instance.PlayVehicleFenceCollision();
			}

            // if we hit the front wheels, change the direction to almost match the fence direction.  If we hit the back wheels this looks weird so
			// only rotate 50% of the way
            if (wheel < 2)
            {
				car.RotateCarAfterCollision = direction * collisionAngle * 1.1f;
                car.Speed *= 0.80f;
            }
            else
            {
                car.RotateCarAfterCollision = direction * collisionAngle *0.5f;
                car.Speed *= 0.9f;
            }
        }

        private static void HandleHeadOnCrash(DrivableVehicle car, int wheel, float collisionAngle)
        {
			if (car.AudioEnabled)
			{
				EnvironmentAudioProvider.Instance.PlayVehicleFenceCollision();
			}


            // Also rotate car if less than 60 degrees
            if (Math.Abs(collisionAngle) < MathHelper.Pi / 3.0f)
                car.RotateCarAfterCollision = +collisionAngle / 3.0f;

			//bounce back a little
			car.Speed *= -0.15f;
        }
    }
}
