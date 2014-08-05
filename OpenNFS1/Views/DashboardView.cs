using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine;
using OpenNFS1.Dashboards;
using OpenNFS1.Physics;
using OpenNFS1.Vehicles;
using OpenNFS1.Views;

namespace OpenNFS1
{
    class DashboardView : IView
    {
		DrivableVehicle _car;
        SimpleCamera _camera;
        private Dashboard _dashboard;

		public DashboardView(DrivableVehicle car)
        {
            _car = car;
            _camera = new SimpleCamera();
			_camera.FieldOfView = GameConfig.FOV;
			_camera.FarPlaneDistance = GameConfig.DrawDistance;

			var dashfile = Path.GetFileNameWithoutExtension(car.Descriptor.ModelFile) + "dh.fsh";
			var dashDescription = DashboardDescription.Descriptions.Find(a => a.Filename == dashfile);
			_dashboard = new Dashboard(car, dashDescription);
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
			_camera.LookAt = _camera.Position + _car.RenderDirection * 60f + new Vector3(0, _car.BodyPitch.Position, 0);
			_camera.UpVector = _car.Up; // +new Vector3(_car.Roll.Position * 0.2f, 0, 0);

            _dashboard.Update(gameTime);
        }

        public void Render()
        {
			Engine.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

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
