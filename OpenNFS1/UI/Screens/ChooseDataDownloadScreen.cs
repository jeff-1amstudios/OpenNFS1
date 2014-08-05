using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameEngine;

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
			if (Engine.Instance.Input.WasPressed(Keys.Left))
			{
				_selectedIndex = 0;
			}
			else if (Engine.Instance.Input.WasPressed(Keys.Right))
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

			WriteLine("No game data found!", Color.Red, 0, 30, TextSize);
			WriteLine("Download Need for Speed 1 SE CD data package (15mb)?", TextColor, 80, 30, TextSize);
						
			WriteLine("OK", _selectedIndex == 0, 40, 30, TextSize);
			WriteLine("No thanks", _selectedIndex == 1, 0, 80, TextSize);

			WriteLine(
				"Need for Speed 1 was produced by Pioneer Productions\r\nand EA Seattle in 1995.\r\n" +
				"1amStudios and OpenNFS1 are not affiliated in any way\r\nwith Pioneer Productions or EA Seattle.", Color.LightGray, 120, 30, TextSize);

			Engine.Instance.SpriteBatch.End();
		}
	}
}
