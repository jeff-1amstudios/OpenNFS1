//using System;
//using System.Collections.Generic;
//using System.Text;
//using NfsEngine;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace OpenNFS1.UI.Screens
//{
//	class ControlsScreen : BaseUIScreen, IGameScreen
//	{
//		string _text;

//		public ControlsScreen()
//			: base(false)
//		{
//			StringBuilder sb = new StringBuilder();
//			sb.AppendLine("-- Keyboard ---------------------------------------------------------------");
//			sb.AppendLine("Accelerate/Brake:     Up / Down");
//			sb.AppendLine("Steering:                   Left / Right");
//			sb.AppendLine("Gear Up/Down:          A / Z");
//			sb.AppendLine("Change camera:       C");
//			sb.AppendLine("Reset:                       R");
//			sb.AppendLine("Pause:                      Escape");
//			sb.AppendLine();
//			//sb.AppendLine("Xbox 360 Gamepad:");
//			sb.AppendLine("-- Xbox 360 Gamepad -----------------------------------------------");
//			sb.AppendLine("Accelerate/Brake:     Right Trigger / Left Trigger");
//			sb.AppendLine("Steering:                   Left Thumbstick");
//			sb.AppendLine("Gear Up/Down:          Button B / Button X");
//			sb.AppendLine("Change camera:       Right Shoulder");
//			_text = sb.ToString();
//		}

//		#region IDrawableObject Members

//		public void Update(GameTime gameTime)
//		{
//			if (UIController.Back || UIController.Ok)
//			{
//				Engine.Instance.Mode = new HomeScreen();
//			}
//		}

//		public void Draw()
//		{
//			Engine.Instance.SpriteBatch.Begin();
            
//			Engine.Instance.SpriteBatch.DrawString(Font, "Controls", new Vector2(20, 40), Color.Yellow, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);

//			Engine.Instance.SpriteBatch.DrawString(Font, _text, new Vector2(40, 120), Color.White, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

//			Engine.Instance.SpriteBatch.End();
//		}

//		#endregion
//	}
//}
