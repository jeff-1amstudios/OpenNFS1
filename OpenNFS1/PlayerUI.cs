using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NfsEngine;
using OpenNFS1.Physics;
using OpenNFS1.Tracks;
using OpenNFS1.Vehicles;
using OpenNFS1.Views;


namespace OpenNFS1
{
	class PlayerUI
	{
		const int DebugViewIndex = 0;
		DrivableVehicle _vehicle;
		List<IView> _views = new List<IView>();
		int _currentView = 1;


		public PlayerUI(DrivableVehicle vehicle)
		{
			_vehicle = vehicle;

			_views.Add(new DebugView(_vehicle));
			_views.Add(new ChaseView(_vehicle, 32, 14, 0));
			_views.Add(new DashboardView(_vehicle));
			_views.Add(new BumperView(_vehicle));
			_views.Add(new DropCameraView(_vehicle));
			_views[_currentView].Activate();
		}

		public bool ShouldRenderCar { get { return _views[_currentView].ShouldRenderPlayer; } }

		public TrackNode CurrentNode { get { return _vehicle.CurrentNode; } }

		public void Update(GameTime gameTime)
		{
			if (VehicleController.ChangeView)
			{
				_views[_currentView].Deactivate();

				while (true)
				{
					_currentView++;
					_currentView %= _views.Count;
					if (_views[_currentView].Selectable)
						break;
				}
				_views[_currentView].Activate();
			}
			if (Engine.Instance.Input.WasPressed(Keys.F1))  //toggle debug FPS view
			{
				if (_currentView == DebugViewIndex)
					_currentView = 1;
				else
					_currentView = DebugViewIndex;
				_views[_currentView].Activate();
			}

			_views[_currentView].Update(gameTime);
		}
		

		public void Render()
		{
			_views[_currentView].Render();
		}
	}
}