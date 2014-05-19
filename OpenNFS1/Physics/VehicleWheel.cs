using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;


namespace OpenNFS1.Physics
{

	class VehicleWheel
	{
		public const float Width = 1.5f;
		Vector3 _axlePoint;
		Vehicle _car;
		float _steeringAngle;
		float _size;
        float _rotation;
		Texture2D _texture;
        ParticleEmitter _smokeEmitter;
		Vector3 _renderOffset;  //we need to offset the wheel

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public bool IsSkidding { get; set; }


		public VehicleWheel(Vehicle car, Vector3 axlePoint, float size, Texture2D texture, float renderXOffset)
		{
			_car = car;
			_axlePoint = axlePoint;
			_size = size;
			_texture = texture;
			_renderOffset = new Vector3(renderXOffset, 0, 0);
			if (_smokeEmitter == null)
				_smokeEmitter = new ParticleEmitter(TyreSmokeParticleSystem.Instance, 20, BottomPosition);
		}

		public Vector3 WorldPosition
		{
			get
			{
				var m = Matrix.Identity; 
				m.Right = _car.CarRight;
				m.Up = _car.UpVector;
				m.Forward = _car.Direction;
				return Vector3.Transform(_axlePoint, m * Matrix.CreateTranslation(_car.Position));
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
			var m = Matrix.Identity;
			m.Right = _car.CarRight;
			m.Up = _car.UpVector;
			m.Forward = _car.Direction;
			var pos = _axlePoint + offset;
			return Vector3.Transform(pos, m * Matrix.CreateTranslation(_car.Position));
		}

		public float Size
		{
			get { return _size; }
		}


		public void Steer(float angle)
		{
			_steeringAngle = -angle;
		}

        public void Update()
        {
			_smokeEmitter.Enabled = IsSkidding;
            _smokeEmitter.Update(BottomPosition);
            IsSkidding = false;
        }

		public void Render()
		{
			Matrix carOrientation = Matrix.Identity;
			carOrientation.Forward = _car.Direction;
			carOrientation.Up = _car.UpVector;
			carOrientation.Right = _car.CarRight;
			WheelModel.Render(
				Matrix.CreateScale(new Vector3(_size, Width, _size)) *
				Matrix.CreateRotationZ(MathHelper.ToRadians(-90)) *  //cylinder geometry faces upwards
				Matrix.CreateRotationX(_rotation / _size * 2f) *
				Matrix.CreateRotationY(_steeringAngle * 1.3f) *
				carOrientation *
				Matrix.CreateTranslation(GetOffsetPosition(_renderOffset)),
				_texture);
		}
	}
}