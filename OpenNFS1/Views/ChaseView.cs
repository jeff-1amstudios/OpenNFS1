using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NfsEngine;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.UI;
using NeedForSpeed.Physics;
using System.Diagnostics;

namespace NeedForSpeed.Views
{
    class ChaseView : IView
    {
        Vehicle _car;
        FixedChaseCamera _camera;
        SpeedoControl _speedoControl;
        
        public ChaseView(Vehicle car, int distance, int height, int offset)
        {
            _car = car;
            _camera = new FixedChaseCamera();
			_camera.FieldOfView = MathHelper.ToRadians(65);
            _camera.ChaseDistance = distance;
            _camera.ChaseHeight = height;
            _camera.ChaseOffset = offset;
            _speedoControl = new SpeedoControl();
        }

        #region IView Members

        public bool Selectable
        {
            get { return true; }
        }

        public void Activate()
        {
            Engine.Instance.Camera = _camera;
            _camera.Position = _car.Position;
            _camera.ChaseDirection = _car.Direction;
        }

        public void Deactivate()
        {
        }

        public void Update(GameTime gameTime)
        {
            _camera.Position = _car.Position;
			_camera.ChaseDirection = _car.Direction;
			_camera.UpVector = _car.UpVector;
        }

        public void Render()
        {
            _car.Render();
            _speedoControl.Render(_car.Motor.Rpm / _car.Motor.RedlineRpm);
        }

        #endregion

    }
}
