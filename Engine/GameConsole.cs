using System;
using System.Collections.Generic;
using System.Text;
using GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine
{
    public static class GameConsole
    {
		static int _row = 0;
		public static void Clear()
		{
			_row = 0;
		}

        public static void WriteLine(object o)
        {
			Engine.Instance.GraphicsUtils.AddText(new Vector2(21, _row * 18 + 101), o.ToString(), Justify.MIDDLE_LEFT, Color.Black);
			Engine.Instance.GraphicsUtils.AddText(new Vector2(20, _row * 18 + 100), o.ToString(), Justify.MIDDLE_LEFT, Color.White);
			_row++;
        }

		public static void WriteLine(Vector3 vec)
		{
			vec.X = (float)Math.Round(vec.X, 3);
			vec.Y = (float)Math.Round(vec.Y, 3);
			vec.Z = (float)Math.Round(vec.Z, 3);
			WriteLine(vec.ToString());
		}
	}
}
