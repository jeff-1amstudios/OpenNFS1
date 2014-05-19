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
//	class ChooseTrackScreen : BaseUIScreen, IGameScreen
//	{
//		SimpleCamera _camera;
//		int _selectedIndex = 0, _nextSelectedIndex = 0;
//		float _moveFactor;
//		Vector3 _startPosition, _targetPosition;
//		List<TrackPoster> _tracks = new List<TrackPoster>();
//		Vector3 _cameraOffset = new Vector3(0, 45, 150);
//		GarageModel _garageModel;
//		float _screenWidth;
//		SamplerState _samplerState;
//		RasterizerState _rasterizer;
//		AlphaTestEffect _effect;

//		public ChooseTrackScreen()
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

//			for (int i = 0; i < TrackDescription.Descriptions.Count; i++)
//			{
//				if (!TrackDescription.Descriptions[i].HideFromMenu)
//					_tracks.Add(new TrackPoster(TrackDescription.Descriptions[i], GetPosition(i) + new Vector3(0, 50, 0)));
//			}

//			_garageModel = new GarageModel(_tracks.Count);

//			_rasterizer = new RasterizerState();
//			_rasterizer.CullMode = CullMode.None;
//			_rasterizer.FillMode = FillMode.Solid;
//			Engine.Instance.Device.RasterizerState = _rasterizer;
//		}

//		#region IDrawableObject Members

//		public void Update(GameTime gameTime)
//		{
//			if (UIController.Right)
//			{
//				if (_nextSelectedIndex == _tracks.Count - 1)
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
//				GameConfig.SelectedTrack = _tracks[_nextSelectedIndex].Track;
//				ScreenEffects.Instance.FadeCompleted += new EventHandler(Instance_FadeCompleted);
//				ScreenEffects.Instance.FadeSpeed = 500;
//				ScreenEffects.Instance.FadeScreen();
//			}
//			else if (UIController.Back)
//			{
//				Engine.Instance.Mode = new HomeScreen();
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
//			Engine.Instance.Mode = new ChooseCarScreen();
//		}

//		public void Draw()
//		{
//			//Engine.Instance.Device.RenderState.DepthBufferEnable = true;
//			//Engine.Instance.Device.RenderState.AlphaBlendEnable = false;
//			Engine.Instance.Device.RasterizerState = _rasterizer;
//			_effect.View = Engine.Instance.Camera.View;
//			_effect.Projection = Engine.Instance.Camera.Projection;

//			_effect.World = Matrix.CreateScale(128 * _tracks.Count, 90, 130) * Matrix.CreateRotationY(0.43f) * Matrix.CreateTranslation(50, 52, 48);
//			_garageModel.Render(_effect);

//			for (int i = 0; i < _tracks.Count; i++)
//			{
//				_tracks[i].Render(_effect, _moveFactor > 1 && i == _nextSelectedIndex);
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

//			if (_moveFactor < 0.5f)
//			{
//				position = SlideTextBlock(new Vector3(240, 0, 0), -1000);
//				Engine.Instance.SpriteBatch.DrawString(Font, _tracks[_selectedIndex].Title, new Vector2(position.X, 140), Color.White, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);
//			}
//			else
//			{
//				position = SlideTextBlock(new Vector3(-1000, 0, 0), 240);
//				Engine.Instance.SpriteBatch.DrawString(Font, _tracks[_nextSelectedIndex].Title, new Vector2(position.X, 140), Color.White, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);
//			}

//			if (_moveFactor >= 1 && _tracks[_nextSelectedIndex].Track.IsOpenRoad)
//			{
//				Engine.Instance.SpriteBatch.DrawString(Font, "Use Up/Down to choose stage", new Vector2(position.X, 530), Color.White, 0, Vector2.Zero, 0.7f, SpriteEffects.None, 0);
//			}

//			Engine.Instance.SpriteBatch.End();
//		}

//		private Vector3 SlideTextBlock(Vector3 startPos, float destination)
//		{
//			return Vector3.Lerp(startPos, new Vector3(destination, 0, 0), _moveFactor);
//		}
//	}
//}
