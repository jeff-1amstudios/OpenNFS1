using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NfsEngine;
using System.Diagnostics;

namespace NfsEngine
{
    
    public class ChaseCamera : ICamera
    {
		private Vector3 _chasePosition;
		private Vector3 _chaseDirection = new Vector3(0, 0, -1);
		private Vector3 _up = Vector3.Up;
		private Vector3 _desiredPositionOffset = new Vector3(0, 2.0f, 2.0f);
		private Vector3 _desiredPosition;
		private Vector3 _lookAtOffset = new Vector3(0, 2.8f, 0);
		private Vector3 _lookAt;
		private float _stiffness = 1800.0f;
		private float _zstiffness = 2.0f;
		private float _damping = 600.0f;
		private float _mass = 50.0f;
		private Vector3 _position;
		private Vector3 _velocity;
		private float _fieldOfView = MathHelper.ToRadians(45.0f);
		private float _nearPlaneDistance = 1.0f;
		private float _farPlaneDistance = 15000.0f;
		private Matrix _view;
		private Matrix _projection;

        
        /// <summary>
        /// Position of object being chased.
        /// </summary>
        public Vector3 ChasePosition
        {
            get { return _chasePosition; }
            set { _chasePosition = value; }
        }        

        public void FollowObject(GameObject obj)
        {
            _chasePosition = obj.Position;
            _chaseDirection = obj.Orientation;
        }

        /// <summary>
        /// Direction the chased object is facing.
        /// </summary>
        public Vector3 ChaseDirection
        {
            get { return _chaseDirection; }
            set { _chaseDirection = value; }
        }        

        /// <summary>
        /// Chased object's Up vector.
        /// </summary>
        public Vector3 Up
        {
            get { return _up; }
            set { _up = value; }
        }

        /// <summary>
        /// Desired camera position in the chased object's coordinate system.
        /// </summary>
        public Vector3 DesiredPositionOffset
        {
            get { return _desiredPositionOffset; }
            set { _desiredPositionOffset = value; }
        }
        
        /// <summary>
        /// Desired camera position in world space.
        /// </summary>
        public Vector3 DesiredPosition
        {
            get
            {
                // Ensure correct value even if update has not been called this frame
                UpdateWorldPositions();

                return _desiredPosition;
            }
        }
		
        /// <summary>
        /// Look at point in the chased object's coordinate system.
        /// </summary>
        public Vector3 LookAtOffset
        {
            get { return _lookAtOffset; }
            set { _lookAtOffset = value; }
        }
		
        /// <summary>
        /// Look at point in world space.
        /// </summary>
        public Vector3 LookAt
        {
            get
            {
                // Ensure correct value even if update has not been called this frame
                UpdateWorldPositions();

                return _lookAt;
            }
        }
		
        /// <summary>
        /// Physics coefficient which controls the influence of the camera's position
        /// over the spring force. The stiffer the spring, the closer it will stay
        /// the chased object.
        /// </summary>
        public float Stiffness
        {
            get { return _stiffness; }
            set { _stiffness = value; }
        }
		
		/// <summary>
		/// Controls how hard the camera tries to keep up with the chased object 
		/// </summary>
        public float ZStiffness
        {
            get { return _zstiffness; }
            set { _zstiffness = value; }
        }
		
        /// <summary>
        /// Physics coefficient which approximates internal friction of the spring.
        /// Sufficient damping will prevent the spring from oscillating infinitely.
        /// </summary>
        public float Damping
        {
            get { return _damping; }
            set { _damping = value; }
        }
		
        /// <summary>
        /// Mass of the camera body. Heaver objects require stiffer springs with less
        /// damping to move at the same rate as lighter objects.
        /// </summary>
        public float Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }
		

        /// <summary>
        /// Position of camera in world space.
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
        }
		
        /// <summary>
        /// Velocity of camera.
        /// </summary>
        public Vector3 Velocity
        {
            get { return _velocity; }
        }
		

        #region Perspective properties

		
        /// <summary>
        /// Perspective field of view.
        /// </summary>
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set { _fieldOfView = value; }
        }
		
        /// <summary>
        /// Distance to the near clipping plane.
        /// </summary>
        public float NearPlaneDistance
        {
            get { return _nearPlaneDistance; }
            set { _nearPlaneDistance = value; }
        }
		
        /// <summary>
        /// Distance to the far clipping plane.
        /// </summary>
        public float FarPlaneDistance
        {
            get { return _farPlaneDistance; }
            set { _farPlaneDistance = value; }
        }
		
        #endregion


        /// <summary>
        /// View transform matrix.
        /// </summary>
        public Matrix View
        {
            get { return _view; }
        }
		
        /// <summary>
        /// Projecton transform matrix.
        /// </summary>
        public Matrix Projection
        {
            get { return _projection; }
        }


        /// <summary>
        /// Rebuilds object space values in world space. Invoke before publicly
        /// returning or privately accessing world space values.
        /// </summary>
        private void UpdateWorldPositions()
        {
            Matrix transform = Matrix.Identity;
            transform.Forward = ChaseDirection;
            transform.Up = Up;
            transform.Right = Vector3.Cross(Up, ChaseDirection);

            // Calculate desired camera properties in world space
            _desiredPosition = ChasePosition + Vector3.TransformNormal(DesiredPositionOffset, transform);
            _lookAt = ChasePosition + Vector3.TransformNormal(LookAtOffset, transform);
        }

        /// <summary>
        /// Rebuilds camera's view and projection matricies.
        /// </summary> 
        private void UpdateMatrices()
        {
            _view = Matrix.CreateLookAt(this.Position, this.LookAt, this.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView,
                Engine.Instance.AspectRatio, NearPlaneDistance, FarPlaneDistance);
        }

        /// <summary>
        /// Forces camera to be at desired position and to stop moving. The is useful
        /// when the chased object is first created or after it has been teleported.
        /// Failing to call this after a large change to the chased object's position
        /// will result in the camera quickly flying across the world.
        /// </summary>
        public void Reset()
        {
            UpdateWorldPositions();

            // Stop motion
            _velocity = Vector3.Zero;

            // Force desired position
            _position = _desiredPosition;

            UpdateMatrices();
        }

        /// <summary>
        /// Animates the camera from its current position towards the desired offset
        /// behind the chased object. The camera's animation is controlled by a simple
        /// physical spring attached to the camera and anchored to the desired position.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            
            UpdateWorldPositions();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate spring force
            Vector3 stretch = (_position - _desiredPosition);
            
            Vector3 force = -_stiffness * stretch - _damping * _velocity;

            // Apply acceleration
            Vector3 acceleration = force / _mass;
            _velocity += acceleration * elapsed;

            // Apply velocity
            _position += _velocity * elapsed;

			// Keep up with chased object
            //if (Vector3.Distance(_chasePosition, _position) > 50)
            //{

            //_position += _chaseDirection * elapsed * (Vector3.Distance(_chasePosition, _position)) * _zstiffness;
            
            //}

            UpdateMatrices();
        }

        public void SetPosition(Vector3 position)
        {
            _position = position;
        }
    }
}
