using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NfsEngine;

namespace NfsEngine
{
    public class Engine : DrawableGameComponent
    {
                
        private static Engine _instance;
        private ContentManager _contentManager;
        private ICamera _camera;
        private InputProvider _inputProvider;
        private GraphicsUtilities _graphicsUtils;
        private IWorld _world;
        private IGameScreen _mode;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Vector2 ScreenSize;
        private bool _fullScreen;
		public float FrameTime;
                
        public static Engine Instance
        {
            get
            {
                return _instance;
            }
        }

        public static void Initialize(Game game, GraphicsDeviceManager graphics)
        {
            Debug.Assert(_instance == null);
            _instance = new Engine(game);
            
            _instance.EngineStartup(graphics);
                       
        }

        
        private Engine(Game game)
            : base(game)
        {
             
        }

        private void EngineStartup(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;

            _contentManager = new ContentManager(base.Game.Services);

            //Game bits
            _inputProvider = new InputProvider(base.Game);
            _graphicsUtils = new GraphicsUtilities();
            _spriteBatch = new SpriteBatch(Device);
            base.Game.Components.Add(this);

            _instance._fullScreen = graphics.IsFullScreen;
        }

        public float AspectRatio
        {
            get
            {
                if (_fullScreen)
                    return ScreenSize.X / ScreenSize.Y;
                else
                {
					return (float)Device.Viewport.Width / (float)Device.Viewport.Height;
                    //Rectangle rect = Game.Window.ClientBounds;
                    //return (float)rect.Width / (float)rect.Height;
                }
            }
        }
        

        public override void Update(GameTime gameTime)
        {
			FrameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(gameTime);
            
            _inputProvider.Update(gameTime);
            SoundEngine2.Instance.Update(gameTime);
            //SoundEngine.Instance.Update();

            _mode.Update(gameTime);

            ScreenEffects.Instance.Update(gameTime);

            _graphicsUtils.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _mode.Draw();
            _graphicsUtils.Draw();
            ScreenEffects.Instance.Draw();
            _graphicsUtils.DrawText();
        }

        public ContentManager ContentManager
        {
            get { return _contentManager; }
        }

        public GraphicsDevice Device
        {
            get { return _graphics.GraphicsDevice; }
        }
        
				//public BasicEffect CurrentEffect
				//{
				//		get { return _currentEffect; }
				//		set { _currentEffect = value; }
				//}

        public GraphicsUtilities GraphicsUtils
        {
            get { return _graphicsUtils; }
        }

        public IWorld World
        {
            get { return _world; }
            set { _world = value; }
        }

        public ICamera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }

        public InputProvider Input
        {
            get { return _inputProvider; }
            set { _inputProvider = value; }
        }

        public IGameScreen Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public SpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

		Random _random = new Random();

		public Random Random
		{
			get
			{
				return _random;
			}
		}

        

        //public bool EnableBloom
        //{
        //    set
        //    {
        //        if (value)
        //        {
        //            _game.Components.Add(new BloomComponent(_game));
        //        }
        //        else
        //        {
        //            foreach (IGameComponent component in _game.Components)
        //            {
        //                if (component is BloomComponent)
        //                {
        //                    _game.Components.Remove(component);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
