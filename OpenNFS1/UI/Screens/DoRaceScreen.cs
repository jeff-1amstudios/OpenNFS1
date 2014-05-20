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
using OpenNFS1.Vehicles.AI;


namespace OpenNFS1
{
	class DoRaceScreen : IGameScreen
	{
		Track _track;
		DrivableVehicle _car;
		Race _race;
		RaceUI _raceUI;
		TrafficController _trafficController;
		PlayerUI _playerUI;
		Viewport _raceViewport, _uiViewport;
		PlayerDriver _playerDriver;
		List<IDriver> _aiDrivers = new List<IDriver>();

		public DoRaceScreen(Track track)
		{
			_track = track;
			_car = GameConfig.SelectedVehicle;

			_playerDriver = new PlayerDriver(_car);
			_track.AddDriver(_playerDriver);
			_car.AudioEnabled = true;

			AIDriver d = new AIDriver(VehicleDescription.Descriptions.Find(a => a.Name == "RX7"));
			_aiDrivers.Add(d);
			_track.AddDriver(d);
			_playerUI = new PlayerUI(_car);
			
			
			d = new AIDriver(VehicleDescription.Descriptions.Find(a => a.Name == "911"));
			_aiDrivers.Add(d);
			_track.AddDriver(d);
			d = new AIDriver(VehicleDescription.Descriptions.Find(a => a.Name == "Viper"));
			_aiDrivers.Add(d);
			_track.AddDriver(d);
			d = new AIDriver(VehicleDescription.Descriptions.Find(a => a.Name == "Diablo"));
			_aiDrivers.Add(d);
			_track.AddDriver(d);
			d = new AIDriver(VehicleDescription.Descriptions.Find(a => a.Name == "F512"));
			_aiDrivers.Add(d);
			_track.AddDriver(d);
			d = new AIDriver(VehicleDescription.Descriptions.Find(a => a.Name == "ZR1"));
			_aiDrivers.Add(d);
			_track.AddDriver(d);
			d = new AIDriver(VehicleDescription.Descriptions.Find(a => a.Name == "NSX"));
			_aiDrivers.Add(d);
			_track.AddDriver(d);
			

			_race = new Race(3, _car, _aiDrivers, _track);
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

			_playerUI.Update(gameTime);
			_playerDriver.Update();
			foreach (var driver in _aiDrivers)
				driver.Update();

			Engine.Instance.Camera.Update(gameTime);

			_track.Update(gameTime);
			_trafficController.Update(gameTime);

			_race.UpdatePlayerPosition(_playerUI.CurrentNode);

			if (_race.HasFinished)
			{
				_car.AudioEnabled = false;
				Engine.Instance.Mode = new RaceFinishedScreen(_race, _track);
			}

			if (UIController.Pause)
			{
				Pause();
				return;
			}
		}

		public void Pause()
		{
			_car.AudioEnabled = false;
			Engine.Instance.Mode = new RacePausedScreen(this);
		}

		public void Resume()
		{
			_car.AudioEnabled = true;
		}

		public void Draw()
		{
			Engine.Instance.Device.BlendState = BlendState.Opaque;
			Engine.Instance.Device.DepthStencilState = DepthStencilState.Default;
			Engine.Instance.Device.Viewport = _raceViewport;

			_track.Render(Engine.Instance.Camera.Position, _playerUI.CurrentNode);

			_trafficController.Render();

			if (_playerUI.ShouldRenderCar)
			{
				_car.RenderShadow();
			}

			foreach (var driver in _aiDrivers)
			{
				driver.Vehicle.RenderShadow();
			}

			if (_playerUI.ShouldRenderCar)
			{
				_car.Render();
			}
			foreach (var driver in _aiDrivers)
			{
				driver.Vehicle.Render();
			}

			Engine.Instance.Device.Viewport = _uiViewport;

			_playerUI.Render();
			_raceUI.Render();

			Engine.Instance.Device.Viewport = _raceViewport;
		}

		#endregion
	}
}
