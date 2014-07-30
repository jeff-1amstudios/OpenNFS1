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

			WriteLine("No game data found", Color.Red, 20, 30, TitleSize);
			WriteLine("Download and use Need for Speed 1 CD data package (15mb)?", Color.White, 80, 30, TextSize);
						
			WriteLine("OK", _selectedIndex == 0, 40, 30, TextSize);
			WriteLine("No thanks", _selectedIndex == 1, 0, 80, TextSize);

			WriteLine("The Need for Speed 1 CD data package contains files produced", Color.LightGray, 150, 30, TextSize);
			WriteLine("by Pioneer Productions / EA Seattle in 1995.", Color.LightGray, 20, 30, TextSize);
			WriteLine("* 1amStudios and OpenNFS1 are not connected in any way", Color.LightGray, 50, 30, TextSize);
			WriteLine("with Pioneer Productions or EA Seattle.", Color.LightGray, 20, 30, TextSize);

			Engine.Instance.SpriteBatch.End();
		}
	}
}
