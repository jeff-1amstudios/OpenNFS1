using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NfsEngine;

namespace NfsEngine
{
    /// <summary>
    /// Camera that stays a fixed distance behind an object but swings freely
    /// </summary>
    public class FixedChaseCamera : ICamera
    {
        public FixedChaseCamera()
		{
		}

        public Vector3 RightVec = Vector3.Right;
        public Vector3 UpVector = Vector3.Up;
        AverageValueVector3 _lookAt = new AverageValueVector3(40);

		
		/// <summary>
		/// Position of camera in world space.
		/// </summary>
		public Vector3 Position
		{
			get { return _position; }
			set { _position = value; }
		}

		private Vector3 _position;

        public Vector3 ChaseDirection
        {
            set {
                
                //_lookAt.AddValue(value);
                _chaseDirection = value;
            }
        }
        private Vector3 _chaseDirection;

        public float ChaseOffset { get; set; }

		
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
			get { return _view; }
		}
		private Matrix _view;

		/// <summary>
		/// Projecton transform matrix.
		/// </summary>
		public Matrix Projection
		{
			get { return _projection; }
		}
		private Matrix _projection;


		public void Update(GameTime gameTime)
		{
            _lookAt.AddValue(new Vector3(ChaseOffset, ChaseHeight, 0) + (-_chaseDirection * new Vector3(ChaseDistance, ChaseDistance, ChaseDistance)));
            Vector3 avgLookAt = _lookAt.GetAveragedValue();
            Vector3 cameraPosition = _position +avgLookAt;
            _view = Matrix.CreateLookAt(cameraPosition, cameraPosition - avgLookAt + new Vector3(0,13,0), UpVector);
            _projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, Engine.Instance.AspectRatio, NearPlaneDistance, FarPlaneDistance);
		}

		public void SetPosition(Vector3 position)
		{
			_position = position;
		}

		public void FollowObject(GameObject obj)
		{
		}

        public float ChaseDistance { get; set; }
        public float ChaseHeight { get; set; }

        
    }
}
