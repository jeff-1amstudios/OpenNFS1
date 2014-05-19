using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Parsers;
using OpenNFS1.Parsers.Track;
using Microsoft.Xna.Framework.Input;
using NfsEngine;
using OpenNFS1.Loaders;
using OpenNFS1.UI;
using OpenNFS1.Vehicles;
using OpenNFS1.Physics;
using System.Diagnostics;
using OpenNFS1.UI.Screens;
using OpenNFS1;


namespace OpenNFS1
{
	class DoRaceScreen : IGameScreen
	{
		Track _track;
		DrivableVehicle _car;
		Race _race;
		RaceUI _raceUI;
		TrafficController _trafficController;
		Driver _player;
		Viewport _raceViewport, _uiViewport;

		public DoRaceScreen(Track track)
		{
			_track = track;
			_car = GameConfig.SelectedVehicle;
			_track.AddVehicle(_car);
			_car.EnableAudio();
			_player = new Driver(_car);
			
			_race = new Race(2, _car, GameConfig.SelectedTrack, _track);
			_raceUI = new RaceUI(_race);
			_race.StartCountdown();

			_trafficController = new TrafficController(_track, _car);
			_trafficController.Enabled = GameConfig.SelectedTrack.IsOpenRoad;

			_raceViewport = new Viewport(0, 0, 640, 400);
			_uiViewport = new Viewport(0, 0, 640, 480);
		}

		#region IDrawableObject Members

		public void Update(GameTime gameTime)
		{
			Engine.Instance.Device.Viewport = _raceViewport;

			if (Engine.Instance.Input.WasPressed(Keys.F1))
			{
				GameConfig.Render2dScenery = !GameConfig.Render2dScenery;
			}
			if (Engine.Instance.Input.WasPressed(Keys.F2))
			{
				GameConfig.Render3dScenery = !GameConfig.Render3dScenery;
			}

			_player.Update(gameTime);
			Engine.Instance.Camera.Update(gameTime);

			_track.Update(gameTime);
			_trafficController.Update(gameTime);

			_race.UpdatePosition(_car.CurrentNode);

			if (_race.HasFinished)
			{
				_car.DisableAudio();
				Engine.Instance.Mode = new RaceFinishedScreen(_race, _track);
			}

			if (UIController.Pause)
			{
				Engine.Instance.Mode = new RacePausedScreen(this, _car);
				return;
			}
		}

		public void Draw()
		{
			Engine.Instance.Device.BlendState = BlendState.Opaque;
			Engine.Instance.Device.DepthStencilState = DepthStencilState.Default;
			Engine.Instance.Device.Viewport = _raceViewport;
			
			_track.Render(Engine.Instance.Camera.Position, _car.CurrentNode);

			_trafficController.Render();

			if (_player.ShouldRenderCar)
			{
				_car.Render();
			}

			Engine.Instance.Device.Viewport = _uiViewport;

			_player.Render();
			_raceUI.Render();

			Engine.Instance.Device.Viewport = _raceViewport;
		}

		#endregion
	}
}
