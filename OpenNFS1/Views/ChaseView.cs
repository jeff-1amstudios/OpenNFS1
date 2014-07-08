using Microsoft.Xna.Framework;
using NfsEngine;
using OpenNFS1.Physics;

namespace OpenNFS1.Views
{
	

    class ChaseView : BaseExternalView, IView
    {
        DrivableVehicle _car;
        FixedChaseCamera _camera;
		
		public bool ShouldRenderPlayer { get { return true; } }
        
        public ChaseView(DrivableVehicle car, int distance, int height, int offset)
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
			_camera.UpVector = _car.Up;
        }

        public void Render()
        {
			RenderBackground(_car);
        }

        #endregion

    }
}
