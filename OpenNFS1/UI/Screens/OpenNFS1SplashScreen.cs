using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using GameEngine;

namespace OpenNFS1.UI.Screens
{
	class OpenNFS1SplashScreen : BaseUIScreen, IGameScreen
	{
		float _elapsedTime;
		public void Update(GameTime gameTime)
		{
			_elapsedTime += Engine.Instance.FrameTime;
			if (Engine.Instance.Input.WasPressed(Keys.Enter))
			{
				if (!Directory.Exists(GameConfig.CdDataPath) || Directory.GetDirectories(GameConfig.CdDataPath).Length == 0)
					Engine.Instance.Screen = new ChooseDataDownloadScreen();
				else
				{
					Engine.Instance.Screen = new HomeScreen();
				}
			}
		}

		public override void Draw()
		{
			base.Draw();

			var version = Assembly.GetExecutingAssembly().GetName().Version;

			WriteLine("OpenNFS1 v" + version.ToString(2), Color.Red, 0, 30, TextSize);
			WriteLine(
				"OpenNFS1 is a remake of the original EA Need for Speed 1.\r\n\r\n" + 
				"OpenNFS1 code was written from scratch without reverse\r\n" + 
				"engineering the NFS executable, and it uses the original data\r\n" + 
				"files that were on the CD back in 1995!", TextColor, 50, 30, TextSize);

			WriteLine("By Jeff H - www.1amstudios.com", TextColor, 40, 30, TextSize);

			WriteLine(
				"Need for Speed 1 was produced by Pioneer Productions\r\nand EA Seattle in 1995.\r\n" + 
				"1amStudios and OpenNFS1 are not affiliated in any way\r\nwith Pioneer Productions or EA Seattle.", Color.LightGray, 190, 30, TextSize);

			Engine.Instance.SpriteBatch.End();
		}
	}
}
