
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NfsEngine;
using NeedForSpeed.Parsers;
using NeedForSpeed;
using NeedForSpeed.Parsers.Track;
using System.Diagnostics;
using NeedForSpeed.Physics;
using NeedForSpeed.Loaders;
using NeedForSpeed.Audio;
using NeedForSpeed.Dashboards;
using OpenNFS1.Tracks;


namespace NeedForSpeed.Physics
{
	abstract class Vehicle
	{
		#region Constants

		const float Gravity = 9.81f;
		const float CarFrictionOnRoad = 14;// 0.005f;
		const float AirFrictionPerSpeed = 0.07f; //0.001f;
		const float MaxAirFriction = AirFrictionPerSpeed * 100.0f;

		/// <summary>
		/// Max rotation per second we use for our car.
		/// </summary>
		public const float MaxRotationPerSec = 2.5f;

		#endregion

		#region Variables

		#region Car variables (based on the car model)

		private Track _track;

		public Track Track
		{
			get { return _track; }
			set
			{
				_track = value;
				_prevPosition = _position;
				_speed = 0;
				CurrentNode = _track.RoadNodes[0];
			}
		}

		float _moveFactorPerSecond;

		public float _slipFactor;
		public float _steeringWheel;
		public float MaxSteeringLock = 0.3f;

		public Vector3 _position, _prevPosition;
		protected Vector3 _direction;
		protected Vector3 _up;
		protected Vector3 _force;
		protected float _previousSpeed;
		protected float _speed;
		protected float _mass; //kg
		protected float _bodyRideHeight = 0.0f;

		protected Motor _motor;
		private VehicleAudioProvider _audioProvider;
		private int _wheelsOutsideRoad;

		protected float _traction = 520;


		public Vector3 UpVector
		{
			get { return _up; }
		}

		public float FrontSlipFactor
		{
			get { return _slipFactor; }
		}

		#endregion

		private Spring _carPitchSpring;

		public Spring Pitch
		{
			get { return _carPitchSpring; }
		}
		private Spring _carRollSpring;


		/// <summary>
		/// Rotate car after collision.
		/// </summary>
		float _rotateCarAfterCollision = 0;

		public float RotateCarAfterCollision
		{
			get { return _rotateCarAfterCollision; }
			set { _rotateCarAfterCollision = value; }
		}

		/// <summary>
		/// Is car on ground? Only allow rotation, apply ground friction,
		/// speed changing if we are on ground and adding brake tracks.
		/// </summary>
		protected bool _isOnGround = true;

		/// <summary>
		/// Car render matrix we calculate each frame.
		/// </summary>
		protected Matrix _renderMatrix = Matrix.Identity;

		protected AlphaTestEffect _effect;

		#endregion

		#region Properties

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

		AverageValueVector3 _upVectors = new AverageValueVector3(15);

		bool _allWheelsOnTrack;

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

		internal abstract BaseDashboard Dashboard { get; }
		
		public TrackNode CurrentNode { get; private set; }

		#endregion


		public Vehicle(float mass, string name)
		{
			_direction = new Vector3(0, 0, -1);
			_up = Vector3.Up;
			_mass = mass;

			_carPitchSpring = new Spring(1200, 1.5f, 200, 0, 1.4f);
			_carRollSpring = new Spring(1200, 1.5f, 180, 0, 3);

			_audioProvider = new VehicleAudioProvider(this, name);
			_effect = new AlphaTestEffect(Engine.Instance.Device);
		}

		public virtual void InitializeForDriving()
		{
			_audioProvider.Initialize();

			foreach (VehicleWheel wheel in _wheels)
				wheel.InitializeForDriving();
		}

		public void StopDriving()
		{
			_audioProvider.StopAll();
			TyreSmokeParticleSystem.Instance.Clear();
		}


		#region Update

		float _rotationChange = 0.0f;

		private void UpdateWheels()
		{
			//front wheels
			for (int i = 0; i < 2; i++)
				_wheels[i].Rotation += _speed * Engine.Instance.FrameTime;

			//back wheels
			for (int i = 2; i < 4; i++)
				_wheels[i].Rotation += Engine.Instance.FrameTime * (_motor.WheelsSpinning ? 50 : _speed);

			_wheels[0].Steer(_steeringWheel);
			_wheels[1].Steer(_steeringWheel);
		}

		private void UpdateSteering()
		{
			float steeringSpeed = 2.4f;
			float elapsedSeconds = Engine.Instance.FrameTime;

			if (VehicleController.Turn < 0)
			{
				float extraForce = _steeringWheel > 0 ? 2 : 1;
				_steeringWheel += steeringSpeed * elapsedSeconds * VehicleController.Turn;
				_steeringWheel = Math.Max(_steeringWheel, -MaxSteeringLock);
			}
			else if (VehicleController.Turn > 0)
			{
				_steeringWheel += steeringSpeed * elapsedSeconds * VehicleController.Turn;
				_steeringWheel = Math.Min(_steeringWheel, MaxSteeringLock);
			}
			else
			{
				if (_steeringWheel > 0.005f)
					_steeringWheel -= steeringSpeed * elapsedSeconds;
				else if (_steeringWheel < -0.005f)
					_steeringWheel += steeringSpeed * elapsedSeconds;
				else
					_steeringWheel = 0;
			}
			_rotationChange = _steeringWheel * 0.05f;
			if (_speed > 0)
				_rotationChange *= -1;

			float maxRot = MaxRotationPerSec;

			// Handle car rotation after collision
			if (_rotateCarAfterCollision != 0)
			{
				_audioProvider.PlaySkid(true);

				if (_rotateCarAfterCollision > maxRot)
				{
					_rotationChange += maxRot;
					_rotateCarAfterCollision -= maxRot;
				}
				else if (_rotateCarAfterCollision < -maxRot)
				{
					_rotationChange -= maxRot;
					_rotateCarAfterCollision += maxRot;
				}
				else
				{
					_rotationChange += _rotateCarAfterCollision;
					_rotateCarAfterCollision = 0;
				}
			}
			else
			{
				_slipFactor = 0;
				//If we are stopped or moving very slowly, limit rotation!
				if (Math.Abs(_speed) < 1)
					_rotationChange = 0;
				else if (Math.Abs(_speed) < 20.0f && _rotationChange != 0)
					_rotationChange *= (0.06f * Math.Abs(_speed));
				else
				{
					if (_rotationChange != 0)
					{
						_slipFactor = _mass * Math.Abs(_speed) * 0.0000020f;
						if (VehicleController.Brake > 0)
							_slipFactor *= 1.2f;

						_slipFactor = Math.Min(_slipFactor, 0.91f);
						//if (_motor.Throttle == 1)
						//    _rotationChange *= _slipFactor;
						//else
						_rotationChange *= 1 - _slipFactor;
					}
				}

				if (_isOnGround && VehicleController.Brake > 0.5f && Math.Abs(_speed) > 5)
				{
					_wheels[0].IsSkidding = _wheels[1].IsSkidding = _wheels[2].IsSkidding = _wheels[3].IsSkidding = true;
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

			/* 20% for force slowdown*/
			_force *= 1.0f - (0.275f * 0.02125f * 0.2f * (groundFriction + airFriction));
			_speed *= 1.0f - (0.01f * 0.1f * 0.02125f * (groundFriction + airFriction));

			if (_isOnGround)
			{
				float drag = _mass * 0.03f * elapsedSeconds * VehicleController.Brake;
				drag += Math.Abs(_steeringWheel) * 23f * elapsedSeconds;
				if (Math.Abs(_speed) > 30)
				{
					drag += _wheelsOutsideRoad * 5f * elapsedSeconds;
				}

				if (_motor.Throttle == 0 && _motor.Gearbox.GearEngaged)
				{
					drag += _motor.CurrentFriction * elapsedSeconds * 0.5f;
				}

				if (Math.Abs(_speed) < 1)
					drag = 0;

				if (_speed > 0)
					_speed -= drag;
				else if (_speed < 0)
					_speed += drag;


				// Calculate pitch depending on the force
				float speedChange = _speed - _previousSpeed;

				_carPitchSpring.ChangePosition(speedChange * 0.6f);
				_carRollSpring.ChangePosition(_steeringWheel * -0.05f * Math.Min(1, _speed / 30));

				_carPitchSpring.Simulate(_moveFactorPerSecond);
				_carRollSpring.Simulate(_moveFactorPerSecond);
			}
			else
			{
				//air drag
				_speed -= (_mass * 0.001f * elapsedSeconds);
			}
		}

		private void UpdateEngineForce()
		{
			float newAccelerationForce = 0.0f;

			_motor.Throttle = VehicleController.Acceleration;
			newAccelerationForce += _motor.CurrentPowerOutput * 20;

			if (_motor.Gearbox.GearEngaged && _motor.Gearbox.CurrentGear > 0)
			{
				float tractionFactor = (_traction + _speed) / newAccelerationForce;
				if (tractionFactor > 1) tractionFactor = 1;
				_motor.WheelsSpinning = tractionFactor < 1 || (_motor.Rpm > 0.7f && _speed < 5 && _motor.Throttle > 0);
				if (_motor.WheelsSpinning)
				{
					//_force *= 0.98f;
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

			TyreSmokeParticleSystem.Instance.Update();
			TyreSmokeParticleSystem.Instance.SetCamera(Engine.Instance.Camera);

			_motor.Update(_speed);

			if (_motor.AtRedline && !_motor.WheelsSpinning)
			{
				_force *= 0.2f;
			}

			if (_motor.Throttle == 0 && Math.Abs(_speed) < 1)
			{
				_speed = 0;
			}

			// Add acceleration force to total car force, but use the current carDir!
			if (_isOnGround)
				_force += _direction * newAccelerationForce * (_moveFactorPerSecond) * 1f;

			// Change speed with standard formula, use acceleration as our force
			_previousSpeed = _speed;
			Vector3 speedChangeVector = _force / _mass;
			// Only use the amount important for our current direction (slower rot)
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

		public virtual void Update(GameTime gameTime)
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

			_position += _speed * _direction * moveFactor * 1f;

			
			_audioProvider.UpdateEngine();

			#region Update track position and handle physics

			Matrix trackMatrix = Matrix.Identity;

			var nextNode = CurrentNode.Next;
			if (!Utility.IsLeftOfLine(nextNode.GetLeftBoundary(), nextNode.GetRightBoundary(), Position))
			{
				Debug.WriteLine("Node.b: {0},{1},{2},{3}", nextNode.b[0], nextNode.b[1], nextNode.b[2], nextNode.b[3]);
				CurrentNode = CurrentNode.Next;
				nextNode = CurrentNode.Next;
				Debug.WriteLine("passed node - new node " + CurrentNode.Number);
			}

			var closestPoint1 = Utility.GetClosestPointOnLine(CurrentNode.GetLeftBoundary(), CurrentNode.GetRightBoundary(), _position);
			var closestPoint2 = Utility.GetClosestPointOnLine(nextNode.GetLeftBoundary(), nextNode.GetRightBoundary(), _position);

			var dist = Vector3.Distance(closestPoint1, closestPoint2);
			var carDist = Vector3.Distance(closestPoint1, _position);
			float ratio = Math.Min(carDist / dist, 1.0f);

			_up = Vector3.Lerp(CurrentNode.Up, nextNode.Up, ratio);
			_up = Vector3.Normalize(_up);
			_direction = Vector3.Cross(_up, CarRight);
			
			var height = _track.GetHeightAtPoint(CurrentNode, _position);
			if (height != -9999)  _position.Y = height;

			groundPlaneNormal = trackMatrix.Up;
			groundPlanePos = trackMatrix.Translation;
			
			UpdateCarMatrixAndCamera();

			UpdateWheels();

			#endregion
		}

		#endregion

		/// <summary>
		/// Resets the car at the center of the current track segment
		/// </summary>
		public void Reset()
		{
			_position = CurrentNode.Position + new Vector3(0, 5, 0);
			_direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(MathHelper.ToRadians(CurrentNode.Orientation)));
			_prevPosition = _position;
			_speed = 0;
			ScreenEffects.Instance.UnFadeScreen();
			return;
		}

		#region CheckForCollisions

		//float gravitySpeed = 0.0f;

		public void ApplyGravityAndCheckForCollisions(NeedForSpeed.Parsers.Track.Track track)
		{
			_wheelsOutsideRoad = VehicleFenceCollision.GetWheelsOutsideRoadVerge(this);

			_audioProvider.PlayOffRoad(_speed > 3 && _wheelsOutsideRoad > 0);
			if (_speed > 3 && _wheelsOutsideRoad > 0)
			{
				_wheels[2].IsSkidding = _wheels[3].IsSkidding = true;
			}

			VehicleFenceCollision.Handle(this);

			//ApplyGravity();
		}

		float _timeInAir = 0;
		private void ApplyGravity()
		{

			// Fix car on ground
			float distFromGround = Vector3Helper.SignedDistanceToPlane(_position, groundPlanePos, groundPlaneNormal);

			bool wasOnGround = _isOnGround;

			_isOnGround = distFromGround > -0.5f;  //underneath ground = on ground

			if (distFromGround > 0)
			{
				_position.Y += Math.Max(0.05f, distFromGround) * _moveFactorPerSecond * 7.0f;
				return;
			}

			if (_isOnGround && !wasOnGround)
			{
				if (_timeInAir > 10)
				{
					_audioProvider.HitGround();
				}
			}

			if (_isOnGround)
				_timeInAir = 0;
			else
				_timeInAir++;


			// Use more smooth gravity for jumping
			float minGravity = -Gravity * 1.3f * _moveFactorPerSecond;

			float fixedDist = distFromGround;

			if (fixedDist < minGravity)
			{
				fixedDist = minGravity;
			}

			_position.Y += fixedDist;
		}

		#endregion

		#region SetGuardRails

		protected Vector3 groundPlanePos, groundPlaneNormal;

		#endregion

		public void UpdateCarMatrixAndCamera()
		{
			Matrix orientation = Matrix.Identity;
			orientation.Right = CarRight;
			orientation.Up = _up;
			orientation.Forward = _direction;

			_renderMatrix =
					Matrix.CreateRotationX( _carPitchSpring.Position / 60) *
					Matrix.CreateRotationZ(-_carRollSpring.Position * 0.21f) *
					orientation *
					Matrix.CreateTranslation(_position);
		}


		public void RenderShadow()
		{
			// Shadow
			Vector3[] points = new Vector3[4];
			float y = -_wheels[0].Size / 2;
			float xoffset = 0.1f;
			points[0] = _wheels[0].GetOffsetPosition(new Vector3(-xoffset,y,-2));
			points[1] = _wheels[1].GetOffsetPosition(new Vector3(xoffset, y, -2));
			points[2] = _wheels[2].GetOffsetPosition(new Vector3(-xoffset, y, 3.5f));
			points[3] = _wheels[3].GetOffsetPosition(new Vector3(xoffset, y, 3.5f));

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

			TyreSmokeParticleSystem.Instance.Render();
		}

		protected void Gearbox_GearChanged(object sender, EventArgs e)
		{
			_audioProvider.ChangeGear();
		}
	}
}
