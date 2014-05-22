using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNFS1
{
	class PlayerRaceStats
	{
		public int CurrentLap { get; private set; }
		DateTime _currentLapStartTime = DateTime.MinValue;
		public List<int> LapTimes = new List<int>();
		public bool HasPassedLapHalfwayPoint;

		public void OnLapStarted()
		{
			if (_currentLapStartTime != DateTime.MinValue)
			{
				LapTimes.Add((int)new TimeSpan(DateTime.Now.Ticks - _currentLapStartTime.Ticks).TotalSeconds);
			}
			CurrentLap++;
			_currentLapStartTime = DateTime.Now;
			HasPassedLapHalfwayPoint = false;
		}

		public TimeSpan CurrentLapTime
		{
			get { return new TimeSpan(DateTime.Now.Ticks - _currentLapStartTime.Ticks); }
		}
	}
}