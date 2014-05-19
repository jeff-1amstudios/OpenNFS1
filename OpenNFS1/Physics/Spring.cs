using System;
using System.Collections;
using System.Text;


namespace OpenNFS1.Physics
{
    /// <summary>
    /// Simple 1D spring
    /// </summary>
    class Spring
    {
        private float _mass;
        private float _friction;
        private float _springConstant;
        private float _position = 0.0f;
        private float _velocity = 0.0f;
        private float _force = 0.0f;
        private float _maxValue;

        public float Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Create simple 1D spring with specified values.
        /// </summary>
        public Spring(float mass, float friction, float springConstant, float initialPosition, float maxValue)
        {
            _mass = mass;
            _friction = friction;
            _springConstant = springConstant;
            _position = initialPosition;
            _force = 0;
            _velocity = 0;
            _maxValue = maxValue;
        }

        
        public void Simulate(float timeChange)
        {
            // Calculate force again
            _force += -_position * _springConstant;
            // Calculate velocity
            _velocity = _force / _mass;
            // And apply it to the current position
            _position += timeChange * _velocity;
            // Apply friction
            _force *= 1.0f - (timeChange * _friction);
        }
        
        public void ChangePosition(float change)
        {
            _position += change;
            if (_position < 0 && _position < -_maxValue)
                _position = -_maxValue;
            else if (_position > 0 && _position > _maxValue)
                _position = _maxValue;
        }
    }
}
