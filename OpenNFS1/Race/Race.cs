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
		public PlayerDriver Player { get; private set; }
		TrafficController _trafficController;
		public List<IDriver> Drivers { get; private set; }
		public PlayerRaceStats PlayerStats { get; private set; }
		public Track Track { get; private set; }

		public Race(int nbrLaps, Track track, PlayerDriver player)
		{
			_nbrLaps = nbrLaps;
			Player = player;
			Track = track;
			PlayerStats = new PlayerRaceStats();
			Drivers = new List<IDriver>();
			AddDriver(player);
			if (track.Description.IsOpenRoad)
			{
				_trafficController = new TrafficController(this);
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
			Drivers.ForEach(a =>
			{
				if (a is AIDriver)
				{
					((DrivableVehicle)a.Vehicle).Motor.Gearbox.CurrentGear = 0;
				}
			});
		}

		public bool HasFinished
		{
			get
			{
				return _finished;
			}
		}

		public void AddDriver(IDriver d)
		{
			AddDriver(d, Track.RoadNodes[1]);
		}

		// add a driver to the race, placing him in a starting grid
		public void AddDriver(IDriver d, TrackNode startNode)
		{
			d.Vehicle.PlaceOnTrack(Track, startNode);
			if (d is TrafficDriver)
			{
			}
			else if (d is PlayerDriver)
			{

			}
			else
			{
				// place on starting grid
				((AIDriver)d).VirtualLane = (Drivers.Count % 2 == 0 ? 3 : 0);
				Vector3 pos = d.Vehicle.CurrentNode.GetLeftVerge();
				pos.Z -= Drivers.Count * 30;
				pos.X = ((AIDriver)d).GetNextTarget().X;
				d.Vehicle.Position = pos;
			}
			Drivers.Add(d);
		}

		public void Update()
		{
			foreach (var driver in Drivers)
				driver.Update(Drivers);

			Track.Update();

			/* so you cant reverse over the line  and get 0.1sec laps */
			var node = Player.Vehicle.CurrentNode;
			if (node.Number == Track.CheckpointNode && PlayerStats.HasPassedLapHalfwayPoint)
			{
				PlayerStats.OnLapStarted();
			}

			if (node.Number == 100)  //just pick some arbitrary node thats farish away from the start
			{
				PlayerStats.HasPassedLapHalfwayPoint = true;
			}

			var sortedDrivers = new List<IDriver>(Drivers);
			sortedDrivers.Sort((a1, a2) =>
				a2.Vehicle.TrackProgress.CompareTo(a1.Vehicle.TrackProgress)
			);
			PlayerStats.Position = sortedDrivers.FindIndex(a => a == Player);

			if (SecondsTillStart <= 0 && !_started)
			{
				//VehicleController.ForceBrake = false;
				Drivers.ForEach(a => {
					if (a.Vehicle is DrivableVehicle)
					{
						((DrivableVehicle)a.Vehicle).Motor.Gearbox.CurrentGear = 1;
					}
				});
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

			Track.Render(Engine.Instance.Camera.Position, Player.Vehicle.CurrentNode);

			foreach (var driver in Drivers)
			{
				if (!renderPlayerVehicle && driver is PlayerDriver)
					continue;
				if (driver.Vehicle is DrivableVehicle)
					((DrivableVehicle)driver.Vehicle).RenderShadow();
			}
			foreach (var driver in Drivers)
			{
				if (!renderPlayerVehicle && driver is PlayerDriver)
					continue;
				driver.Vehicle.Render();
			}
		}
	}
}
