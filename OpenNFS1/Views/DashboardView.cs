using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using NfsEngine;
using NeedForSpeed.Views;
using NeedForSpeed.Dashboards;
using Microsoft.Xna.Framework;
using NeedForSpeed.Physics;
using Microsoft.Xna.Framework.Graphics;

namespace NeedForSpeed
{
    class DashboardView : IView
    {
        Vehicle _car;
        SimpleCamera _camera;
        private BaseDashboard _dashboard;
                
        public DashboardView(Vehicle car)
        {
            _car = car;
            _camera = new SimpleCamera();
			_camera.FieldOfView = GameConfig.FOV;
            _dashboard = _car.Dashboard;
        }

        #region IView Members

        public bool Selectable
        {
            get { return true; }
        }

		public bool ShouldRenderPlayer { get { return false; } }

        public void Update(GameTime gameTime)
        {
            _camera.Position = _car.Position + new Vector3(0, 5, 0);
            _camera.LookAt = _camera.Position + _car.Direction * 60f + new Vector3(0, _car.Pitch.Position, 0);
            _camera.UpVector = _car.UpVector; // +new Vector3(_car.Roll.Position * 0.2f, 0, 0);

            _dashboard.Update(gameTime);
        }

        public void Render()
        {
            Engine.Instance.SpriteBatch.Begin();

            _dashboard.Render();

            Engine.Instance.SpriteBatch.End();
        }

        public void Activate()
        {
            Engine.Instance.Camera = _camera;
            _dashboard.IsVisible = true;
        }

        public void Deactivate()
        {
            _dashboard.IsVisible = false;
        }

        #endregion
    }
}
