
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NfsEngine;
using OpenNFS1.Parsers;
using OpenNFS1;
using OpenNFS1.Parsers.Track;
using System.Diagnostics;
using OpenNFS1.Physics;
using OpenNFS1.Loaders;
using OpenNFS1.Audio;
using OpenNFS1.Dashboards;
using OpenNFS1.Tracks;


namespace OpenNFS1.Physics
{

	abstract class Vehicle
	{
		const float Gravity = 9.81f;
		const float CarFrictionOnRoad = 14;// 0.005f;
		const float AirFrictionPerSpeed = 0.07f; //0.001f;
		const float MaxAirFriction = AirFrictionPerSpeed * 100.0f;
		// for reflecting away from walls
		public float MaxRotationPerSec = 5f;
		public float SteeringSpeed = 2.1f;

		private Track _track;

		public Track Track
		{
			get { return _track; }
			set
			{
				_track = value;
				_speed = 0;
				CurrentNode = _track.RoadNodes[0];
			}
		}

		float _moveFactorPerSecond;

		public float _slipFactor;
		public float _steeringWheel;
		public float MaxSteeringLock = 0.3f;

		protected Vector3 _direction;
		protected Vector3 _up;
		protected Vector3 _force;
		protected float _previousSpeed;
		protected float _speed;
		protected float _mass; //kg;
		private float _currentHeightOfTrack;

		protected Motor _motor;
		private VehicleAudioProvider _audioProvider;
		private int _wheelsOutsideRoad;

		protected float _traction = 520;


		public Vector3 UpVector
		{
			get { return _up; }
		}

		
		public Spring BodyPitch { get; private set; }
		public Spring BodyRoll { get; private set; }

		public float RotateCarAfterCollision {get; set; }		
		protected bool _isOnGround = true;

		protected Matrix _renderMatrix = Matrix.Identity;
		protected AlphaTestEffect _effect;

		private Vector3 _position;
		public Vector3 Position
		{
			get { return _position; }
			set { _position = value; }
		}
		
		public float Speed 
		{
			get { return _speed; }
			set { _speed = value; }
		}

		protected VehicleWheel[] _wheels = new VehicleWheel[4];

		internal VehicleWheel[] Wheels
		{
			get { return _wheels; }
		}

		public Vector3 Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}

		public Vector3 CarRight
		{
			get { return Vector3.Cross(_direction, _up); }
		}

		internal Motor Motor
		{
			get { return _motor; }
		}
		
		public TrackNode CurrentNode { get; private set; }

		public float TrackProgress { get; set; }

		// inputs
		public float ThrottlePedalInput, BrakePedalInput, SteeringInput;
		public bool GearUpInput, GearDownInput;

		public Vehicle(float mass, string name)
		{
			_direction = new Vector3(0, 0, -1);
			_up = Vector3.Up;
			_mass = mass;

			BodyPitch = new Spring(1200, 1.5f, 200, 0, 1.4f);
			BodyRoll = new Spring(1200, 1.5f, 180, 0, 3);

			_audioProvider = new VehicleAudioProvider(this, name);
			_effect = new AlphaTestEffect(Engine.Instance.Device);
		}

		bool _audioEnabled;
		public bool AudioEnabled
		{
			get { return _audioEnabled; }
			set
			{
				if (value)
					_audioProvider.Initialize();
				else
					_audioProvider.StopAll();
				_audioEnabled = value;
			}
		}


		float _rotationChange = 0.0f;

		private void UpdateWheels()
		{
			//front wheels
			for (int i = 0; i < 2; i++)
				_wheels[i].Rotation -= _speed * Engine.Instance.FrameTime;

			//back wheels
			for (int i = 2; i < 4; i++)
				_wheels[i].Rotation -= Engine.Instance.FrameTime * (_motor.WheelsSpinning ? 50 : _speed);

			_wheels[0].Steer(_steeringWheel);
			_wheels[1].Steer(_steeringWheel);
		}

		private void UpdateSteering()
		{
			float elapsedSeconds = Engine.Instance.FrameTime;

			if (SteeringInput < 0)
			{
				_steeringWheel += SteeringSpeed * elapsedSeconds * SteeringInput;
				_steeringWheel = Math.Max(_steeringWheel, -MaxSteeringLock);
			}
			else if (SteeringInput > 0)
			{
				_steeringWheel += SteeringSpeed * elapsedSeconds * SteeringInput;
				_steeringWheel = Math.Min(_steeringWheel, MaxSteeringLock);
			}
			else
			{
				if (_steeringWheel > 0.05f)
					_steeringWheel -= SteeringSpeed * elapsedSeconds;
				else if (_steeringWheel < -0.05f)
					_steeringWheel += SteeringSpeed * elapsedSeconds;
				else
					_steeringWheel = 0;
			}
			_rotationChange = _steeringWheel * 0.05f;
			if (_speed > 0)
				_rotationChange *= -1;

			float maxRot = MaxRotationPerSec * elapsedSeconds;

			// Handle car rotation after collision
			if (RotateCarAfterCollision != 0)
			{
				_audioProvider.PlaySkid(true);

				if (RotateCarAfterCollision > maxRot)
				{
					_rotationChange += maxRot;
					RotateCarAfterCollision -= maxRot;
				}
				else if (RotateCarAfterCollision < -maxRot)
				{
					_rotationChange -= maxRot;
					RotateCarAfterCollision += maxRot;
				}
				else
				{
					_rotationChange += RotateCarAfterCollision;
					RotateCarAfterCollision = 0;
				}
			}
			else
			{
				_slipFactor = 0;
				//If we are stopped or moving very slowly, limit rotation!
				if (Math.Abs(_speed) < 1)
					_rotationChange = 0;
				else if (Math.Abs(_speed) < 20.0f && _rotationChange != 0)
				{
					_rotationChange *= (0.08f * Math.Abs(_speed));
				}
				else
				{
					if (_rotationChange != 0)
					{
						_slipFactor = _mass * Math.Abs(_speed) * 0.0000020f;
						if (BrakePedalInput > 0)
							_slipFactor *= 1.2f;

						_slipFactor = Math.Min(_slipFactor, 0.91f);
						//if (_motor.Throttle == 1)
						//    _rotationChange *= _slipFactor;
						//else
						_rotationChange *= 1 - _slipFactor;
					}
				}

				if (_isOnGround && BrakePedalInput > 0.5f && Math.Abs(_speed) > 10)
				{
					if (_speed < 50)
					{
						_wheels[0].IsSkidding = _wheels[1].IsSkidding = _wheels[2].IsSkidding = _wheels[3].IsSkidding = true;
					}
					_audioProvider.PlaySkid(true);
				}
				else if (_isOnGround && Math.Abs(_steeringWheel) > 0.25f && _slipFactor > 0.43f)
				{
					_audioProvider.PlaySkid(true);
				}
				else
				{
					_audioProvider.PlaySkid(false);
				}

				if (!_isOnGround)
					_rotationChange = 0;
			}
		}

		private void UpdateDrag()
		{
			float elapsedSeconds = Engine.Instance.FrameTime;

			float airFriction = AirFrictionPerSpeed * Math.Abs(_speed);
			if (airFriction > MaxAirFriction)
				airFriction = MaxAirFriction;
			// Don't use ground friction if we are not on the ground.
			float groundFriction = CarFrictionOnRoad;
			if (_isOnGround == false)
				groundFriction = 0;

			_force *= 1.0f - (groundFriction + airFriction) * 0.06f * elapsedSeconds;
			_speed *= 1.0f - (groundFriction + airFriction) * 0.0015f * elapsedSeconds;

			if (_isOnGround)
			{
				float drag = 7000f * BrakePedalInput / (Math.Abs(_speed) * 3f);  //we should slow more quickly as our speed drops
				float inertia = _mass;
				drag += Math.Abs(_steeringWheel) * 10f;
				if (Math.Abs(_speed) > 30)
				{
					drag += _wheelsOutsideRoad * 5f;
				}

				drag += _motor.CurrentFriction * 4f;

				if (drag < 0) drag = 0;

				if (Math.Abs(_speed) < 1)
					drag = 0;

				if (_speed > 0)
				{
					_speed -= drag * elapsedSeconds;
					if (_speed < 0) _speed = 0; //avoid braking so hard we go backwards
						
				}
				else if (_speed < 0)
				{
					_speed += drag * elapsedSeconds;
				}


				// Calculate pitch depending on the force
				float speedChange = _speed - _previousSpeed;

				BodyPitch.ChangePosition(speedChange * 0.6f);
				BodyRoll.ChangePosition(_steeringWheel * -0.05f * Math.Min(1, Math.Abs(_speed) / 30));

				BodyPitch.Simulate(_moveFactorPerSecond);
				BodyRoll.Simulate(_moveFactorPerSecond);
			}
		}

		private void UpdateEngineForce()
		{
			_previousSpeed = _speed;

			float newAccelerationForce = 0.0f;

			_motor.Throttle = ThrottlePedalInput;
			newAccelerationForce += _motor.CurrentPowerOutput * 0.4f;

			if (_motor.Gearbox.GearEngaged && _motor.Gearbox.CurrentGear > 0)
			{
				float tractionFactor = (_traction + _speed) / newAccelerationForce;
				if (tractionFactor > 1) tractionFactor = 1;
				_motor.WheelsSpinning = tractionFactor < 1 || (_motor.Rpm > 0.7f && _speed < 5 && _motor.Throttle > 0);
				if (_motor.WheelsSpinning)
				{
					_audioProvider.PlaySkid(true);
					_wheels[2].IsSkidding = _wheels[3].IsSkidding = true;
				}
				else if (!_isOnGround)
				{
					_motor.WheelsSpinning = true;
				}
			}

			foreach (VehicleWheel wheel in _wheels)
				wheel.Update();

			GearboxAction action = GearboxAction.None;
			if (GearUpInput) action = GearboxAction.GearUp;
			else if (GearDownInput) action = GearboxAction.GearDown;
			_motor.Update(_speed, action);

			if (_motor.AtRedline && !_motor.WheelsSpinning)
			{
				_force *= 0.2f;
			}

			if (_motor.Throttle == 0 && Math.Abs(_speed) < 1)
			{
				_speed = 0;
			}

			if (_isOnGround)
				_force += _direction * newAccelerationForce * (_moveFactorPerSecond) * 1f;

			// Change speed with standard formula, use acceleration as our force

			Vector3 speedChangeVector = _force / _mass;

			if (_isOnGround && speedChangeVector.Length() > 0)
			{
				float speedApplyFactor = Vector3.Dot(Vector3.Normalize(speedChangeVector), _direction);
				if (speedApplyFactor > 1)
					speedApplyFactor = 1;
				_speed += speedChangeVector.Length() * speedApplyFactor;
			}
		}

		private void SetMoveFactor()
		{
			_moveFactorPerSecond = (Engine.Instance.FrameTime) * 1000 / 400;// (float)gameTime.ElapsedGameTime.TotalMilliseconds / 400;
			
			// Make sure this is never below 0.001f and never above 0.5f
			// Else our formulas below might mess up or carSpeed and carForce!
			if (_moveFactorPerSecond < 0.001f)
				_moveFactorPerSecond = 0.001f;
			if (_moveFactorPerSecond > 0.5f)
				_moveFactorPerSecond = 0.5f;
		}

		public virtual void Update()
		{
			SetMoveFactor();
			float moveFactor = _moveFactorPerSecond;

			if (Engine.Instance.Input.WasPressed(Keys.R))
			{
				Reset();
			}

			float elapsedSeconds = Engine.Instance.FrameTime;

			UpdateSteering();
			_direction = Vector3.TransformNormal(_direction, Matrix.CreateFromAxisAngle(_up, _rotationChange));

			UpdateEngineForce();

			UpdateDrag();

			_position += _speed * _direction * moveFactor;


			_audioProvider.UpdateEngine();

			UpdateTrackNode();
			var nextNode = CurrentNode.Next;

			//GameConsole.WriteLine("inAir: " + !_isOnGround + ", " + _direction.Y, 3);
			//GameConsole.WriteLine("slope: " + CurrentNode.Slope + ", " + nextNode.Slope, 4);
			//GameConsole.WriteLine("slope delta: " + (CurrentNode.Slope - nextNode.Slope), 5);
			if ((CurrentNode.Slope - nextNode.Slope > 50 && _speed > 100 && _isOnGround) || Engine.Instance.Input.WasPressed(Keys.Space))
			{
				_isOnGround = false;
				_upVelocity = -0.5f;
				//_position.Y += 0.05f;
			}

			var closestPoint1 = Utility.GetClosestPointOnLine(CurrentNode.GetLeftBoundary(), CurrentNode.GetRightBoundary(), _position);
			var closestPoint2 = Utility.GetClosestPointOnLine(nextNode.GetLeftBoundary(), nextNode.GetRightBoundary(), _position);

			var dist = Vector3.Distance(closestPoint1, closestPoint2);
			var carDist = Vector3.Distance(closestPoint1, _position);
			float ratio = Math.Min(carDist / dist, 1.0f);
			TrackProgress = CurrentNode.Number + ratio;

			if (_isOnGround)
			{
				_up = Vector3.Lerp(CurrentNode.Up, nextNode.Up, ratio);
				_up = Vector3.Normalize(_up);
				_direction = Vector3.Cross(_up, CarRight);
			}

			_currentHeightOfTrack = MathHelper.Lerp(closestPoint1.Y, closestPoint2.Y, ratio); // _track.GetHeightAtPoint(CurrentNode, Position);
			if (_currentHeightOfTrack == -9999)
			{
				throw new Exception();
			}
			if (_isOnGround)
			{
				_position.Y = _currentHeightOfTrack;
			}

			UpdateWheels();
			UpdateCarMatrixAndCamera();
			ApplyGravityAndCheckForCollisions();
		}


		private void UpdateTrackNode()
		{
			var nextNode = CurrentNode.Next;
			var prevNode = CurrentNode.Prev;
			if (!Utility.IsLeftOfLine(nextNode.GetLeftBoundary(), nextNode.GetRightBoundary(), _position))
			{
				CurrentNode = CurrentNode.Next;
				//Debug.WriteLine("passed node - new node " + CurrentNode.Number);
			}
			else if (prevNode != null && Utility.IsLeftOfLine(prevNode.GetLeftBoundary(), prevNode.GetRightBoundary(), _position))
			{
				CurrentNode = prevNode;
				Debug.WriteLine("passed node (back) - new node " + CurrentNode.Number);
			}
		}

		/// <summary>
		/// Resets the car at the center of the current track segment
		/// </summary>
		public void Reset()
		{
			_position = CurrentNode.Position + new Vector3(0, 50, 0);
			_direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(MathHelper.ToRadians(CurrentNode.Orientation)));
			_speed = 0;
			_isOnGround = false;
			ScreenEffects.Instance.UnFadeScreen();
			return;
		}

		
		void ApplyGravityAndCheckForCollisions()
		{
			_wheelsOutsideRoad = VehicleFenceCollision.GetWheelsOutsideRoadVerge(this);

			_audioProvider.PlayOffRoad(_speed > 3 && _wheelsOutsideRoad > 0);
			if (_speed > 3 && _wheelsOutsideRoad > 0)
			{
				_wheels[2].IsSkidding = _wheels[3].IsSkidding = true;
			}

			VehicleFenceCollision.Handle(this);

			ApplyGravity();
		}

		float _upVelocity = 0;
		float _timeInAir = 0;
		private void ApplyGravity()
		{
			if (_isOnGround) return;

			bool wasOnGround = _isOnGround;

			_isOnGround = _position.Y < _currentHeightOfTrack;

			if (!_isOnGround)
			{
				_position.Y -= Gravity * 10f * _timeInAir * Engine.Instance.FrameTime;
				Debug.WriteLine("inair: " + Position.Y + ", " + _direction.Y);
				// slowly pitch the nose of the car downwards - helps to flatten out the jump and looks better
				if (_timeInAir > 0.3f && _direction.Y > -0.3f)
				{
					_direction.Y -= _timeInAir * 0.006f;
					_direction = Vector3.Normalize(_direction);
				}
			}

			if (_isOnGround && !wasOnGround)
			{
				Debug.WriteLine("back on ground");
				if (_timeInAir > 0.2f)
				{
					_audioProvider.HitGround();
				}
			}

			if (_isOnGround)
				_timeInAir = 0;
			else
			{
				_timeInAir += Engine.Instance.FrameTime;
				_upVelocity -= Engine.Instance.FrameTime * 100;
			}
		}


		public void UpdateCarMatrixAndCamera()
		{
			Matrix orientation = Matrix.Identity;
			orientation.Right = CarRight;
			orientation.Up = _up;
			orientation.Forward = _direction;

			_renderMatrix =
					Matrix.CreateRotationX( BodyPitch.Position / 60) *
					Matrix.CreateRotationZ(-BodyRoll.Position * 0.21f) *
					orientation *
					Matrix.CreateTranslation(Position);
		}


		public void RenderShadow()
		{
			// Shadow
			Vector3[] points = new Vector3[4];
			float y = -_wheels[0].Size / 2;
			float xoffset = 0.1f;
			points[0] = _wheels[0].GetOffsetPosition(new Vector3(-xoffset, y, -2));
			points[1] = _wheels[1].GetOffsetPosition(new Vector3(xoffset, y, -2));
			points[2] = _wheels[2].GetOffsetPosition(new Vector3(-xoffset, y, 3.5f));
			points[3] = _wheels[3].GetOffsetPosition(new Vector3(xoffset, y, 3.5f));

			if (!_isOnGround)
			{
				points[0].Y = _currentHeightOfTrack;
				points[1].Y = _currentHeightOfTrack;
				points[2].Y = _currentHeightOfTrack;
				points[3].Y = _currentHeightOfTrack;
			}
			ObjectShadow.Render(points);
		}

		public virtual void Render()
		{
			
			WheelModel.BeginBatch();
			foreach (VehicleWheel wheel in _wheels)
			{
				wheel.Render();
			}
			
			Matrix carMatrix = Matrix.Identity;
			carMatrix.Right = CarRight;
			carMatrix.Up = _up;
			carMatrix.Forward = -_direction;
		}

		protected void Gearbox_GearChanged(object sender, EventArgs e)
		{
			_audioProvider.ChangeGear();
		}
	}
}
