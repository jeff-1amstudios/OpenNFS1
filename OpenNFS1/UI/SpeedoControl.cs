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
		const int _size = 160;
		const float _needleLength = 2.5f;
        Texture2D _speedoTexture, _speedoLineTexture;

        public SpeedoControl()
        {
            int height = Engine.Instance.Device.Viewport.Height;
            int width = Engine.Instance.Device.Viewport.Width;

            x = 20;
            y = height - _size - 30;

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
            
            Color color = new Color(255,255,255,200);
            Engine.Instance.SpriteBatch.Draw(_speedoTexture, new Rectangle(x, y, _size, _size), Color.White);

            float rotation = (float)(rpmFactor * Math.PI * 1.4f) - 2.56f;
			Engine.Instance.SpriteBatch.Draw(_speedoLineTexture, new Vector2(x + _size / 2, y + _size / 2), null, color, rotation, new Vector2(5, 25), new Vector2(0.6f, _needleLength), SpriteEffects.None, 0);

			Engine.Instance.SpriteBatch.End();
        }
    }
}
