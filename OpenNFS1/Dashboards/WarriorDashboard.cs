using System;
using System.Collections.Generic;

using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.Physics;

namespace NeedForSpeed.Dashboards
{
    class WarriorDashboard : BaseDashboard
    {

        public WarriorDashboard(Vehicle car)
            : base(car, @"SIMDATA\DASH\traffcdh.fsh")
        {
            _car = car;
        }

        public override void Render()
        {
            base.Render();

            Color color = new Color(165, 0, 0, 255);
            float rpmFactor = _car.Motor.Rpm / _car.Motor.RedlineRpm;
            Vector2 revCounterPosition = _screenSize - new Vector2(515, 166);
            float rotation = (float)(rpmFactor * Math.PI * 0.63f) - 1.8f;

            base.RenderSteeringWheel();

            Engine.Instance.SpriteBatch.Draw(_instrumentLine, revCounterPosition, null, color, rotation, new Vector2(5, 25), new Vector2(0.8f, 2.3f), SpriteEffects.None, 0);
        }
    }
}

