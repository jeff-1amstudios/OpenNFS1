using System;
using System.Collections.Generic;
using System.Text;
using GameEngine;
using Microsoft.Xna.Framework.Input;

namespace OpenNFS1
{
    static class VehicleController
    {

        public static bool ForceBrake { get; set; }

        public static float Acceleration
        {
            get
            {
                if (ForceBrake)
                    return 0;

                if (Engine.Instance.Input.IsKeyDown(Keys.Up))
                    return 1.0f;

                return Engine.Instance.Input.GamePadState.Triggers.Right;
            }
        }

        public static float Brake
        {
            get
            {
                if (ForceBrake)
                    return 1.0f;

                if (Engine.Instance.Input.IsKeyDown(Keys.Down))
                    return 1.0f;

                return Engine.Instance.Input.GamePadState.Triggers.Left;
            }
        }

        public static float Turn
        {
            get
            {
                if (Engine.Instance.Input.IsKeyDown(Keys.Left))
                    return -1;
                else if (Engine.Instance.Input.IsKeyDown(Keys.Right))
                    return 1;

                return Engine.Instance.Input.GamePadState.ThumbSticks.Left.X;
            }
        }

        public static bool ChangeView
        {
            get
            {
                if (Engine.Instance.Input.WasPressed(Keys.C))
                    return true;
                if (Engine.Instance.Input.WasPressed(Buttons.RightShoulder))
                    return true;
                return false;
            }
        }

        public static bool GearUp
        {
            get
            {
                if (Engine.Instance.Input.WasPressed(Keys.A))
                    return true;
                if (Engine.Instance.Input.WasPressed(Buttons.B))
                    return true;
                return false;
            }
        }

        public static bool GearDown
        {
            get
            {
                if (Engine.Instance.Input.WasPressed(Keys.Z))
                    return true;
                if (Engine.Instance.Input.WasPressed(Buttons.X))
                    return true;
                return false;
            }
        }

		public static bool Handbrake
		{
			get
			{
				if (Engine.Instance.Input.IsKeyDown(Keys.Space))
					return true;
				return false;
			}
		}
    }
}
