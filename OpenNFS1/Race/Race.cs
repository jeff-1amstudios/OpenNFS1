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
		public List<IDriver> Drivers {get; private set; }
		public PlayerRaceStats PlayerStats { get; private set; }

		public Race(int nbrLaps, Track track, PlayerDriver player)
		{
			_nbrLaps = nbrLaps;
			Player = player;
			_track = track;
			PlayerStats = new PlayerRaceStats();
			Drivers = new List<IDriver>();
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
			Drivers.ForEach(a=> a.Vehicle.Motor.Gearbox.CurrentGear = 0);
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
			pos.Z -= Drivers.Count * 30;
			pos.X += Drivers.Count % 2 == 0 ? 20 : -20;
			d.Vehicle.Position = pos;
			Drivers.Add(d);
		}

		public void Update()
		{
			foreach (var driver in Drivers)
				driver.Update(Drivers);

			_track.Update();

			/* so you cant reverse over the line  and get 0.1sec laps */
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
				//VehicleController.ForceBrake = false;
				Drivers.ForEach(a => a.Vehicle.Motor.Gearbox.CurrentGear = 1);
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

			_track.Render(Engine.Instance.Camera.Position, Player.Vehicle.CurrentNode);

			foreach (var driver in Drivers)
			{
				if (!renderPlayerVehicle && driver is PlayerDriver)
					continue;
				driver.Vehicle.RenderShadow();
			}
			foreach (var driver in Drivers)
			{
				if (!renderPlayerVehicle && driver is PlayerDriver)
					continue;
				driver.Vehicle.Render();
			}

			if (_trafficController != null) _trafficController.Render();
		}
	}
}
