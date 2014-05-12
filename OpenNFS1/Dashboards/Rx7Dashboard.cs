using System;
using System.Collections.Generic;

using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NfsEngine;
using NeedForSpeed.Physics;

namespace NeedForSpeed.Dashboards
{
    class Rx7Dashboard : BaseDashboard
    {

        public Rx7Dashboard(Vehicle car)
            : base(car, @"SIMDATA\DASH\mrx7dh.fsh")
        {
            _car = car;
        }

        public override void Render()
        {
            base.Render();

            Color color = new Color(165, 0, 0, 255);
            float rpmFactor = _car.Motor.Rpm / _car.Motor.RedlineRpm;
            Vector2 revCounterPosition = _screenSize - new Vector2(518, 185);
            float rotation = (float)(rpmFactor * Math.PI * 1.5f) - 2.62f;

            base.RenderSteeringWheel();

            Engine.Instance.SpriteBatch.Draw(_instrumentLine, revCounterPosition, null, color, rotation, new Vector2(5, 25), new Vector2(0.8f, 1.8f), SpriteEffects.None, 0);
        }
    }
}
