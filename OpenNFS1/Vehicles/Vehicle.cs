using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1;
using OpenNFS1.Dashboards;
using OpenNFS1.Physics;
using OpenNFS1.Vehicles;
using NfsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenNFS1.Parsers.Track;
using OpenNFS1.Tracks;
using System.Diagnostics;

namespace OpenNFS1.Vehicles
{
	class Vehicle
	{
		public const float MaxSteeringLock = 0.3f;
		const float Gravity = 9.81f;

		public float SteeringSpeed = 2.1f;
		public Track Track { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Direction { get; set; }
		public Vector3 Up { get; set; }
		public float Speed { get; set; }
		public TrackNode CurrentNode { get; private set; }
		public float TrackProgress { get; set; }
		

		protected CarMesh _model;
		private AlphaTestEffect _effect;
		public float _steeringWheel;

		// inputs
		public float SteeringInput;
		

		protected float _previousSpeed;
		protected float _currentHeightOfTrack;
		protected float _rotationChange = 0.0f;
		protected bool _isOnGround = true;
		float _upVelocity = 0;
		float _timeInAir = 0;
		

		public Vector3 Right
		{
			get { return Vector3.Cross(Direction, Up); }
		}

		public Vehicle(string cfmFile)
		{
			_model = CarModelCache.GetCfm(cfmFile);
			_effect = new AlphaTestEffect(Engine.Instance.Device);
			Direction = Vector3.Forward;
			Up = Vector3.Up;
		}

		public void PlaceOnTrack(Track t, TrackNode startNode)
		{
			Track = t;
			CurrentNode = startNode;
			Position = Vector3.Lerp(startNode.GetLeftVerge(), startNode.GetRightVerge2(), 0.5f);  //center of node
		}

		public virtual void Update()
		{
			float elapsedSeconds = Engine.Instance.FrameTime;
			Direction = Vector3.TransformNormal(Direction, Matrix.CreateFromAxisAngle(Up, _rotationChange));			
			Position += Speed * Direction * Engine.Instance.FrameTime * 2.5f;

			UpdateSteering();
			UpdateTrackNode();
			TrackNode node = null, nextNode = null;

			if (CurrentNode.Next == null) return;

			if (Distance2d(Position, CurrentNode.Next.Position) < Distance2d(Position, CurrentNode.Prev.Position))
			{
				node = CurrentNode;
				nextNode = CurrentNode.Next;
			}
			else
			{
				node = CurrentNode.Prev;
				nextNode = CurrentNode;
			}

			FollowTrackOrientation(node, nextNode);
			ApplyGravity();
		}

		private void UpdateTrackNode()
		{
			var nextNode = CurrentNode.Next;
			var prevNode = CurrentNode.Prev;
			if (!Utility.IsLeftOfLine(nextNode.GetLeftBoundary(), nextNode.GetRightBoundary(), Position))
			{
				CurrentNode = CurrentNode.Next;
				//Debug.WriteLine("passed node - new node " + CurrentNode.Number);
			}
			else if (prevNode != null && Utility.IsLeftOfLine(prevNode.GetLeftBoundary(), prevNode.GetRightBoundary(), Position))
			{
				CurrentNode = prevNode;
				//Debug.WriteLine("passed node (back) - new node " + CurrentNode.Number);
			}
		}

		private void FollowTrackOrientation(TrackNode node, TrackNode nextNode)
		{
			var closestPoint1 = Utility.GetClosestPointOnLine(node.GetLeftBoundary(), node.GetRightBoundary(), Position);
			var closestPoint2 = Utility.GetClosestPointOnLine(nextNode.GetLeftBoundary(), nextNode.GetRightBoundary(), Position);

			var dist = Distance2d(closestPoint1, closestPoint2);
			var carDist = Distance2d(closestPoint1, Position);
			float ratio = Math.Min(carDist / dist, 1.0f);
			TrackProgress = CurrentNode.Number + ratio;

			// if the road is sloping downwards and we have enough speed, unstick from ground
			if (node.Slope - nextNode.Slope > 50 && Speed > 100 && _isOnGround)
			{
				_isOnGround = false;
				_upVelocity = -0.4f;
			}

			if (_isOnGround)
			{
				Up = Vector3.Lerp(node.Up, nextNode.Up, ratio);
				Up = Vector3.Normalize(Up);
				Direction = Vector3.Cross(Up, Right);
			}

			_currentHeightOfTrack = MathHelper.Lerp(closestPoint1.Y, closestPoint2.Y, ratio);
			if (_currentHeightOfTrack == -9999)
			{
				throw new Exception();
			}
			if (_isOnGround)
			{
				var newPosition = Position;
				newPosition.Y = _currentHeightOfTrack;
				Position = newPosition;
			}
			//GameConsole.WriteLine("height: " + _position.Y, 0);
			//GameConsole.WriteLine("ratio: " + ratio, 1);
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
				if (_steeringWheel > 0.01f)
					_steeringWheel -= SteeringSpeed * elapsedSeconds * 0.9f;
				else if (_steeringWheel < -0.01f)
					_steeringWheel += SteeringSpeed * elapsedSeconds * 0.9f;
				else
					_steeringWheel = 0;
			}
			GameConsole.WriteLine("steering: " + _steeringWheel, 11);
			_rotationChange = _steeringWheel * 0.05f;
			if (Speed > 0)
				_rotationChange *= -1;

			HandleExtraSteeringPhysics();
		}

		public virtual void HandleExtraSteeringPhysics() { }

		
		private void ApplyGravity()
		{
			if (_isOnGround) return;

			bool wasOnGround = _isOnGround;

			_isOnGround = Position.Y < _currentHeightOfTrack;

			if (!_isOnGround)
			{
				var newPosition = Position;
				newPosition.Y -= Gravity * 10f * _timeInAir * Engine.Instance.FrameTime;
				Position = newPosition;
				// slowly pitch the nose of the car downwards - helps to flatten out the jump and looks better
				if (_timeInAir > 0.3f && Direction.Y > -0.3f)
				{
					var newDirection = Direction;
					newDirection.Y -= _timeInAir * 0.006f;
					Direction = Vector3.Normalize(newDirection);
				}
			}

			if (_isOnGround && !wasOnGround)
			{
				if (_timeInAir > 0.2f)
				{
					OnGroundHit();
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

		public virtual void OnGroundHit() { }

		public void Reset()
		{
			Position = CurrentNode.Position;
			Direction = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(MathHelper.ToRadians(CurrentNode.Orientation)));
			Speed = 0;
		}

		public virtual void Render()
		{
			_effect.View = Engine.Instance.Camera.View;
			_effect.Projection = Engine.Instance.Camera.Projection;

			_effect.World = GetRenderMatrix();

			Engine.Instance.Device.RasterizerState = RasterizerState.CullNone;
			Engine.Instance.Device.BlendState = BlendState.Opaque;
			_effect.CurrentTechnique.Passes[0].Apply();
			_model.Render(_effect, false);
		}


		public virtual Matrix GetRenderMatrix()
		{
			Matrix orientation = Matrix.Identity;
			orientation.Right = Right;
			orientation.Up = Up;
			orientation.Forward = Direction;
			return orientation * Matrix.CreateTranslation(Position);
		}

		public static float Distance2d(Vector3 pos1, Vector3 pos2)
		{
			pos1.Y = pos2.Y = 0;
			return Vector3.Distance(pos1, pos2);
		}
	}
}
