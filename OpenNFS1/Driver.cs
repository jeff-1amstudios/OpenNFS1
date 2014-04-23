using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using NeedForSpeed.Parsers;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.Parsers.Track;
using NeedForSpeed.Views;
using NeedForSpeed.Physics;
using NeedForSpeed.Vehicles;


namespace NeedForSpeed
{
    class Driver : GameObject
    {
        Vehicle _vehicle;
        Track _track;

        List<IView> _views = new List<IView>();
        int _currentView;


        public Driver(Vehicle vehicle, Track track)
        {
            _vehicle = vehicle;
            _vehicle.Position = track.StartPosition + new Vector3(0, 2, 0);
            _vehicle.Direction = Vector3.Forward;
            _vehicle.Track = track;

            _track = track;

            _views.Add(new ChaseView(_vehicle, 90, 48, 0));
            _views.Add(new DashboardView(_vehicle));
            _views.Add(new BumperView(_vehicle));
            _views.Add(new ChaseView(_vehicle, 120, 60, 0));           
            _views.Add(new TelevisionView(_vehicle));
			_views[_currentView].Activate();
        }

        public override void Update(GameTime gameTime)
        {
            _vehicle.Update(gameTime);
            
            _vehicle.ApplyGravityAndCheckForCollisions(_track);

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

            _position = _vehicle.Position;
            
            _views[_currentView].Update(gameTime);
        }
		

        public override void Render()
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