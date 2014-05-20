using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NfsEngine
{
    public class Triangle
    {
        public Vector3 V1, V2, V3;
        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
        }

    }
    public class Utility
    {
        public static float Epsilon = 1E-5f; //for numerical imprecision
        public static Random RandomGenerator = new Random();

        public static bool FindRayTriangleIntersection(ref Vector3 rayOrigin, Vector3 rayDirection, float maximumLength, ref Vector3 a, ref Vector3 b, ref Vector3 c, out Vector3 hitLocation, out float t)
        {
            hitLocation = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            
            t = float.NegativeInfinity;
            Vector3 vector = b - a;
            Vector3 vector2 = c - a;
            Vector3 vector4 = Vector3.Cross(rayDirection, vector2);
            float num = Vector3.Dot(vector, vector4);
            if ((num > -1E-07f) && (num < 1E-07f))
            {
                return false;
            }
            float num2 = 1f / num;
            Vector3 vector3 = rayOrigin - a;
            float num3 = Vector3.Dot(vector3, vector4) * num2;
            if ((num3 < -0.01f) || (num3 > 1.01f))
            {
                return false;
            }
            Vector3 vector5 = Vector3.Cross(vector3, vector);
            float num4 = Vector3.Dot(rayDirection, vector5) * num2;
            if ((num4 < -0.01f) || ((num3 + num4) > 1.01f))
            {
                return false;
            }
            t = Vector3.Dot(vector2, vector5) * num2;
            if ((t > maximumLength) || (t < 0f))
            {
                t = float.NegativeInfinity;
                return false;
            }
            hitLocation = rayOrigin + (t * rayDirection);
            return true;
        }
        

        public static Vector3 RotatePoint(Vector2 point, float degrees)
        {
            float cDegrees = ((float)Math.PI * degrees) / 180.0f; //Radians
            float cosDegrees = (float)Math.Cos(cDegrees);
            float sinDegrees = (float)Math.Sin(cDegrees);

            float x = (point.X * cosDegrees) + (point.Y * sinDegrees);
            float z = (point.X * -sinDegrees) + (point.Y * cosDegrees);
            return new Vector3(x, 0, -z);
        }

		public static bool IsLeftOfLine(Vector3 line1, Vector3 line2, Vector3 pos)
		{
			return ((line2.X - line1.X) * (pos.Z - line1.Z) - (line2.Z - line1.Z) * (pos.X - line1.X)) > 0;
		}

		public static Vector3 GetClosestPointOnLine(Vector3 line1, Vector3 line2, Vector3 pos)
		{
			Vector3 lineDir = line2 - line1;
			lineDir.Normalize();

			float d = Vector3.Distance(line1, line2);

			Vector3 v1 = pos - line1;
			float t = Vector3.Dot(lineDir, v1);

			if (t <= 0)
				return line1;
			if (t >= d)
				return line2;

			return line1 + lineDir * t;
		}

		public static float GetSignedAngleBetweenVectors(Vector3 from, Vector3 to, bool ignoreY)
		{
			if (ignoreY) from.Y = to.Y = 0;
			from.Normalize();
			to.Normalize();
			Vector3 toRight = Vector3.Cross(to, Vector3.Up);
			toRight.Normalize();

			float forwardDot = Vector3.Dot(from, to);
			float rightDot = Vector3.Dot(from, toRight);

			// Keep dot in range to prevent rounding errors
			forwardDot = MathHelper.Clamp(forwardDot, -1.0f, 1.0f);

			double angleBetween = Math.Acos(forwardDot);

			if (rightDot < 0.0f)
				angleBetween *= -1.0f;

			return (float)angleBetween;
		}
    }
}