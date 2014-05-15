using System;
using System.Collections.Generic;

using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using NeedForSpeed.Parsers;
using Microsoft.Xna.Framework.Graphics;
using NfsEngine;
using Microsoft.Xna.Framework.Input;
using NeedForSpeed.Physics;
using NeedForSpeed.Vehicles;
using Microsoft.Xna.Framework.Media;

namespace NeedForSpeed.UI.Screens
{
	class ChooseCarScreen : BaseUIScreen, IGameScreen
	{
		SimpleCamera _camera;
		int _selectedIndex, _nextSelectedIndex;
		float _moveFactor;
		Vector3 _startPosition, _targetPosition;
		List<Vehicle> _models = new List<Vehicle>();
		Vector3 _cameraOffset = new Vector3(0, 55, 80);

		GarageModel _garageModel;
		float _screenWidth;
		float _spin;
		AlphaTestEffect _effect;


		public ChooseCarScreen()
			: base(true)
		{
			_camera = new SimpleCamera();
			_camera.LookAt = Vector3.Forward;
			Engine.Instance.Camera = _camera;
			_camera.Position = GetPosition(0) + _cameraOffset;
			_camera.LookAt = new Vector3(_camera.Position.X, _camera.Position.Y - 55, -1);
			_targetPosition = _camera.Position;
			_effect = new AlphaTestEffect(Engine.Instance.Device);
			_screenWidth = Engine.Instance.Device.Viewport.Width;

			


			for (int i = 0; i < _models.Count; i++)
			{
				_models[i].Position = GetPosition(i);
				_models[i].Direction = new Vector3(0.9f, 0, 0.5f);
				_models[i].UpdateCarMatrixAndCamera();
			}

			_garageModel = new GarageModel(_models.Count);

			ScreenEffects.Instance.UnFadeScreen();
		}

		#region IDrawableObject Members

		public void Update(GameTime gameTime)
		{
			if (UIController.Right)
			{
				if (_nextSelectedIndex == _models.Count - 1)
					return;
				_nextSelectedIndex++;
			}
			else if (UIController.Left)
			{
				if (_nextSelectedIndex == 0)
					return;
				_nextSelectedIndex--;
			}
			if (UIController.Ok)
			{
				GameConfig.SelectedVehicle = _models[_nextSelectedIndex];
				Engine.Instance.Mode = new ChooseGearboxScreen();

			}
			else if (UIController.Back)
			{
				Engine.Instance.Mode = new ChooseTrackScreen();
			}

			if (UIController.Left || UIController.Right)
			{
				_moveFactor = 0;
				_startPosition = _camera.Position;
				_targetPosition = GetPosition(_nextSelectedIndex) + _cameraOffset;
			}


			if (_moveFactor < 1 && _nextSelectedIndex != _selectedIndex)
			{
				Vector3 cameraPosition = Vector3.Lerp(_startPosition, _targetPosition, _moveFactor);
				_camera.Position = cameraPosition;
				_camera.LookAt = new Vector3(_camera.Position.X, _camera.Position.Y - 55, _camera.Position.Z - 80);
			}

			if (_moveFactor > 1)
			{
				_selectedIndex = _nextSelectedIndex;
				_targetPosition = _camera.Position;
			}
			else
			{
				_moveFactor += (float)gameTime.ElapsedGameTime.TotalSeconds * 4.5f;
			}

			Engine.Instance.Camera.Update(gameTime);
		}


		public void Draw()
		{
			Engine.Instance.Device.DepthStencilState = DepthStencilState.Default;
			Engine.Instance.Device.BlendState = BlendState.NonPremultiplied;
			_effect.View = Engine.Instance.Camera.View;
			_effect.Projection = Engine.Instance.Camera.Projection;

			_effect.World = Matrix.CreateScale(120 * _models.Count, 100, 130) * Matrix.CreateRotationY(0.53f) * Matrix.CreateTranslation(50, 52, -50);
			_garageModel.Render(_effect);

			for (int i = 0; i < _models.Count; i++)
			{
				_models[i].Render();
			}



			UpdateDescriptionText();
		}

		#endregion

		private Vector3 GetPosition(int index)
		{
			Vector3 position = new Vector3(0, 2, 0);
			position += (index * (new Vector3(60, 0, -35)));
			return position;
		}

		private void UpdateDescriptionText()
		{
			//Vector3 position;

			//Engine.Instance.SpriteBatch.Begin();

			//if (_moveFactor < 0.5f)
			//{
			//	position = SlideTextBlock(new Vector3(20, 0, 0), -1000);
			//	Engine.Instance.SpriteBatch.DrawString(Font, _models[_selectedIndex].Name, new Vector2(position.X, 50), Color.White, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);
			//}
			//else
			//{
			//	position = SlideTextBlock(new Vector3(-1000, 0, 1000), 20);
			//	Engine.Instance.SpriteBatch.DrawString(Font, _models[_nextSelectedIndex].Name, new Vector2(position.X, 50), Color.White, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);
			//}


			//if (_moveFactor < 0.5f)
			//{
			//	position = SlideTextBlock(new Vector3(_screenWidth - 350, 0, 0), _screenWidth + 1200);
			//	Engine.Instance.SpriteBatch.DrawString(Font, _models[_selectedIndex].Description, new Vector2(position.X, 110), Color.White, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
			//}
			//else
			//{
			//	position = SlideTextBlock(new Vector3(_screenWidth + 1200, 0, 0), _screenWidth - 350);
			//	Engine.Instance.SpriteBatch.DrawString(Font, _models[_nextSelectedIndex].Description, new Vector2(position.X, 110), Color.White, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
			//}

			//Engine.Instance.SpriteBatch.End();
		}

		private Vector3 SlideTextBlock(Vector3 startPos, float destination)
		{
			return Vector3.Lerp(startPos, new Vector3(destination, 0, 0), _moveFactor);
		}
	}
}
