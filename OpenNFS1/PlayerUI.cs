using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using OpenNFS1.Parsers;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Parsers.Track;
using OpenNFS1.Views;
using OpenNFS1.Physics;
using OpenNFS1.Vehicles;
using OpenNFS1.Tracks;


namespace OpenNFS1
{
    class PlayerUI
    {
        DrivableVehicle _vehicle;
        List<IView> _views = new List<IView>();
        int _currentView;

		
        public PlayerUI(DrivableVehicle vehicle)
        {
            _vehicle = vehicle;
            
            _views.Add(new ChaseView(_vehicle, 32, 14, 0));
			_views.Add(new DebugView(_vehicle));
            _views.Add(new DashboardView(_vehicle));
            _views.Add(new BumperView(_vehicle));
            //_views.Add(new ChaseView(_vehicle, 140, 60, 0));           
            _views.Add(new TelevisionView(_vehicle));
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

            _views[_currentView].Update(gameTime);
        }
		

        public void Render()
        {
            _views[_currentView].Render();
        }

        public void ChangeToChaseView()
        {
            _currentView = 3;
            _views[_currentView].Activate();
        }
    }
}