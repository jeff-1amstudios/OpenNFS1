using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using NfsEngine;
using OpenNFS1.Views;
using OpenNFS1.Dashboards;
using Microsoft.Xna.Framework;
using OpenNFS1.Physics;
using Microsoft.Xna.Framework.Graphics;
using OneAmEngine;

namespace OpenNFS1
{
	class DebugView : IView
	{
		Vehicle _car;
		FPSCamera _camera;

		public DebugView(Vehicle car)
		{
			_car = car;
			_camera = new FPSCamera();
		}

		#region IView Members

		public bool Selectable
		{
			get { return true; }
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
