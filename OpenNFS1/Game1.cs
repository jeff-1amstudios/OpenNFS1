using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using NfsEngine;
using NfsEngine;
using NeedForSpeed.UI.Screens;

namespace NeedForSpeed
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 480;
            _graphics.PreferMultiSampling = true;
			_graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
            
            _graphics.IsFullScreen = false;

            //_graphics.GraphicsProfile = GraphicsProfile.Reach;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            Engine.Initialize(this, _graphics);
            Engine.Instance.ScreenSize = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
			//Engine.Instance.Device.DepthStencilState.
            //enable per-pixel transparency
            //Engine.Instance.Device.RenderState.AlphaTestEnable = true;
            //Engine.Instance.Device.RenderState.ReferenceAlpha = 5;
            //Engine.Instance.Device.RenderState.AlphaFunction = CompareFunction.Greater;

            Engine.Instance.Mode = new HomeScreen();         
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Engine.Instance.ContentManager.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}
