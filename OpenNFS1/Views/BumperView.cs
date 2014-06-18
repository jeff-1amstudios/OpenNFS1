using Microsoft.Xna.Framework;
using NfsEngine;
using OpenNFS1.Physics;

namespace OpenNFS1.Views
{
	class BumperView : BaseExternalView, IView
	{
		Vehicle _car;
        SimpleCamera _camera;

		public BumperView(Vehicle car)
        {
            _car = car;
			_camera = new SimpleCamera();
			_camera.FieldOfView = GameConfig.FOV;
        }

        #region IView Members

        public bool Selectable
        {
            get { return true; }
        }

		public bool ShouldRenderPlayer { get { return false; } }

        public void Activate()
        {
            Engine.Instance.Camera = _camera;
        }

        public void Deactivate()
        {
        }

        public void Update(GameTime gameTime)
        {
            _camera.Position = _car.Position + _car.Direction * 2.8f + new Vector3(0, 5, 0);
            _camera.LookAt = _camera.Position + _car.Direction * 60f + new Vector3(0, _car.BodyPitch.Position, 0);
            _camera.UpVector = _car.UpVector; // +new Vector3(_car.Roll.Position * 0.2f, 0, 0);
            //float f = 1.8f; // _car.Roll.Position;
            //_camera.UpVector = _car.UpVector + _car.Direction * new Vector3(f, f, f);
            
        }

        public void Render()
        {
			RenderBackground(_car);
        }

        #endregion
	}
}
