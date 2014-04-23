
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
				_wheels[0].TrackTriangleUnderWheel = _wheels[1].TrackTriangleUnderWheel = _wheels[2].TrackTriangleUnderWheel = _wheels[3].TrackTriangleUnderWheel = -1;
				_prevPosition = _position;
				_currentTrackTriangle = -1;
				_speed = 0;
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
		protected bool _isOnGround = false;

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

		public Vector3 Force
		{
			get { return _force; }
			set { _force = value; }
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

		private int _currentTrackTriangle;

		public int CurrentTrackTriangle
		{
			get { return _currentTrackTriangle; }
		}

		public int CurrentTrackNode { get; private set; }

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

		float _virtualRotationAmount = 0.0f;
		float _rotationChange = 0.0f;

		/// <summary>
		/// Update game logic for our car.
		/// </summary>
		public virtual void Update(GameTime gameTime)
		{
			GameConsole.WriteLine(_track.RoadNodes[CurrentTrackNode].Slope, 0);
			_moveFactorPerSecond = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 400;

			if (Engine.Instance.Input.WasPressed(Keys.R))
			{
				Reset();
			}
			if (Engine.Instance.Input.WasPressed(Keys.F2))
			{
				GameConfig.RenderOnlyPhysicalTrack = !GameConfig.RenderOnlyPhysicalTrack;
			}

			for (int i = 0; i < 2; i++)
				_wheels[i].Rotation += _moveFactorPerSecond * _speed;
			for (int i = 2; i < 4; i++)
				_wheels[i].Rotation += _moveFactorPerSecond * (_motor.WheelsSpinning ? 50 : _speed);

			float moveFactor = _moveFactorPerSecond;
			// Make sure this is never below 0.001f and never above 0.5f
			// Else our formulas below might mess up or carSpeed and carForce!
			if (moveFactor < 0.001f)
				moveFactor = 0.001f;
			if (moveFactor > 0.5f)
				moveFactor = 0.5f;

			float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

			#region Handle rotations

			// Left/right changes rotation
			float steeringSpeed = 0.9f;

			if (VehicleController.Turn < 0)
			{
				float extraForce = _steeringWheel > 0 ? 2 : 1;
				_steeringWheel += steeringSpeed * extraForce * elapsedSeconds * VehicleController.Turn;
				if (_steeringWheel < -MaxSteeringLock)
					_steeringWheel = -MaxSteeringLock;
			}
			else if (VehicleController.Turn > 0)
			{
				float extraForce = _steeringWheel < 0 ? 2 : 1;
				_steeringWheel += steeringSpeed * extraForce * elapsedSeconds * VehicleController.Turn;
				if (_steeringWheel > MaxSteeringLock)
					_steeringWheel = MaxSteeringLock;
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

			float maxRot = MaxRotationPerSec * moveFactor;

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

			_virtualRotationAmount += _rotationChange;
			// Smooth over 200ms
			float interpolatedRotationChange = (_rotationChange + _virtualRotationAmount) * moveFactor / 0.225f;
			_virtualRotationAmount -= interpolatedRotationChange;


			_direction = Vector3.TransformNormal(_direction, Matrix.CreateFromAxisAngle(_up, interpolatedRotationChange));

			#endregion


			#region Handle speed

			//GameConsole.WriteLine(Math.Round(_motor.Rpm * 1000), 5);

			float newAccelerationForce = 0.0f;

			_motor.Throttle = VehicleController.Acceleration;
			newAccelerationForce += _motor.CurrentPowerOutput * 30;

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
				wheel.Update(gameTime);

			TyreSmokeParticleSystem.Instance.Update(gameTime);
			TyreSmokeParticleSystem.Instance.SetCamera(Engine.Instance.Camera);

			_motor.Update(gameTime, _speed);


			if (_motor.AtRedline && !_motor.WheelsSpinning)
			{
				_force *= 0.2f;
			}

			if (_motor.Throttle == 0 && Math.Abs(_speed) < 1)
			{
				_speed = 0;
			}

			//GameConsole.WriteLine(_motor.Gearbox.CurrentGear, 6);

			// Add acceleration force to total car force, but use the current carDir!
			if (_isOnGround)
				_force += _direction * newAccelerationForce * (moveFactor) * 1f;

			// Change speed with standard formula, use acceleration as our force
			float oldSpeed = _speed;
			Vector3 speedChangeVector = _force / _mass;
			// Only use the amount important for our current direction (slower rot)
			if (_isOnGround && speedChangeVector.Length() > 0)
			{
				float speedApplyFactor = Vector3.Dot(Vector3.Normalize(speedChangeVector), _direction);
				if (speedApplyFactor > 1)
					speedApplyFactor = 1;
				_speed += speedChangeVector.Length() * speedApplyFactor;
			}

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
				float speedChange = _speed - oldSpeed;

				_carPitchSpring.ChangePosition(speedChange * -0.6f);
				_carRollSpring.ChangePosition(_steeringWheel * -0.05f * Math.Min(1, _speed / 30));

				_carPitchSpring.Simulate(moveFactor);
				_carRollSpring.Simulate(moveFactor);
			}
			else
			{
				//air drag
				_speed -= (_mass * 0.001f * elapsedSeconds);
			}

			_position += _speed * _direction * moveFactor * 1.5f;


			#endregion


			_audioProvider.UpdateEngine();

			#region Update track position and handle physics

			Matrix trackMatrix = Matrix.Identity;

			var nextNode = _track.RoadNodes[CurrentTrackNode+1];
			if (!Utility.IsLeftOfLine(nextNode.GetLeftBoundary(), nextNode.GetRightBoundary(), Position))
			{
				CurrentTrackNode++;
				Debug.WriteLine("passed node - new node " + CurrentTrackNode);
			}

			Vector3 hitLocation, avgHit = Vector3.Zero, avgNormal = Vector3.Zero;
			float hitFl;
			int hits = 0;
			int trianglesToLookAt = 100;

			if (_wheels[0].TrackTriangleUnderWheel == -1)
				trianglesToLookAt = _track.PhysicalRoadTriangles.Count;

			//Debug.WriteLine(String.Format("{0} {1} {2} {3}", _wheels[0].TrackTriangleUnderWheel,_wheels[1].TrackTriangleUnderWheel, _wheels[2].TrackTriangleUnderWheel, _wheels[3].TrackTriangleUnderWheel));
			//Debug.WriteLine(String.Format("{0},{1},{2}", _up.X.ToString("0.00"), _up.Y.ToString("0.00"), _up.Z.ToString("0.00")));
			foreach (VehicleWheel wheel in _wheels)
			{
				//var wheel = _wheels[0];
				Vector3 pos = wheel.WorldPosition + new Vector3(0, 5, 0);
				
				int startTriangle = wheel.TrackTriangleUnderWheel - 30;
				if (startTriangle < 0)
					startTriangle = _track.PhysicalRoadTriangles.Count + startTriangle;

				for (int i = 0; i < trianglesToLookAt; i++)
				{
					int tri = startTriangle + i;
					if (tri >= _track.PhysicalRoadTriangles.Count)
						tri -= _track.PhysicalRoadTriangles.Count;

					Triangle triangle = _track.PhysicalRoadTriangles[tri];
					if (Utility.FindRayTriangleIntersection(ref pos, Vector3.Down, 500, ref triangle.V1, ref triangle.V2, ref triangle.V3, out hitLocation, out hitFl))
					{
						wheel.TrackPositionUnderWheel = hitLocation;
						wheel.TrackTriangleUnderWheel = tri;
						var normal = Vector3.Normalize(Vector3.Cross(triangle.V2 - triangle.V1, triangle.V3 - triangle.V1));
						avgHit += hitLocation;
						hits++;
						
						Engine.Instance.GraphicsUtils.AddLine(triangle.V1, triangle.V1 + normal * 40, Color.Yellow);
						Engine.Instance.GraphicsUtils.AddLine(triangle.V2, triangle.V2 + normal * 40, Color.Yellow);
						Engine.Instance.GraphicsUtils.AddLine(triangle.V3, triangle.V3 + normal * 40, Color.Yellow);
						break;
					}
				}
			}

			//var t = _track.PhysicalRoadTriangles[_wheels[0].TrackTriangleUnderWheel];
			//var n1 = Vector3.Cross(t.V2 - t.V1, t.V3 - t.V1);
			//var t2 = _track.PhysicalRoadTriangles[_wheels[1].TrackTriangleUnderWheel];
			//var n2 = Vector3.Cross(t2.V2 - t2.V1, t2.V3 - t2.V1);

			//GameConsole.WriteLine(Math.Abs(n1.Y - n2.Y), 0);
			//GameConsole.WriteLine(_wheels[0].TrackPositionUnderWheel.Y - _wheels[1].TrackPositionUnderWheel.Y, 1);
			//GameConsole.WriteLine(_wheels[0].TrackTriangleUnderWheel + ", " + _wheels[1].TrackTriangleUnderWheel, 2);
			

			_allWheelsOnTrack = hits == 4;
			if (!_allWheelsOnTrack)
			{
			}


			var normal2 = Vector3.Cross(_wheels[1].TrackPositionUnderWheel - _wheels[0].TrackPositionUnderWheel,
				_wheels[2].TrackPositionUnderWheel - _wheels[0].TrackPositionUnderWheel);
			var normal3 = Vector3.Cross(_wheels[3].TrackPositionUnderWheel - _wheels[0].TrackPositionUnderWheel,
				_wheels[2].TrackPositionUnderWheel - _wheels[0].TrackPositionUnderWheel);
			//Debug.WriteLine(_wheels[0].TrackPositionUnderWheel);
			trackMatrix.Up = Vector3.Normalize(avgNormal / hits);
			trackMatrix.Translation = avgHit / hits;

			trackMatrix.Up = Vector3.Normalize(normal2);

			Vector3 prevRightVector = CarRight;
			_upVectors.AddValue(trackMatrix.Up);
			_up = Vector3.Normalize(_upVectors.GetAveragedValue());
			//normal2 += normal3;
			//normal2 /= 4;

			_up = trackMatrix.Up;

			_direction = Vector3.Cross(_up, CarRight);

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
			_prevPosition = new Vector3(-2.191055f, -1195.377f, -6288.538f);
			int roadNode = _track.GetRoadSegment(_prevPosition, 0);
			_position = _prevPosition; // _track.RoadNodes[roadNode].Position + new Vector3(0, 5, 0);
			_direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(MathHelper.ToRadians(_track.RoadNodes[roadNode].Orientation + 20)));
			_wheels[0].TrackTriangleUnderWheel = _wheels[1].TrackTriangleUnderWheel = _wheels[2].TrackTriangleUnderWheel = _wheels[3].TrackTriangleUnderWheel = -1;
			_currentTrackTriangle = -1;
			_prevPosition = _position;
			_speed = 0;
			//ScreenEffects.Instance.UnFadeScreen();
			return;
		}

		#region CheckForCollisions

		//float gravitySpeed = 0.0f;

		public void ApplyGravityAndCheckForCollisions(NeedForSpeed.Parsers.Track.Track track)
		{
			int triangle = track.GetRoadTriangle(_position, _currentTrackTriangle);
			if (triangle == -1)
				triangle = track.GetRoadTriangle(_prevPosition, _currentTrackTriangle);
			else
				_prevPosition = _position;

			_currentTrackTriangle = triangle;

			// TODO: TerrainSegment should not contain the road verge
			//TerrainSegment segment = track.TerrainSegments[triangle / TrackAssembler.TRIANGLES_PER_SEGMENT];

			//CurrentTrackNode = triangle / TrackAssembler.TRIANGLES_PER_ROW;
			//int segmentRow = CurrentTrackNode % 4;

			//if (segment.LeftBoundary[0].Normal == Vector3.Zero)
			//{
			//	for (int i = 0; i < 2; i++)
			//	{
			//		segment.LeftBoundary[i].Normal = Vector3.Cross(Vector3.Normalize(segment.LeftBoundary[i].Point2 - segment.LeftBoundary[i].Point1), groundPlaneNormal);
			//		segment.RightBoundary[i].Normal = Vector3.Cross(groundPlaneNormal, Vector3.Normalize(segment.RightBoundary[i].Point2 - segment.RightBoundary[i].Point1));

			//		segment.LeftVerge[i].Normal = Vector3.Cross(Vector3.Normalize(segment.LeftVerge[i].Point2 - segment.LeftVerge[i].Point1), groundPlaneNormal);
			//		segment.RightVerge[i].Normal = Vector3.Cross(groundPlaneNormal, Vector3.Normalize(segment.RightVerge[i].Point2 - segment.RightVerge[i].Point1));
			//	}
			//}

			//VehicleFenceCollision.Handle(this, _moveFactorPerSecond, segment, segmentRow);
			//_wheelsOutsideRoad = VehicleFenceCollision.GetWheelsOutsideRoadVerge(this, segment, segmentRow);
			//_audioProvider.PlayOffRoad(_speed > 3 && _wheelsOutsideRoad > 0);
			//if (_speed > 3 && _wheelsOutsideRoad > 0)
			//{
			//	_wheels[2].IsSkidding = _wheels[3].IsSkidding = true;
			//}

			ApplyGravity();
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
			// Get car matrix with help of the current car position, dir and up
			Matrix carMatrix = Matrix.Identity;
			carMatrix.Right = CarRight;
			carMatrix.Up = _up;
			carMatrix.Forward = -_direction;

			_renderMatrix =

					Matrix.CreateRotationX(MathHelper.Pi / 2.0f - _carPitchSpring.Position / 60) *
					Matrix.CreateRotationZ(MathHelper.Pi) *
					Matrix.CreateRotationX(MathHelper.ToRadians(-90)) *
					Matrix.CreateRotationZ(-_carRollSpring.Position * 0.21f) *
					carMatrix *
					Matrix.CreateTranslation(_position + (Vector3.Up * _bodyRideHeight));
		}

		public void UpdateWheels()
		{
			_wheels[0].Steer(_steeringWheel);
			_wheels[1].Steer(_steeringWheel);
		}

		public void RenderShadow()
		{
			// Shadow
			Vector3[] points = new Vector3[4];
			float y = -_wheels[0].Size / 2;
			Vector3 offset2 = new Vector3(0.5f, 0, 0.5f);
			points[1] = _wheels[0].GetOffsetPosition(new Vector3(-2.5f, y, 10)) + offset2;
			points[0] = _wheels[1].GetOffsetPosition(new Vector3(2.5f, y, 10)) + offset2;
			points[3] = _wheels[2].GetOffsetPosition(new Vector3(-2.5f, y, -12)) + offset2;
			points[2] = _wheels[3].GetOffsetPosition(new Vector3(2.5f, y, -12)) + offset2;

			points[0].Y -= (_wheels[0].BottomPosition.Y - _wheels[0].TrackPositionUnderWheel.Y);
			points[1].Y -= (_wheels[1].BottomPosition.Y - _wheels[1].TrackPositionUnderWheel.Y);
			points[2].Y -= (_wheels[2].BottomPosition.Y - _wheels[2].TrackPositionUnderWheel.Y);
			points[3].Y -= (_wheels[3].BottomPosition.Y - _wheels[3].TrackPositionUnderWheel.Y);

			ObjectShadow.Render(points);
		}

		public virtual void Render()
		{
			/*
			WheelModel.BeginBatch();
			foreach (VehicleWheel wheel in _wheels)
			{
				wheel.Render();
			}
			*/

			foreach (VehicleWheel wheel in _wheels)
			{
				Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
					Matrix.CreateTranslation(wheel.TrackPositionUnderWheel - new Vector3(0, 0, 0)), Color.Yellow,
					null);
			}

			Engine.Instance.GraphicsUtils.AddLine(_wheels[2].TrackPositionUnderWheel, _wheels[3].TrackPositionUnderWheel, Color.Green);
			Engine.Instance.GraphicsUtils.AddLine(_wheels[0].TrackPositionUnderWheel, _wheels[1].TrackPositionUnderWheel, Color.Green);

			Matrix carMatrix = Matrix.Identity;
			carMatrix.Right = CarRight;
			carMatrix.Up = _up;
			carMatrix.Forward = -_direction;

			Engine.Instance.GraphicsUtils.AddSolidShape(ShapeType.Cube,
					Matrix.CreateScale(2, 100, 2) * carMatrix *
					Matrix.CreateTranslation(_position + (Vector3.Up * -10))
					, Color.Yellow,
					null);
			
			TyreSmokeParticleSystem.Instance.Render();
		}

		protected void Gearbox_GearChanged(object sender, EventArgs e)
		{
			_audioProvider.ChangeGear();
		}

		public abstract string Name { get; }
		public abstract string Description { get; }

	}
}
