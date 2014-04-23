using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;


namespace NeedForSpeed.Physics
{
	class VehicleWheel
	{
		Vector3 _axlePoint;
		Vehicle _car;
		float _steeringAngle;
		float _size;
		Texture2D _texture;
		Vector3 _trackPositionUnderWheel;
        int _trackTriangleUnderWheel;
        float _rotation;
        ParticleEmitter _smokeEmitter;

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public bool IsSkidding { get; set; }


		public VehicleWheel(Vehicle car, Vector3 axlePoint, Texture2D texture, float size)
		{
			_car = car;
			_axlePoint = axlePoint;
			_axlePoint.Y += size / 2;
			_texture = texture;
			_size = size;
            _trackTriangleUnderWheel = -1;
		}

        public void InitializeForDriving()
        {
            if (_smokeEmitter == null)
                _smokeEmitter = new ParticleEmitter(TyreSmokeParticleSystem.Instance, 20, _trackPositionUnderWheel);
        }

		public Vector3 WorldPosition
		{
			get
			{
				return _car.Position + _axlePoint.Y * _car.UpVector + _car.Direction * _axlePoint.Z - (_car.CarRight) * _axlePoint.X;
			}
		}

		public Vector3 BottomPosition
		{
			get
			{
				return WorldPosition - new Vector3(0, Size / 2, 0);
			}
		}

		public Vector3 GetOffsetPosition(Vector3 offset)
		{
            return (_car.Position + ((_axlePoint.Y + offset.Y) * _car.UpVector)) + _car.Direction * (_axlePoint.Z + offset.Z) -_car.CarRight * (_axlePoint.X + offset.X);
		}

		public float Size
		{
			get { return _size; }
		}

		public Vector3 _lastPositionUnderWheel;

		public Vector3 TrackPositionUnderWheel
		{
			get { return _trackPositionUnderWheel; }
			set
			{
				_lastPositionUnderWheel = _trackPositionUnderWheel;
				_trackPositionUnderWheel = value;
			}
		}


        public int TrackTriangleUnderWheel
        {
            get { return _trackTriangleUnderWheel; }
            set { _trackTriangleUnderWheel = value; }
        }

		public void Steer(float angle)
		{
			_steeringAngle = -angle;
		}

        public void Update(GameTime gameTime)
        {
			_smokeEmitter.Enabled = IsSkidding;
            _smokeEmitter.Update(gameTime, _trackPositionUnderWheel);
            IsSkidding = false;
        }

		public void Render()
		{
			Matrix carMatrix = Matrix.Identity;
			carMatrix.Right = -_car.CarRight;
			carMatrix.Up = _car.UpVector;
			carMatrix.Forward = -_car.Direction;
			carMatrix.Translation = WorldPosition;
			WheelModel.Render(
				Matrix.CreateScale(new Vector3(_size, 2.6f, _size)) *
				Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) *
				Matrix.CreateRotationX(_rotation / _size * 1.5f) *
				Matrix.CreateRotationY(_steeringAngle * 1.3f) *
				carMatrix,
				_texture);
		}
	}
}