using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework.Input;

namespace OpenNFS1
{
    static class UIController
    {

        public static bool Left
        {
            get
            {
                if (Engine.Instance.Input.WasPressed(Keys.Left))
                    return true;
                if (Engine.Instance.Input.WasPressed(Buttons.DPadLeft))
                    return true;
                return false;
            }
        }

        public static bool Right
        {
            get
            {
                if (Engine.Instance.Input.WasPressed(Keys.Right))
                    return true;
                if (Engine.Instance.Input.WasPressed(Buttons.DPadRight))
                    return true;
                return false;
            }
        }

        public static bool Up
        {
            get
            {
                if (Engine.Instance.Input.WasPressed(Keys.Up))
                    return true;
                if (Engine.Instance.Input.WasPressed(Buttons.DPadUp))
                    return true;
                return false;
            }
        }

        public static bool Down
        {
            get
            {
                if (Engine.Instance.Input.WasPressed(Keys.Down))
                    return true;
                if (Engine.Instance.Input.WasPressed(Buttons.DPadDown))
                    return true;
                return false;
            }
        }

        public static bool Back
        {
            get
            {
                if (Engine.Instance.Input.WasPressed(Keys.Escape))
                    return true;
                if (Engine.Instance.Input.WasPressed(Buttons.B))
                    return true;
                return false;
            }
        }

        public static bool Ok
        {
            get
            {
                if (Engine.Instance.Input.WasPressed(Keys.Enter))
                    return true;
                if (Engine.Instance.Input.WasPressed(Buttons.A))
                    return true;
                return false;
            }
        }

        public static bool Pause
        {
            get
            {
                if (Engine.Instance.Input.WasPressed(Keys.P) || Engine.Instance.Input.WasPressed(Keys.Escape))
                    return true;
                if (Engine.Instance.Input.WasPressed(Buttons.Start))
                    return true;
                return false;
            }
        }
    }
}
