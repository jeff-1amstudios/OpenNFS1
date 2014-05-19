using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using OpenNFS1.Parsers.Track;
using NfsEngine;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Physics;

namespace OpenNFS1.Views
{
	class TelevisionView : BaseExternalView, IView
    {
        ChaseCamera _camera = new ChaseCamera();
        Vehicle _car;

        public TelevisionView(Vehicle car)
        {
            _car = car;
			_camera.FieldOfView = GameConfig.FOV;
            _camera.DesiredPositionOffset = new Vector3(80.0f, 90.0f, 80.0f);
        }

        #region IView Members

        public bool Selectable
        {
            get { return false; }
        }

		public bool ShouldRenderPlayer { get { return true; } }

        public void Update(GameTime gameTime)
        {
            _camera.ChaseDirection = _car.Direction;
            _camera.ChasePosition = _car.Position;
        }

        public void Render()
        {
			RenderBackground(_car);
        }

        public void Activate()
        {
            Engine.Instance.Camera = _camera;
            _camera.ChasePosition = _car.Position;
            _camera.ChaseDirection = _car.Direction;
            _camera.Reset();
        }

        public void Deactivate()
        {
        }

        #endregion
    }
}
