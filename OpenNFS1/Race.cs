using System;
using System.Collections.Generic;

using System.Text;
using OpenNFS1.Physics;
using NfsEngine;
using OpenNFS1.UI.Screens;
using OpenNFS1.Parsers.Track;
using OpenNFS1.Tracks;
using OpenNFS1.Vehicles.AI;
using OpenNFS1.Vehicles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OpenNFS1
{

	class Race
	{
		int _nbrLaps;
		DateTime _countdownStartTime, _raceStartTime;
		bool _started, _finished;
		Track _track;
		public PlayerDriver Player { get; private set; }
		TrafficController _trafficController;
		List<IDriver> _allDrivers = new List<IDriver>();
		public PlayerRaceStats PlayerStats { get; private set; }

		public Race(int nbrLaps, Track track, PlayerDriver player)
		{
			_nbrLaps = nbrLaps;
			Player = player;
			_track = track;
			PlayerStats = new PlayerRaceStats();
			AddDriver(player);
			if (track.Description.IsOpenRoad)
			{
				_trafficController = new TrafficController(_track, Player.Vehicle);
			}
		}


		public int NbrLaps
		{
			get { return _nbrLaps; }
		}

		public TimeSpan RaceTime
		{
			get { return new TimeSpan(DateTime.Now.Ticks - _raceStartTime.Ticks); }
		}

		public int SecondsTillStart
		{
			get { return 3 - (int)new TimeSpan(DateTime.Now.Ticks - _countdownStartTime.Ticks).TotalSeconds; }
		}


		public void StartCountdown()
		{
			VehicleController.ForceBrake = false;
			_countdownStartTime = DateTime.Now;
			_allDrivers.ForEach(a=> a.Vehicle.Motor.Gearbox.CurrentGear = 0);
		}

		public bool HasFinished
		{
			get
			{
				return _finished;
			}
		}

		// add a driver to the race, placing him in a starting grid
		public void AddDriver(IDriver d)
		{
			d.Vehicle.Track = _track;
			Vector3 pos = _track.StartPosition;
			pos.Z -= _allDrivers.Count * 30;
			pos.X += _allDrivers.Count % 2 == 0 ? 20 : -20;
			d.Vehicle.Position = pos;
			_allDrivers.Add(d);
		}

		public void Update()
		{
			foreach (var driver in _allDrivers)
				driver.Update();

			_track.Update();

			/* so you cant reverse over the line and get 0.1sec laps */
			var node = Player.Vehicle.CurrentNode;
			if (node.Number == _track.CheckpointNode && PlayerStats.HasPassedLapHalfwayPoint)
			{
				PlayerStats.OnLapStarted();
			}

			if (node.Number == 100)  //just pick some arbitrary node thats farish away from the start
			{
				PlayerStats.HasPassedLapHalfwayPoint = true;
			}

			if (SecondsTillStart <= 0 && !_started)
			{
				VehicleController.ForceBrake = false;
				_allDrivers.ForEach(a => a.Vehicle.Motor.Gearbox.CurrentGear = 1);
				_raceStartTime = DateTime.Now;
				PlayerStats.OnLapStarted();
				_started = true;
			}

			if (PlayerStats.CurrentLap > _nbrLaps)
			{
				if (Player.Vehicle.Speed > 1)
				{
					VehicleController.ForceBrake = true;
				}
				else
					_finished = true;
			}
			if (_trafficController != null) _trafficController.Update();
		}

		public void Render(bool renderPlayerVehicle)
		{
			Engine.Instance.Device.BlendState = BlendState.Opaque;
			Engine.Instance.Device.DepthStencilState = DepthStencilState.Default;
			Engine.Instance.Device.SamplerStates[0] = SamplerState.PointWrap;

			if (_trafficController != null) _trafficController.Render();

			_track.Render(Engine.Instance.Camera.Position, Player.Vehicle.CurrentNode);

			foreach (var driver in _allDrivers)
			{
				if (!renderPlayerVehicle && driver is PlayerDriver)
					continue;
				driver.Vehicle.RenderShadow();
			}
			foreach (var driver in _allDrivers)
			{
				if (!renderPlayerVehicle && driver is PlayerDriver)
					continue;
				driver.Vehicle.Render();
			}
		}
	}
}
