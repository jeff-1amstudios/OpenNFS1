using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OpenNFS1.UI
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
            
        }
    }
}
