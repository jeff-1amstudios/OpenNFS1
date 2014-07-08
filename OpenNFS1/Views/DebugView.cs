using Microsoft.Xna.Framework;
using NfsEngine;
using OneAmEngine;
using OpenNFS1.Physics;
using OpenNFS1.Views;

namespace OpenNFS1
{
	class DebugView : IView
	{
		DrivableVehicle _car;
		FPSCamera _camera;

		public DebugView(DrivableVehicle car)
		{
			_car = car;
			_camera = new FPSCamera();
		}

		#region IView Members

		public bool Selectable
		{
			get { return false; }
		}

		public bool ShouldRenderPlayer { get { return true; } }

		public void Update(GameTime gameTime)
		{
			_camera.Update(gameTime);
		}

		public void Render()
		{
		}

		public void Activate()
		{
			Engine.Instance.Camera = _camera;
			_camera.Position = _car.Position;
		}

		public void Deactivate()
		{
		}

		#endregion
	}
}
