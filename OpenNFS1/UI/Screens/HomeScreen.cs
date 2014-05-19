//using System;
//using System.Collections.Generic;

//using System.Text;
//using NfsEngine;
//using Microsoft.Xna.Framework;
//using OpenNFS1.Parsers;
//using Microsoft.Xna.Framework.Graphics;
//using NfsEngine;
//using Microsoft.Xna.Framework.Input;
//using OpenNFS1.Physics;
//using OpenNFS1.Vehicles;

//namespace OpenNFS1.UI.Screens
//{
//	class HomeScreen : BaseUIScreen, IGameScreen
//	{
//		SimpleCamera _camera;
//		int _selectedIndex = 0, _nextSelectedIndex = 0;
//		float _moveFactor;
//		Vector3 _startPosition, _targetPosition;
//		List<MenuPoster> _options = new List<MenuPoster>();
//		Vector3 _cameraOffset = new Vector3(0, 45, 150);
//		GarageModel _garageModel;
//		float _screenWidth;
//		SamplerState _samplerState;
//		RasterizerState _rasterizer;
//		AlphaTestEffect _effect;

//		public HomeScreen()
//			: base(true)
//		{
//			_camera = new SimpleCamera();
//			_camera.LookAt = Vector3.Forward;
//			Engine.Instance.Camera = _camera;
//			_camera.Position = GetPosition(0) + _cameraOffset;
//			_camera.LookAt = new Vector3(_camera.Position.X, _camera.Position.Y, -1);
//			_targetPosition = _camera.Position;
//			_effect = new AlphaTestEffect(Engine.Instance.Device);
//			_screenWidth = Engine.Instance.Device.Viewport.Width;

//			_options.Add(new MenuPoster("Content/race", "Choose race", GetPosition(0) + new Vector3(0, 50, 0)));
//			_options.Add(new MenuPoster("Content/Controls", "Controls", GetPosition(1) + new Vector3(0, 50, 0)));
//			_options.Add(new MenuPoster("Content/About", "About", GetPosition(2) + new Vector3(0, 50, 0)));

//			_samplerState = new SamplerState();
//			_samplerState.AddressU = TextureAddressMode.Clamp;
//			_samplerState.AddressV = TextureAddressMode.Clamp;
//			Engine.Instance.Device.SamplerStates[0] = _samplerState;
//			_rasterizer = new RasterizerState();
//			_rasterizer.CullMode = CullMode.None;
//			_rasterizer.FillMode = FillMode.Solid;
//			Engine.Instance.Device.RasterizerState = _rasterizer;

//			_garageModel = new GarageModel(_options.Count);
//		}

//		#region IDrawableObject Members

//		public void Update(GameTime gameTime)
//		{
//			if (UIController.Right)
//			{
//				if (_nextSelectedIndex == _options.Count - 1)
//					return;
//				_nextSelectedIndex++;
//			}
//			else if (UIController.Left)
//			{
//				if (_nextSelectedIndex == 0)
//					return;
//				_nextSelectedIndex--;
//			}
//			if (UIController.Ok)
//			{
//				ScreenEffects.Instance.FadeCompleted += new EventHandler(Instance_FadeCompleted);
//				ScreenEffects.Instance.FadeSpeed = 500;
//				ScreenEffects.Instance.FadeScreen();
//			}
//			else if (UIController.Back)
//			{
//				Engine.Instance.Game.Exit();
//			}

//			if (UIController.Left || UIController.Right)
//			{
//				_moveFactor = 0;
//				_startPosition = _camera.Position;
//				_targetPosition = GetPosition(_nextSelectedIndex) + _cameraOffset;
//			}


//			if (_moveFactor < 1 && _nextSelectedIndex != _selectedIndex)
//			{
//				Vector3 cameraPosition = Vector3.Lerp(_startPosition, _targetPosition, _moveFactor);
//				_camera.Position = cameraPosition;
//				_camera.LookAt = new Vector3(_camera.Position.X, _camera.Position.Y, _camera.Position.Z - 100);
//			}

//			if (_moveFactor > 1)
//			{
//				_selectedIndex = _nextSelectedIndex;
//				_targetPosition = _camera.Position;
//			}
//			else
//			{
//				_moveFactor += (float)gameTime.ElapsedGameTime.TotalSeconds * 4.5f;
//			}

//			Engine.Instance.Camera.Update(gameTime);

//		}

//		void Instance_FadeCompleted(object sender, EventArgs e)
//		{
//			ScreenEffects.Instance.FadeCompleted -= Instance_FadeCompleted;
//			if (_selectedIndex == 0)
//			{
//				Engine.Instance.Mode = new ChooseTrackScreen();
//			}
//			else if (_selectedIndex == 1)
//			{
//				Engine.Instance.Mode = new ControlsScreen();
//			}
//			else if (_selectedIndex == 2)
//			{
//				Engine.Instance.Mode = new AboutScreen();
//			}
//		}

//		public void Draw()
//		{
//			Engine.Instance.Device.DepthStencilState = DepthStencilState.Default;
//			Engine.Instance.Device.RasterizerState = _rasterizer;

//			_effect.View = Engine.Instance.Camera.View;
//			_effect.Projection = Engine.Instance.Camera.Projection;
//			_effect.World = Matrix.CreateScale(128 * _options.Count, 80, 130) * Matrix.CreateRotationY(0.43f) * Matrix.CreateTranslation(50, 52, 48);
//			_garageModel.Render(_effect);

//			for (int i = 0; i < _options.Count; i++)
//			{
//				_options[i].Render(_effect, _moveFactor > 1 && i == _nextSelectedIndex);
//			}

//			UpdateDescriptionText();
//		}

//		#endregion

//		private Vector3 GetPosition(int index)
//		{
//			Vector3 position = new Vector3(0, 5, 3);
//			position += (index * (new Vector3(60, 0, -27)));
//			return position;
//		}

//		private void UpdateDescriptionText()
//		{
//			Vector3 position;

//			Engine.Instance.SpriteBatch.Begin();

//			//Engine.Instance.SpriteBatch.Draw(Engine.Instance.ContentManager.Load<Texture2D>("Content/homescreen-background"), new Rectangle(0, 0, 640, 266), Color.White);

//			if (_moveFactor < 0.5f)
//			{
//				position = SlideTextBlock(new Vector3(240, 0, 0), -1000);
//				Engine.Instance.SpriteBatch.DrawString(Font, _options[_selectedIndex].Title, new Vector2(position.X, 140), Color.White, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);
//			}
//			else
//			{
//				position = SlideTextBlock(new Vector3(-1000, 0, 0), 240);
//				Engine.Instance.SpriteBatch.DrawString(Font, _options[_nextSelectedIndex].Title, new Vector2(position.X, 140), Color.White, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);
//			}

//			Engine.Instance.SpriteBatch.DrawString(Font, "Need for Speed:XNA - v1.1", new Vector2(60, 710), Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);

//			Engine.Instance.SpriteBatch.End();
//		}

//		private Vector3 SlideTextBlock(Vector3 startPos, float destination)
//		{
//			return Vector3.Lerp(startPos, new Vector3(destination, 0, 0), _moveFactor);
//		}
//	}
//}