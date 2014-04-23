using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using NfsEngine;
using Microsoft.Xna.Framework;

namespace NeedForSpeed.Parsers.Track
{
    class SkyboxGenerator
    {
        Texture2D _horizon;
        Texture2D _topTexture, _bottomTexture;
        Texture2D _sideTexture;

        public SkyboxGenerator(Texture2D horizonTexture)
        {
            _horizon = horizonTexture;
            
            Color[] pixels = new Color[_horizon.Width * _horizon.Height];
            _horizon.GetData<Color>(pixels);

            _topTexture = new Texture2D(Engine.Instance.Device, 1, 1);
            _topTexture.SetData<Color>(new Color[] { pixels[0] });  //top left pixel

            _bottomTexture = new Texture2D(Engine.Instance.Device, 1, 1);
            _bottomTexture.SetData<Color>(new Color[] { pixels[pixels.Length - 1] }); //bottom right pixel

			_sideTexture = new Texture2D(Engine.Instance.Device, _horizon.Width, _horizon.Height);
            int ptr = 0;
            Color[] flippedPixels = new Color[pixels.Length];
            for (int h = 0; h < _horizon.Height; h++)
            {
                for (int w = 0; w < _horizon.Width; w++)
                {
                    flippedPixels[ptr] = pixels[h * _horizon.Width + (_horizon.Width - w)-1];
                    ptr++;
                }
            }
            _sideTexture.SetData<Color>(flippedPixels);
        }

        public SkyBox Generate()
        {
            SkyBox skyBox = new SkyBox();
            skyBox.Textures[0] = _horizon;
            skyBox.Textures[1] = _horizon;
            skyBox.Textures[2] = _bottomTexture;
            skyBox.Textures[3] = _topTexture;
            skyBox.Textures[4] = _sideTexture;
            skyBox.Textures[5] = _sideTexture;
            return skyBox;
        }
    }
}
