using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace NfsEngine
{
    public abstract class GameObject
    {

        protected Vector3 _position, _lastPosition, _size, _orientation;
        protected float _velocity;
        protected bool _visible;
        protected bool _lockToGround;
        
        public GameObject()
        {
            _visible = true;
        }

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

        public Vector3 Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public float Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public bool LockToGround
        {
            get { return _lockToGround; }
            set { _lockToGround = value; }
        }

        public void SetRotation(float rotation)
        {
            _orientation.X = rotation;
        }

        public void MoveForward()
        {
            _lastPosition = _position;
            _position.X -= (float)((Math.Sin(_orientation.X) * Math.Cos(_orientation.Y)) * _velocity);
            _position.Z -= (float)((Math.Cos(_orientation.X) * Math.Cos(_orientation.Y)) * _velocity);
            if (!_lockToGround)
                _position.Y -= _orientation.Y * _velocity;
        }

        public Vector3 GetLookAt(float distance)
        {
            Vector3 lookAt = _position;
            lookAt.X -= (float)((Math.Sin(_orientation.X) * Math.Cos(_orientation.Y)) * distance);
            lookAt.Z -= (float)((Math.Cos(_orientation.X) * Math.Cos(_orientation.Y)) * distance);
            lookAt.Y -= _orientation.Y * distance;
            return lookAt;
        }

        public Vector3 GetLookAt(Vector3 orientation, float distance)
        {
            Vector3 lookAt = _position;
            lookAt.X -= (float)((Math.Sin(orientation.X) * Math.Cos(orientation.Y)) * distance);
            lookAt.Z -= (float)((Math.Cos(orientation.X) * Math.Cos(orientation.Y)) * distance);
            lookAt.Y -= orientation.Y * distance;
            return lookAt;
        }

        public void Strafe(float amount)
        {
            _position.X += (float)(Math.Cos(_orientation.X) * amount);
            _position.Z -= (float)(Math.Sin(_orientation.X) * amount);
        }

        public Matrix WorldTransform
        {
            get
            {
                Matrix world = Matrix.CreateFromYawPitchRoll(_orientation.X, -_orientation.Y, _orientation.Z);
                world *= Matrix.CreateScale(_size);
                world *= Matrix.CreateTranslation(_position);
                return world;
            }
        }
      
        
        /// <summary>
        /// Moves the camera. The dx, dy, and dz parameters determine how
        /// far to move the camera forwards, upwards, and sideways.
        /// </summary>
        /// <param name="dx">Sideways movement amount.</param>
        /// <param name="dy">Upwards movement amount.</param>
        /// <param name="dz">Forwads movement amount.</param>
        public void Move(float dx, float dy, float dz)
        {
            _position.X += dx;
            _position.Y += dy;
            _position.Z += dz;
        }

        /// <summary>
        /// Moves the camera along the given direction.
        /// </summary>
        /// <param name="direction">The direction to move.</param>
        /// <param name="velocity">How far to move along direction.</param>
        public void Move(Vector3 direction, float amount)
        {
            _position += direction * amount;
        }

        public void Rotate(float amount)
        {
            _orientation.X += amount;
        }

        public void Pitch(float amount)
        {
            _orientation.Y += amount;
        }
        

        public virtual Vector3 GetCameraPosition()
        {
            return _position;
        }

        public BoundingSphere BoundingSphere
        {
            get
            {
                return new BoundingSphere(_position, _size.X);
            }
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Render();

        public virtual void OnPlayerSelect() { }

    }
}
