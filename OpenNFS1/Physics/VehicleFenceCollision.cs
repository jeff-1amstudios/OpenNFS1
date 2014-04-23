using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NeedForSpeed.Parsers.Track;
using NfsEngine;
using System.Diagnostics;
using NfsEngine;
using Microsoft.Xna.Framework.Audio;
using NeedForSpeed.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace NeedForSpeed.Physics
{
    
    class VehicleFenceCollision
    {
        public static void Handle(Vehicle car, float moveFactor, TerrainSegment segment, int segmentRow)
        {
            Line leftFence = segment.LeftBoundary[segmentRow < 2 ? 0 : 1];
            Line rightFence = segment.RightBoundary[segmentRow < 2 ? 0 : 1];

            for (int wheelNumber = 0; wheelNumber < 4; wheelNumber++)
            {
                VehicleWheel wheel = car.Wheels[wheelNumber];

                // Hit any guardrail?
                float distanceFromLeftFence = Vector3Helper.DistanceToLine(wheel.WorldPosition, leftFence.Point1, leftFence.Point2);
                float distanceFromRightFence = Vector3Helper.DistanceToLine(wheel.WorldPosition, rightFence.Point1, rightFence.Point2);

                Vector3 offsetPos = wheel.WorldPosition + (-rightFence.Normal * 0.5f);
                float offsetDistance = Vector3Helper.DistanceToLine(offsetPos, rightFence.Point1, rightFence.Point2);
                bool outsideRightFence = (offsetDistance > distanceFromRightFence);

                offsetPos = wheel.WorldPosition + (-leftFence.Normal * 0.5f);
                offsetDistance = Vector3Helper.DistanceToLine(offsetPos, leftFence.Point1, leftFence.Point2);
                bool outsideLeftFence = (offsetDistance > distanceFromLeftFence);

                if (outsideLeftFence)
                {
                    // Force car back on the road, for that calculate impulse and collision direction
                    Vector3 collisionDir = Vector3.Reflect(car.Direction, leftFence.Normal);

                    float collisionAngle = Vector3Helper.GetAngleBetweenVectors(car.CarRight, leftFence.Normal);
                    // Flip at 180 degrees (if driving in wrong direction)
                    if (collisionAngle > MathHelper.Pi / 2)
                        collisionAngle -= MathHelper.Pi;

                    // Just correct rotation if 0-45 degrees (slowly)
                    if (Math.Abs(collisionAngle) < MathHelper.Pi / 4.0f)
                    {
                        Handle45DegreeCrash(car, wheelNumber, collisionAngle, -1);
                    }

                    // If 90-45 degrees (in either direction), make frontal crash
                    // + stop car + wobble camera
                    else if (Math.Abs(collisionAngle) < MathHelper.Pi * 3.0f / 4.0f)
                    {
                        HandleHeadOnCrash(car, wheelNumber, collisionAngle);
                    }

                    // For all collisions, kill the current car force
                    car.Force = Vector3.Zero;

                    float speedDistanceToGuardrails = car.Speed * Math.Abs(Vector3.Dot(car.Direction, leftFence.Normal));

                    if (distanceFromLeftFence > 0)
                    {
                        car.Position += leftFence.Normal * 1.1f;
                    }
                }

                else if (outsideRightFence)
                {
                    // Force car back on the road, for that calculate impulse and collision direction
                    Vector3 collisionDir = Vector3.Reflect(car.Direction, rightFence.Normal);

                    float collisionAngle = Vector3Helper.GetAngleBetweenVectors(-car.CarRight, rightFence.Normal);
                    // Flip at 180 degrees (if driving in wrong direction)
                    if (collisionAngle > MathHelper.Pi / 2)
                        collisionAngle -= MathHelper.Pi;
                    // Just correct rotation if 0-45 degrees (slowly)
                    if (Math.Abs(collisionAngle) < MathHelper.Pi / 4.0f)
                    {
                        Handle45DegreeCrash(car, wheelNumber, collisionAngle, 1);
                    }

                    // If 90-45 degrees (in either direction), make frontal crash
                    // + stop car + wobble camera
                    else if (Math.Abs(collisionAngle) < MathHelper.Pi * 3.0f / 4.0f)
                    {
                        HandleHeadOnCrash(car, wheelNumber, collisionAngle);
                    }

                    // For all collisions, kill the current car force
                    car.Force = Vector3.Zero;

                    float speedDistanceToGuardrails = car.Speed * Math.Abs(Vector3.Dot(car.Direction, rightFence.Normal));

                    if (distanceFromRightFence > 0)
                    {
                        car.Position += rightFence.Normal * 1.1f;
                    }
                }
            }
        }

        public static int GetWheelsOutsideRoadVerge(Vehicle car, TerrainSegment segment, int segmentRow)
        {
            Line leftVerge = segment.LeftVerge[segmentRow < 2 ? 0 : 1];
            Line rightVerge = segment.RightVerge[segmentRow < 2 ? 0 : 1];

            int wheelsOutsideVerge = 0;

            for (int wheelNumber = 0; wheelNumber < 4; wheelNumber++)
            {
                VehicleWheel wheel = car.Wheels[wheelNumber];

                float distanceFromLeftVerge = Vector3Helper.DistanceToLine(wheel.WorldPosition, leftVerge.Point1, leftVerge.Point2);
                float distanceFromRightVerge = Vector3Helper.DistanceToLine(wheel.WorldPosition, rightVerge.Point1, rightVerge.Point2);

                Vector3 offsetPos = wheel.WorldPosition + (-rightVerge.Normal * 0.5f);
                float offsetDistance = Vector3Helper.DistanceToLine(offsetPos, rightVerge.Point1, rightVerge.Point2);
                bool outsideRightFence = (offsetDistance > distanceFromRightVerge);

                if (outsideRightFence)
                    wheelsOutsideVerge++;

                offsetPos = wheel.WorldPosition + (-leftVerge.Normal * 0.5f);
                offsetDistance = Vector3Helper.DistanceToLine(offsetPos, leftVerge.Point1, leftVerge.Point2);
                bool outsideLeftFence = (offsetDistance > distanceFromLeftVerge);

                if (outsideLeftFence)
                    wheelsOutsideVerge++;
            }

            return wheelsOutsideVerge;
        }
    

        private static void Handle45DegreeCrash(Vehicle car, int wheel, float collisionAngle, float direction)
        {

            EnvironmentAudioProvider.Instance.PlayVehicleFenceCollision();

            // For front wheels to full collision rotation, for back half!
            if (wheel < 2)
            {
                car.RotateCarAfterCollision = direction * collisionAngle / 1.5f;
                car.Speed *= 0.93f;
            }
            else
            {
                car.RotateCarAfterCollision = direction * collisionAngle / 2.5f;
                car.Speed *= 0.96f;
            }
        }

        private static void HandleHeadOnCrash(Vehicle car, int wheel, float collisionAngle)
        {
            EnvironmentAudioProvider.Instance.PlayVehicleFenceCollision();

            // Also rotate car if less than 60 degrees
            if (Math.Abs(collisionAngle) < MathHelper.Pi / 3.0f)
                car.RotateCarAfterCollision = +collisionAngle / 3.0f;

            // Just stop car!
            car.Speed = 0;
            car.Motor.Idle();

        }
    }
}
