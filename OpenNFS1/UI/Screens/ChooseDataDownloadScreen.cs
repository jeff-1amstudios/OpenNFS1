using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NfsEngine;

namespace OpenNFS1.UI.Screens
{
	class ChooseDataDownloadScreen : BaseUIScreen, IGameScreen
	{
		int _selectedIndex = 0;

		public ChooseDataDownloadScreen() : base()
		{

		}

		public void Update(GameTime gameTime)
		{
			if (Engine.Instance.Input.WasPressed(Keys.Up))
			{
				_selectedIndex = 0;
			}
			else if (Engine.Instance.Input.WasPressed(Keys.Down))
			{
				_selectedIndex = 1;
			}

			if (Engine.Instance.Input.WasPressed(Keys.Enter))
			{
				if (_selectedIndex == 0)
				{
					Engine.Instance.Screen = new DataDownloadScreen();
				}
				else
				{
					Engine.Instance.Game.Exit();
				}
			}
		}

		public override void Draw()
		{
			base.Draw();

			WriteTitleLine("No game data found");

			WriteLine("Download and use Need for Speed 1 CD data package (15mb)?");
			WriteLine("");
			string[] options = { "OK", "Cancel" };
			float y = 250;
			for (int i = 0; i < options.Length; i++)
			{
				Color c = Color.White;
				if (_selectedIndex == i)
					WriteLine("< " + options[i] + " >");
				else
					WriteLine("  " + options[i]);
				y += 35;
			}

			WriteLine("The Need for Speed 1 CD data package contains files produced", Color.White, 350, 30, 0.5f);
			WriteLine("by Pioneer Productions / EA Seattle in 1995.");
			WriteLine("* 1amStudios and OpenNFS1 are not connected in any way", Color.White, 420, 30, 0.5f);
			WriteLine("with Pioneer Productions or EA Seattle.");

			Engine.Instance.SpriteBatch.End();
		}
	}
}
