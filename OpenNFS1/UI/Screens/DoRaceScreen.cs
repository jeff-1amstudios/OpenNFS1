using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.Parsers;
using NeedForSpeed.Parsers.Track;
using Microsoft.Xna.Framework.Input;
using NfsEngine;
using NeedForSpeed.Loaders;
using NeedForSpeed.UI;
using NeedForSpeed.Vehicles;
using NeedForSpeed.Physics;
using System.Diagnostics;
using NeedForSpeed.UI.Screens;
using OpenNFS1;


namespace NeedForSpeed
{
	class DoRaceScreen : IGameScreen
	{
		Track _track;
		Vehicle _car;
		Race _race;
		RaceUI _raceUI;
		TrafficController _trafficController;

		public DoRaceScreen(Track track)
		{
			_track = track;
			_car = GameConfig.SelectedVehicle;
			_car.InitializeForDriving();
			Engine.Instance.Player = new Driver(_car, _track);

			//_car._prevPosition = new Vector3()
			//_car.Reset();

			_race = new Race(2, _car, GameConfig.SelectedTrack, _track);
			_raceUI = new RaceUI(_race);
			_race.StartCountdown();

			_trafficController = new TrafficController(_track, _car);
			_trafficController.Enabled = GameConfig.SelectedTrack.IsOpenRoad;
		}

		#region IDrawableObject Members

		public void Update(GameTime gameTime)
		{
			if (Engine.Instance.Input.WasPressed(Keys.F1))
			{
				GameConfig.Render2dScenery = !GameConfig.Render2dScenery;
			}
			if (Engine.Instance.Input.WasPressed(Keys.F2))
			{
				GameConfig.Render3dScenery = !GameConfig.Render3dScenery;
			}

			Engine.Instance.Player.Update(gameTime);
			Engine.Instance.Camera.Update(gameTime);

			_track.Update(gameTime);
			_trafficController.Update(gameTime);

			//int currentSegment = _car.CurrentTrackTriangle / TrackAssembler.TRIANGLES_PER_SEGMENT;
			_race.UpdatePosition(_car.CurrentTrackNode);

			if (_race.HasFinished)
			{
				_car.StopDriving();
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
			Engine.Instance.Device.BlendState = BlendState.NonPremultiplied;
			Engine.Instance.Device.DepthStencilState = DepthStencilState.Default;
			
			_track.Render(Engine.Instance.Camera.Position, _car.CurrentTrackNode);

			_trafficController.Render();

			Engine.Instance.Player.Render();

			_raceUI.Render();
		}

		#endregion
	}
}
