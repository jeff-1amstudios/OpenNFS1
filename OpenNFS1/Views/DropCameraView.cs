using System;
using Microsoft.Xna.Framework;
using NfsEngine;
using OpenNFS1.Physics;
using OpenNFS1.Tracks;

namespace OpenNFS1.Views
{
	class DropCameraView : BaseExternalView, IView
	{
		const int MaxCameraDistance = 15;
		DrivableVehicle _car;
		SimpleCamera _camera;
		TrackNode _cameraNode;

		public DropCameraView(DrivableVehicle car)
		{
			_car = car;
			_camera = new SimpleCamera();
			_camera.FieldOfView = GameConfig.FOV;
			_camera.FarPlaneDistance = GameConfig.DrawDistance;
			PositionCameraAtNode(car.Track.RoadNodes[10]);
		}

		#region IView Members

		public bool Selectable
		{
			get { return true; }
		}

		public bool ShouldRenderPlayer { get { return true; } }

		public void Activate()
		{
			Engine.Instance.Camera = _camera;
		}

		public void Deactivate()
		{
		}

		public void Update(GameTime gameTime)
		{
			if (Math.Abs(_car.CurrentNode.Number - _cameraNode.Number) > MaxCameraDistance)
			{
				int nextNode = (_car.CurrentNode.Number + MaxCameraDistance) % _car.Track.RoadNodes.Count;
				PositionCameraAtNode(_car.Track.RoadNodes[nextNode]);
			}
			_camera.LookAt = _car.Position;
		}

		private void PositionCameraAtNode(TrackNode node)
		{
			_cameraNode = node;
			_camera.Position = Engine.Instance.Random.Next() % 2 == 0 ? _cameraNode.GetLeftBoundary() : _cameraNode.GetRightBoundary();
			_camera.Position = _camera.Position + new Vector3(0, Engine.Instance.Random.Next(15, 50), 0);
		}

		public void Render()
		{
			RenderBackground(_car);
		}

		#endregion
	}
}
