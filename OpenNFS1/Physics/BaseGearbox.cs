using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using NfsEngine;

namespace OpenNFS1.Physics
{
    class GearboxGearChange
    {
        public int Change;
        public float TimeTillEngaged;
    }

	enum GearboxAction
	{
		None,
		GearUp,
		GearDown
	}

    abstract class BaseGearbox
    {
        public const int GEAR_REVERSE = 0;
        public const int GEAR_NEUTRAL = 1;
        public const int GEAR_1 = 2;

        public event EventHandler GearChangeStarted;
        public event EventHandler GearChangeCompleted;

        private List<float> _ratios;

        protected float _changeTime;
        protected int _currentGear;
        protected GearboxGearChange _gearChange;
        protected Motor _motor;
        private float _clutch;

        public float Clutch
        {
            get { return _clutch; }
            set { _clutch = value; }
        }

        public List<float> Ratios
        {
            get { return _ratios; }
        }

        public int CurrentGear
        {
            get { return _currentGear - 1; }
            set { _currentGear = value + 1; }
        }

        public float CurrentRatio
        {
            get { return _ratios[_currentGear]; }
        }

        public float NextRatio
        {
            get { return _ratios[_currentGear + _gearChange.Change]; }
        }

        public int NextGear
        {
            get { return CurrentGear + _gearChange.Change; }
        }

        public bool GearEngaged
        {
            get { return _gearChange == null; }
        }

        public Motor Motor
        {
            set { _motor = value; }
        }

        public BaseGearbox(List<float> ratios, float changeTime)
        {
            ratios.Insert(0, -ratios[0] * 1.5f); //insert reverse
            ratios.Insert(1, 0); //insert neutral
            
            _ratios = ratios;
            _changeTime = changeTime;
        }

        public void GearUp()
        {
            if (_gearChange == null)
            {
                _gearChange = new GearboxGearChange();
                _gearChange.Change = 1;
                _gearChange.TimeTillEngaged = _changeTime;
                _clutch = 0.0f;
                GearChangeStarted(this, null);
            }
        }

        public void GearDown()
        {
            if (_gearChange == null)
            {
                _gearChange = new GearboxGearChange();
                _gearChange.Change = -1;
                _gearChange.TimeTillEngaged = _changeTime;
                _clutch = 0.0f;
                GearChangeStarted(this, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="motorRpmPercent">0..1 where 1 is max rpms</param>
        /// <param name="gameTime"></param>
        public virtual void Update(float motorRpmPercent, GearboxAction action)
        {
            if (_gearChange != null)
            {
				_gearChange.TimeTillEngaged -= Engine.Instance.FrameTime;

                if (_gearChange.TimeTillEngaged <= 0)
                {
                    if (_gearChange.Change > 0)
                    {
                        if (_currentGear < _ratios.Count - 1)
                            _currentGear++;
                    }
                    else
                    {
                        if (_currentGear > -1)
                            _currentGear--;
                    }
                    _clutch = 1.0f;
                    _gearChange = null;
                    if (GearChangeCompleted != null) GearChangeCompleted(this, null);
                }
                else
                {
                    _clutch = (_changeTime - _gearChange.TimeTillEngaged) / _changeTime;
                }
            }
        }

        public static BaseGearbox Create(bool manual, List<float> ratios, float changeTime)
        {
            if (manual)
                return new ManualGearbox(ratios, changeTime);
            else
                return new AutoGearbox(ratios, changeTime);
        }
    }
}