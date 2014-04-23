using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NeedForSpeed.UI
{
    class SpeedoControl
    {
        int x, y;
        Texture2D _speedoTexture, _speedoLineTexture;

        public SpeedoControl()
        {
            int height = 768;
            int width = 1024;

            x = 20;
            y = height - 220;

            _speedoTexture = Engine.Instance.ContentManager.Load<Texture2D>("Content\\Speedo");
            _speedoLineTexture = Engine.Instance.ContentManager.Load<Texture2D>("Content\\SpeedoLine");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rpmFactor">0..1 percentage of rpm compared to redline</param>
        public void Render(float rpmFactor)
        {
            Engine.Instance.SpriteBatch.Begin();
            
            Color color = new Color(255,255,255,160);
            Engine.Instance.SpriteBatch.Draw(_speedoTexture, new Rectangle(x, y, 200, 200), color);

            float rotation = (float)(rpmFactor * Math.PI * 1.4f) - 2.56f;
            Engine.Instance.SpriteBatch.Draw(_speedoLineTexture, new Vector2(x + 100, y + 100), null, color, rotation, new Vector2(5, 25), new Vector2(1, 3.7f), SpriteEffects.None, 0);


            Engine.Instance.SpriteBatch.End();
        }
    }
}
