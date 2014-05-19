using System;
using System.Collections.Generic;

using System.Text;
using Microsoft.Xna.Framework;

namespace OpenNFS1.Dashboards
{
    enum AnimationStatus
    {
        ToNeutral,
        ToX,
        ToY
    }

    class GearboxAnimation
    {
        List<Vector2> _gearPositions;
        public int Current, Next;
        Vector2 _lastPos, _currentPosition;
        float _amount = 0;
        AnimationStatus _status;
        float _waitAtEndOfAnimationTime;

        public GearboxAnimation()
        {
            _gearPositions = new List<Vector2>();
            _gearPositions.Add(new Vector2(35, 35));
            _gearPositions.Add(new Vector2(0, 0));
            _gearPositions.Add(new Vector2(-35, -35));
            _gearPositions.Add(new Vector2(-35, 35));
            _gearPositions.Add(new Vector2(0, -35));
            _gearPositions.Add(new Vector2(0, 35));
            _gearPositions.Add(new Vector2(35, -35));
            _gearPositions.Add(new Vector2(35, 35));

            _currentPosition = _gearPositions[2];
            Current = 2;
            Next = 2;
        }

        public Vector2 CurrentPosition
        {
            get { return _currentPosition; }
            set { _currentPosition = value; }
        }
        public bool IsAnimating
        {
            get { return Current != Next || _waitAtEndOfAnimationTime > 0; }
        }

        public void Update(GameTime gameTime)
        {
            if (Current != Next)
            {
                if (_currentPosition.Y != 0 && _status == AnimationStatus.ToNeutral)
                {
                    _currentPosition.Y = MathHelper.Lerp(_gearPositions[Current].Y, 0, _amount);
                    if (_amount >= 1)
                    {
                        _currentPosition.Y = 0;
                        _amount = 0;
                        _lastPos = _currentPosition;
                    }
                }
                else if (_currentPosition.X != _gearPositions[Next].X)
                {
                    _currentPosition.X = MathHelper.Lerp(_lastPos.X, _gearPositions[Next].X, _amount);
                    if (_amount >= 1)
                    {
                        _currentPosition.X = _gearPositions[Next].X;
                        _amount = 0;
                        _lastPos = _currentPosition;
                    }
                }

                else if (_currentPosition.Y != _gearPositions[Next].Y)
                {
                    _status = AnimationStatus.ToY;
                    _currentPosition.Y = MathHelper.Lerp(_lastPos.Y, _gearPositions[Next].Y, _amount);
                    if (_amount >= 1)
                    {
                        _currentPosition.Y = _gearPositions[Next].Y;
                        _amount = 0;
                    }
                }
                else if (_currentPosition.X == _gearPositions[Next].X
                    && _currentPosition.Y == _gearPositions[Next].Y)
                {
                    _status = AnimationStatus.ToNeutral;
                    Current = Next;
                    _waitAtEndOfAnimationTime = 0.5f;
                }

                _amount += (float)gameTime.ElapsedGameTime.TotalSeconds * (_gearPositions[Current].X != _gearPositions[Next].X ? 7.5f : 5.5f);
            }

            if (_waitAtEndOfAnimationTime >= 0)
                _waitAtEndOfAnimationTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
