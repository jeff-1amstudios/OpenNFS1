using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using NfsEngine;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.UI;
using OpenNFS1.Physics;
using System.Diagnostics;
using OpenNFS1.Parsers;
using OpenNFS1.Parsers;

namespace OpenNFS1.Views
{
	

    class ChaseView : BaseExternalView, IView
    {
        Vehicle _car;
        FixedChaseCamera _camera;
		
		public bool ShouldRenderPlayer { get { return true; } }
        
        public ChaseView(Vehicle car, int distance, int height, int offset)
        {
            _car = car;
            _camera = new FixedChaseCamera();
			_camera.FieldOfView = GameConfig.FOV;
            _camera.ChaseDistance = distance;
            _camera.ChaseHeight = height;
            _camera.ChaseOffset = offset;
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
			RenderBackground(_car);
        }

        #endregion

    }
}
