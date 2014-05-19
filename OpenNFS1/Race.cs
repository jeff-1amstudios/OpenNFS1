using System;
using System.Collections.Generic;

using System.Text;
using OpenNFS1.Physics;
using NfsEngine;
using OpenNFS1.UI.Screens;
using OpenNFS1.Parsers.Track;
using OpenNFS1.Tracks;

namespace OpenNFS1
{
    class Race
    {
        int _currentLap;
        int _nbrLaps;

        DateTime _countdownStartTime, _raceStartTime, _currentLapStartTime;
        List<int> _lapTimes = new List<int>();

        int _lastSegment;
        Vehicle _playerVehicle;
        bool _started, _finished;
        Track _track;
				TrackDescription _trackDescription;
        
        public Race(int nbrLaps, Vehicle playerVehicle, TrackDescription description, Track track)
        {
            _nbrLaps = nbrLaps;
            _currentLapStartTime = DateTime.Now;
            _playerVehicle = playerVehicle;
            _track = track;
						_trackDescription = description;
        }

        internal TrackDescription TrackDescription
        {
            get { return _trackDescription; }
        }

        public List<int> LapTimes
        {
            get { return _lapTimes; }
        }

        public int CurrentLap
        {
            get { return _currentLap; }
        }

        public int NbrLaps
        {
            get { return _nbrLaps; }
        }

        public Vehicle PlayerVehicle
        {
            get { return _playerVehicle; }
        }

        public TimeSpan CurrentLapTime
        {
            get { return new TimeSpan(DateTime.Now.Ticks - _currentLapStartTime.Ticks); }
        }

        public TimeSpan RaceTime
        {
            get { return new TimeSpan(DateTime.Now.Ticks - _raceStartTime.Ticks); }
        }

        public int SecondsTillStart
        {
            get { return 3 - (int)new TimeSpan(DateTime.Now.Ticks - _countdownStartTime.Ticks).TotalSeconds; }
        }

        public void UpdatePosition(TrackNode node)
        {
            if (node.Number == _track.CheckpointNode && _lastSegment > 30 /* so you cant reverse over the line and get 0.1sec laps */)
            {
                _lapTimes.Add((int)new TimeSpan(DateTime.Now.Ticks - _currentLapStartTime.Ticks).TotalSeconds);
                _currentLap++;
                _currentLapStartTime = DateTime.Now;
            }
            //_lastSegment = trackSegment;

            if (SecondsTillStart <= 0 && !_started)
            {
                VehicleController.ForceBrake = false;
                _playerVehicle.Motor.Gearbox.CurrentGear = 1;
                _raceStartTime = DateTime.Now;
                _currentLapStartTime = _raceStartTime;
                _currentLap = 1;
                _started = true;
            }

            if (_currentLap > _nbrLaps)
            {
                if (_playerVehicle.Speed > 1)
                {
                    VehicleController.ForceBrake = true;
                }
                else
                    _finished = true;
            }
        }

        public void StartCountdown()
        {
            VehicleController.ForceBrake = false;
            _countdownStartTime = DateTime.Now;
            _playerVehicle.Motor.Gearbox.CurrentGear = 0; //neutral
        }

        public bool HasFinished
        {
            get
            {
                return _finished;
            }
        }
    }
}
