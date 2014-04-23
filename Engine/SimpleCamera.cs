using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NfsEngine;

namespace NfsEngine
{

	public class SimpleCamera : ICamera
	{

		public SimpleCamera()
		{
		}

        public Vector3 RightVec = Vector3.Right;
        public Vector3 UpVector = Vector3.Up;

		/// <summary>
		/// Look at point in world space.
		/// </summary>
		public Vector3 LookAt
		{
			get
			{
				return lookAt;
			}
			set
			{
				lookAt = value;
			}
		}
		private Vector3 lookAt;


		/// <summary>
		/// Position of camera in world space.
		/// </summary>
		public Vector3 Position
		{
			get { return _position; }
			set { _position = value; }
		}
		private Vector3 _position;


		/// <summary>
		/// Perspective field of view.
		/// </summary>
		public float FieldOfView
		{
			get { return fieldOfView; }
			set { fieldOfView = value; }
		}
		private float fieldOfView = MathHelper.ToRadians(45.0f);

		/// <summary>
		/// Distance to the near clipping plane.
		/// </summary>
		public float NearPlaneDistance
		{
			get { return nearPlaneDistance; }
			set { nearPlaneDistance = value; }
		}
		private float nearPlaneDistance = 1.0f;

		/// <summary>
		/// Distance to the far clipping plane.
		/// </summary>
		public float FarPlaneDistance
		{
			get { return farPlaneDistance; }
			set { farPlaneDistance = value; }
		}
        private float farPlaneDistance = 15000.0f;


		/// <summary>
		/// View transform matrix.
		/// </summary>
		public Matrix View
		{
			get { return view; }
		}
		private Matrix view;

		/// <summary>
		/// Projecton transform matrix.
		/// </summary>
		public Matrix Projection
		{
			get { return projection; }
		}
		private Matrix projection;


		/// <summary>
		/// Animates the camera from its current position towards the desired offset
		/// behind the chased object. The camera's animation is controlled by a simple
		/// physical spring attached to the camera and anchored to the desired position.
		/// </summary>
		public void Update(GameTime gameTime)
		{
            view = Matrix.CreateLookAt(this.Position, this.LookAt, UpVector);
			projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView,
				Engine.Instance.AspectRatio, NearPlaneDistance, FarPlaneDistance);
		}

		public void SetPosition(Vector3 position)
		{
			_position = position;
		}

		public void FollowObject(GameObject obj)
		{
		} 
	}
}
