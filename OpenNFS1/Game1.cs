using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NfsEngine;
using OpenNFS1.UI.Screens;

namespace OpenNFS1
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager _graphics;
		RenderTarget2D _renderTarget;

		[DllImport("user32.dll")]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndIntertAfter,
			int X, int Y, int cx, int cy, int uFlags);
		
		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			GameConfig.Load();

			if (GameConfig.FullScreen)
			{
				_graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				_graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			}
			else
			{
				_graphics.PreferredBackBufferWidth = 640;
				_graphics.PreferredBackBufferHeight = 480;
			}
			_graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
			_graphics.IsFullScreen = false;
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

			if (GameConfig.FullScreen)
			{
				// move window to top-left
				Type type = typeof(OpenTKGameWindow);
				System.Reflection.FieldInfo field = type.GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				OpenTK.GameWindow window = (OpenTK.GameWindow)field.GetValue(this.Window);
				this.Window.IsBorderless = true;
				window.X = 0;
				window.Y = 0;
			}
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			Engine.Create(this, _graphics);

			_renderTarget = new RenderTarget2D(Engine.Instance.Device, 640, 480, false, _graphics.GraphicsDevice.DisplayMode.Format, DepthFormat.Depth24, 1, RenderTargetUsage.DiscardContents);
			Engine.Instance.Device.SetRenderTarget(_renderTarget);
			Engine.Instance.ScreenSize = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);

			string gameDataPath = "CD_Data";
			if (!Directory.Exists(gameDataPath) || Directory.GetDirectories(gameDataPath).Length == 0)
				Engine.Instance.Screen = new ChooseDataDownloadScreen();
			else
			{
				Engine.Instance.Screen = new HomeScreen();
			}
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
			Engine.Instance.Device.SetRenderTarget(_renderTarget);

			Color c = new Color(0.1f, 0.1f, 0.1f);
			_graphics.GraphicsDevice.Clear(c);

			base.Draw(gameTime);

			Engine.Instance.Device.SetRenderTarget(null);

			using (SpriteBatch sprite = new SpriteBatch(Engine.Instance.Device))
			{
				Rectangle r = Window.ClientBounds;
				if (GameConfig.FullScreen)
				{
					// retain original 4:3 aspect ratio
					int originalWith = r.Width;
					int w = (int)((4f / 3f) * r.Height);
					r.Width = w;
					r.X = originalWith / 2 - w / 2;
				}
				sprite.Begin();
				sprite.Draw(_renderTarget, r, Color.White);
				sprite.End();
			}
		}
	}
}
