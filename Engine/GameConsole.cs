using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NfsEngine
{
    public static class GameConsole
    {
        public static void WriteLine(object o, int row)
        {
            Engine.Instance.GraphicsUtils.AddText(new Vector2(20, row * 18 + 100), o.ToString(), Justify.MIDDLE_LEFT, Color.White);
        }

		public static void WriteLine(Vector3 vec, int row)
		{
			vec.X = (float)Math.Round(vec.X, 3);
			vec.Y = (float)Math.Round(vec.Y, 3);
			vec.Z = (float)Math.Round(vec.Z, 3);
			WriteLine(vec.ToString(), row);
		}
	}
}
