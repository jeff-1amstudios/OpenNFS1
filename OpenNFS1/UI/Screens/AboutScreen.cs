using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenNFS1.UI.Screens
{
    class AboutScreen : BaseUIScreen, IGameScreen
    {
        string _text;

        public AboutScreen()
            : base(false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Need for Speed:XNA is a ground-up remake of Need for Speed 1");
            sb.AppendLine("(released in 1995).");
            sb.AppendLine("It uses the original data files from Need for Speed 1 but");
            sb.AppendLine("the code is completely new.");
            sb.AppendLine();
            sb.AppendLine("All coding by Jeff - jeff@1amstudios.com");
            sb.AppendLine();
            sb.AppendLine("All in-race models, textures, tracks, cars by Pioneer");
            sb.AppendLine("Productions / EA Seattle (C) 1995");
            sb.AppendLine();
            sb.AppendLine("Need for Speed:XNA is not affiated in any way with EA,");
            sb.AppendLine("Pioneer Productions or Road & Track.");
            sb.AppendLine();
            sb.AppendLine("http://www.1amstudios.com/games/needforspeed/");
            
            _text = sb.ToString();
        }

        #region IDrawableObject Members

        public void Update(GameTime gameTime)
        {
            if (UIController.Back || UIController.Ok)
            {
                Engine.Instance.Mode = new HomeScreen2();
            }
        }

        public void Draw()
        {
            Engine.Instance.SpriteBatch.Begin();

            Engine.Instance.SpriteBatch.DrawString(Font, "About", new Vector2(20, 40), Color.Yellow, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);

            Engine.Instance.SpriteBatch.DrawString(Font, _text, new Vector2(40, 120), Color.White, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

            Engine.Instance.SpriteBatch.End();
        }

        #endregion
    }
}
