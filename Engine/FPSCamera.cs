//-----------------------------------------------------------------------------
// Copyright (c) 2007 dhpoware. All Rights Reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameEngine;


namespace OneAmEngine
{
	/// <summary>
	/// The FirstPersonCamera class implements the logic for a first person
	/// style 3D camera. This class also handles player input that is used
	/// to control the camera. To use this class, create an instance of the
	/// FirstPersonCamera class and then call the Update() method once a
	/// frame from your game's main loop. The FirstPersonCamera's Update()
	/// method will process mouse and keyboard input used to manipulate the
	/// camera. To change the default movement key bindings call the
	/// MapActionToKey() method. Most of the code in this class is used to
	/// simulate camera view bobbing, crouching, and jumping.
	/// </summary>
	public class FPSCamera : ICamera
	{

		public const float DEFAULT_FOVX = 60.0f;
		public const float DEFAULT_ROTATION_SPEED = 0.25f;

		public const float DEFAULT_ZNEAR = 0.1f;

		private const float GRAVITY = -9.8f;
		private const float DECELERATION = -0.5f;
		private const float STRAFE_SPEED_MULTIPLIER = 15.5f;

		private const float VelocityInversionMultiplier = 20.0f;
		private const float Acceleration = 5.0f;
		private const float Deceleration = -5.0f;
		private const float JumpVelocity = 0.23f;
		private const float MaxSpeed = 1.5f;
		
		private float _strafeDelta, _forwardDelta, _velocity;

		private Vector3 _orientation, _position;

		public Vector3 Position
		{
			get { return _position; }
			set { _position = value; }
		}
		public Vector3 Orientation
		{
			get { return _orientation; }
			set { _orientation = value; }
		}

		public float DrawDistance { get; set; }

		public Matrix View { get; private set; }
		public Matrix Projection { get; private set; }

		public FPSCamera()
		{
			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(DEFAULT_FOVX), Engine.Instance.AspectRatio, DEFAULT_ZNEAR, 15000);
			View = Matrix.Identity;
		}


		public void Update(GameTime gt)
		{
			InputProvider input = Engine.Instance.Input;
			float elapsedTime = Engine.Instance.FrameTime;

			_forwardDelta = input.MoveForward * elapsedTime * Acceleration;

			_strafeDelta = input.Strafe * elapsedTime * Acceleration;

			float speed = 0.5f;

			if (input.IsKeyDown(Keys.Home))
			{
				_orientation.Y -= speed * elapsedTime;
			}
			if (input.IsKeyDown(Keys.End))
			{
				_orientation.Y += speed * elapsedTime;
			}
			if (input.IsKeyDown(Keys.Delete))
			{
				_orientation.X += speed * elapsedTime;
			}
			if (input.IsKeyDown(Keys.PageDown))
			{
				_orientation.X -= speed * elapsedTime;
			}

			UpdateVelocity();
			MoveForward();

			_position.X += (float)(Math.Cos(_orientation.X) * input.Strafe);
			_position.Z -= (float)(Math.Sin(_orientation.X) * input.Strafe);
			
			Matrix view = Matrix.CreateTranslation(-Position);
			view *= Matrix.CreateRotationY(-_orientation.X);
			view *= Matrix.CreateRotationX(_orientation.Y);
			view *= Matrix.CreateRotationZ(_orientation.Z);

			View = view;
			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(DEFAULT_FOVX), Engine.Instance.AspectRatio, DEFAULT_ZNEAR, 15000);
		}

		private void UpdateVelocity()
		{
			float elapsedTimeSec = Engine.Instance.FrameTime;

			// Accelerate or decelerate as camera is moved forward or backward.
			float acceleration = Acceleration;

			if (_forwardDelta != 0.0f)
			{
				// Speed up the transition from moving backwards to moving
				// forwards and vice versa. Otherwise there will be too much
				// of a delay as the camera slows down and then accelerates.
				if ((_forwardDelta > 0.0f && _velocity < 0.0f) ||
					(_forwardDelta < 0.0f && _velocity > 0.0f))
				{
					acceleration *= VelocityInversionMultiplier;
				}

				_velocity += _forwardDelta * acceleration;
			}
			else
			{

				if (_velocity > 0.0f)
				{
					_velocity += Deceleration * elapsedTimeSec;

					if (_velocity < 0.0f)
						_velocity = 0.0f;
				}
				else if (_velocity < 0.0f)
				{
					_velocity -= Deceleration * elapsedTimeSec;

					if (_velocity > 0.0f)
						_velocity = 0.0f;
				}

			}

			if (_velocity > MaxSpeed)
			{
				_velocity = MaxSpeed;
				acceleration = 0;
			}

			if (_velocity < -MaxSpeed)
			{
				_velocity = -MaxSpeed;
				acceleration = 0;
			}
		}

		public void MoveForward()
		{
			_position.X -= (float)((Math.Sin(_orientation.X) * Math.Cos(_orientation.Y)) * _velocity);
			_position.Z -= (float)((Math.Cos(_orientation.X) * Math.Cos(_orientation.Y)) * _velocity);
			_position.Y -= _orientation.Y * _velocity;
		}

		public void SetPosition(Vector3 pos)
		{

		}

		public void FollowObject(GameObject obj)
		{

		}
	}
}