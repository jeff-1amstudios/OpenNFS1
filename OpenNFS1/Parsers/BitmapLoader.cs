using System;
using System.Collections.Generic;
using System.Text;
using GameEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace OpenNFS1.Parsers
{
    class TextureGenerator
    {
        byte[] _palette;

        public TextureGenerator(byte[] palette)
        {
            _palette = palette;
        }

		public Texture2D Generate(byte[] pixelData, int width, int height)
		{
			Texture2D newTexture = new Texture2D(Engine.Instance.Device, width, height);
			newTexture.SetData<byte>(GenerateImageData(pixelData, width, height));
			return newTexture;
		}

        private byte[] GenerateImageData(byte[] pixelData, int width, int height)
        {
			Vector3 mostUsed = Vector3.Zero;

            int overhang = 0; // (4 - ((width * 4) % 4));
            int stride = (width * 4) + overhang;

            byte[] imgData = new byte[stride * height];
            int curPosition = 0;
            for (int i = 0; i < height; i++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte pixel = pixelData[width * i + x];

                    if (pixel == 0xFF)
                    {
                        //byte[] rgb = GetRGBForPixel(mostUsed);
                        imgData[curPosition] = (byte)mostUsed.Z; // rgb[2];
                        imgData[curPosition + 1] = (byte)mostUsed.Y; // rgb[1];
                        imgData[curPosition + 2] = (byte)mostUsed.X; // rgb[0];
                        imgData[curPosition + 3] = 0;
                    }
                    else
                    {
                        byte[] rgb = GetRGBForPixel(pixel);
                        imgData[curPosition] = rgb[0];
                        imgData[curPosition + 1] = rgb[1];
                        imgData[curPosition + 2] = rgb[2];
                        imgData[curPosition + 3] = 0xFF;
                    }
                    curPosition += 4;
                }
                curPosition += overhang;
            }

            return imgData;
        }

        private Vector3 GetMostUsedColour(string id, byte[] pixels)
        {
			if (_palette == null) return new Vector3(255, 0, 255);

            if (id.StartsWith("tyr"))
            {
                return Vector3.Zero;
            }

            byte[] coloursUsed = new byte[256];
            foreach (byte pixel in pixels)
            {
                if (pixel != 0xFF)
                    coloursUsed[pixel]++;
            }

            int coloursUsedCount = 1;
            Vector3 avgColour = Vector3.Zero;
            for (int i = 0; i < 255; i++)
            {
                if (coloursUsed[i] > 0)
                {
                    avgColour += new Vector3(_palette[i * 3], _palette[i * 3 + 1], (int)_palette[i * 3 + 2]);
                    coloursUsedCount++;
                }
            }
            return avgColour / coloursUsedCount;
        }

        private byte[] GetRGBForPixel(int pixel)
        {
            if (_palette == null)
                return new byte[] { 255, 0, 255 };

            byte[] rgb = new byte[3];
            rgb[0] = _palette[pixel * 3];
            rgb[1] = _palette[pixel * 3 + 1];
            rgb[2] = _palette[pixel * 3 + 2];
            return rgb;
        }
    }
}
