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


        public static bool IsPointInsideTriangle(ref Vector3 vA, ref Vector3 vB, ref Vector3 vC, ref Vector3 position, float margin)
        {
            Vector3 vector = vC - vA;
            Vector3 vector2 = vB - vA;
            Vector3 vector3 = position - vA;
            vector.Y = 0;
            vector2.Y = 0;
            vector3.Y = 0;
            float num = vector.LengthSquared();
            float num2 = Vector3.Dot(vector, vector2);
            float num3 = Vector3.Dot(vector, vector3);
            float num4 = vector2.LengthSquared();
            float num5 = Vector3.Dot(vector2, vector3);
            float num6 = 1f / ((num * num4) - (num2 * num2));
            float num7 = ((num4 * num3) - (num2 * num5)) * num6;
            float num8 = ((num * num5) - (num2 * num3)) * num6;
            float num9 = (1f - num7) - num8;
            return (((num7 > -margin) && (num8 > -margin)) && (num9 > -margin));
        }

        public static bool IsPointInsideTriangle3(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 point)
        {
            Vector3 u = v1 - v0;
            Vector3 v = v2 - v0;
            Vector3 w = point - v0;

            float uu = Vector3.Dot(u, u);
            float uv = Vector3.Dot(u, v);
            float vv = Vector3.Dot(v, v);
            float wu = Vector3.Dot(w, u);
            float wv = Vector3.Dot(w, v);
            float d = uv * uv - uu * vv;

            float invD = 1 / d;
            float s = (uv * wv - vv * wu) * invD;
            if (s < 0 || s > 1)
                return false;
            float t = (uv * wu - uu * wv) * invD;
            if (t < 0 || (s + t) > 1)
                return true;

            return true;
        }

        public static bool IsPointInsideTriangle2(ref Vector3 vA, ref Vector3 vB, ref Vector3 vC, ref Vector3 p)
        {
            int pos = 0;
            int neg = 0;
            Vector3[] verts = { vA, vB, vC };
            Plane pl = new Plane(vA, vB, vC);

            uint v0 = 3 - 1;
            for (uint v1 = 0; v1 < 3; v0 = v1, ++v1)
            {
                Vector3 p0 = verts[v0];
                Vector3 p1 = verts[v1];

                // Generate a normal for this edge

                Vector3 n = Vector3.Cross(p1 - p0, pl.Normal);

                // Which side of this edge-plane is the point?

                float halfPlane = Vector3.Dot(p, n) - Vector3.Dot(p0, n);

                // Keep track of positives and negatives 
                //(but not zeros -- which means it's on the edge)

                if (halfPlane > Epsilon) pos++;
                else if (halfPlane < -Epsilon) neg++;

                // Early-out

                if ((pos | neg) != 0) return false;
            }

            // If they're ALL positive, or ALL negative, then it's inside

            if ((pos | neg) == 0) return true;

            // Must not be inside, because some were pos and some were neg

            return false;
        }


        /// <summary>
        /// Returns height at point, or float.MinValue if not found
        /// </summary>
        public static float GetHeightAtPoint(List<Triangle> terrain, Vector3 point)
        {
            float originalHeight = point.Y;
            point.Y = -4000;
            Vector3 hitLocation;
            float hitFl;
            List<float> results = new List<float>();

            for (int i = 0; i < terrain.Count; i++)
            {
                if (Utility.FindRayTriangleIntersection(ref point, Vector3.Up, 20000, ref terrain[i].V1, ref terrain[i].V2, ref terrain[i].V3, out hitLocation, out hitFl))
                {
                    results.Add(hitLocation.Y);
                }
            }

            if (results.Count > 1)
            {
                results.Sort(delegate(float x, float y)
                {
                    return Math.Abs(originalHeight - x).CompareTo(Math.Abs(originalHeight - y));
                });
                return results[0];
            }
            if (results.Count > 0)
                return results[0];

           return float.MinValue;
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
    }
}