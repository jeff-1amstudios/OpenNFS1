using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using NfsEngine;

namespace OpenNFS1.Physics
{
	class Motor
	{
        private const float DRIVETRAIN_MULTIPLIER = 34;

		private List<float> _powerCurve;
        private float _maxPower;
        private float _redlineRpm;
        private BaseGearbox _gearbox;
        private float _rpm, _prevRpm, _prevEngagedRpm;
        private float _throttle;
        private float _currentPowerOutput;
        private float _rpmLimiter;
        private float _lastCarSpeed;

        public float CurrentPowerOutput
        {
            get
            {
                if (_gearbox.GearEngaged)
                    return _currentPowerOutput * _throttle * _gearbox.CurrentRatio;
                else
                    return 0;
            }
        }

        public float CurrentFriction
        {
            get
            {
                if (_gearbox.GearEngaged && _throttle == 0)
                    return Math.Abs(_gearbox.CurrentRatio) * 4;
                else
                    return 0;
            }
        }

        public float MaxPower
        {
            get { return _maxPower; }
            set { _maxPower = value; }
        }

        public bool AtRedline
        {
            get { return _rpm >= _redlineRpm; }
        }

        public float Throttle
        {
            get { return _throttle; }
            set { _throttle = value; }
        }

		public float Rpm
		{
			get { return _rpm; }
		}

        public float RedlineRpm
        {
            get { return _redlineRpm; }
        }

        public bool IsAccelerating
        {
            get { return _rpm > _prevRpm; }
        }

        internal BaseGearbox Gearbox
        {
            get { return _gearbox; }
        }

		public Motor(List<float> powerCurve, float maxPower, float redline, BaseGearbox gearbox)
		{
			_powerCurve = powerCurve;
            _maxPower = maxPower;
			_redlineRpm = redline;
			_gearbox = gearbox;
            _gearbox.Motor = this;
		}


        public void Update(float carSpeed, GearboxAction action)
        {
            _prevRpm = _rpm;
            _lastCarSpeed = carSpeed;

            if (_rpm >= _redlineRpm)
            {
                _rpm = _redlineRpm;
            }

            if (_rpmLimiter > 0)
            {
                if (!WheelsSpinning) _currentPowerOutput = 0;
                //_throttle = 0;
				_rpmLimiter -= Engine.Instance.FrameTime;
            }
            else
                _currentPowerOutput = _maxPower * MathHelper.Lerp(_powerCurve[(int)_rpm], _powerCurve[(int)_rpm + 1], _rpm - (int)_rpm);

            if (_gearbox.GearEngaged)
            {
                if (_gearbox.CurrentGear == 0 || WheelsSpinning)
                {
                    HandleRpmNoLoad();
                }
                else
                {
                    _rpm = carSpeed * _gearbox.CurrentRatio / DRIVETRAIN_MULTIPLIER;
                    if (_rpm < 0.8f)
                        _rpm = 0.8f;  //idle speed
                }
                _prevEngagedRpm = _rpm;
            }
            else
            {
                _rpm = MathHelper.Lerp(_prevEngagedRpm /*Math.Abs(carSpeed) * _gearbox.CurrentRatio / DRIVETRAIN_MULTIPLIER*/,
                    carSpeed * _gearbox.NextRatio / DRIVETRAIN_MULTIPLIER, _gearbox.Clutch);   
            }

            if (_rpm < 0.8f)
                _rpm = 0.8f;

            if (_rpm >= _redlineRpm)
            {
                _rpmLimiter = 0.2f;
				_rpm = _redlineRpm;
            }

            _gearbox.Update(_rpm / _redlineRpm, action);
        }

        public float GetPowerAtRpmForGear(float rpm, int gear)
        {
            float power = _maxPower * MathHelper.Lerp(_powerCurve[(int)_redlineRpm], _powerCurve[(int)_redlineRpm + 1], _redlineRpm - (int)_redlineRpm);
            power *= _gearbox.Ratios[gear];
            return power;
        }

        public void Idle()
        {
            _rpm = 0.8f;
            _gearbox.CurrentGear = 1;
        }

        public float GetRpmForGear(int gear)
        {
            return _lastCarSpeed * _gearbox.Ratios[gear] / DRIVETRAIN_MULTIPLIER;
        }

        private void HandleRpmNoLoad()
        {
            if (_throttle == 0.0f || _rpmLimiter > 0)
            {
                _rpm -= Engine.Instance.FrameTime * 4.4f;

                if (_rpm < 0.8f)
                    _rpm = 0.8f;
            }
            else
            {
				_rpm += Engine.Instance.FrameTime *_throttle * 8f;
            }
        }

        public bool WheelsSpinning { get; set; }

        public bool CanChangeDown
        {
            get
            {
                return GetRpmForGear(_gearbox.CurrentGear) / RedlineRpm < 0.9f;
            }
        }
	}
}