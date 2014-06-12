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
		PlayerUI _playerUI;
		Viewport _raceViewport, _uiViewport;
		PlayerDriver _playerDriver;

		public DoRaceScreen(Track track)
		{
			_track = track;
			_car = GameConfig.SelectedVehicle;

			_playerDriver = new PlayerDriver(_car);
			//_car.AudioEnabled = true;

			_race = new Race(3, _track, _playerDriver);
			for (int i = 0; i < 10; i++)
			{
				int j = Engine.Instance.Random.Next(VehicleDescription.Descriptions.Count);
				_race.AddDriver(new AIDriver(VehicleDescription.Descriptions[j]));
			}
				//_race.AddDriver(new AIDriver(VehicleDescription.Descriptions.Find(a => a.Name == "Viper")));
				//_race.AddDriver(new AIDriver(VehicleDescription.Descriptions.Find(a => a.Name == "Viper")));
				//_race.AddDriver(new AIDriver(VehicleDescription.Descriptions.Find(a => a.Name == "Viper")));
				//_race.AddDriver(new AIDriver(VehicleDescription.Descriptions.Find(a => a.Name == "Viper")));
				_playerUI = new PlayerUI(_car);
			/*
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
			*/
						
			_raceUI = new RaceUI(_race);
			_race.StartCountdown();

			_raceViewport = new Viewport(0, 0, 640, 400);
			_uiViewport = new Viewport(0, 0, 640, 480);
		}

		#region IDrawableObject Members

		public void Update(GameTime gameTime)
		{
			Engine.Instance.Device.Viewport = _raceViewport;

			_race.Update();
			TyreSmokeParticleSystem.Instance.Update();
			
			_playerUI.Update(gameTime);

			if (_race.HasFinished)
			{
				_car.AudioEnabled = false;
				Engine.Instance.Mode = new RaceFinishedScreen(_race, _track);
				return;
			}

			if (UIController.Pause)
			{
				Pause();
				return;
			}

			TyreSmokeParticleSystem.Instance.SetCamera(Engine.Instance.Camera);
			Engine.Instance.Camera.Update(gameTime);
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
			Engine.Instance.Device.Viewport = _raceViewport;

			_race.Render(_playerUI.ShouldRenderCar);
			TyreSmokeParticleSystem.Instance.Render();

			Engine.Instance.Device.Viewport = _uiViewport;

			_raceUI.Render();
			_playerUI.Render();
		}

		#endregion
	}
}
