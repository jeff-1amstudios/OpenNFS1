using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine;
using OpenNFS1.Parsers;
using OpenNFS1.Physics;

namespace OpenNFS1.Views
{
	class BaseExternalView
	{
		static BitmapEntry _bottomBar, _bottomFill, _tacho, _map;
		static Texture2D _tachLineTexture;
		const int _size = 160;
		const float _needleLength = 2.5f;
		const float _needleWidth = 3f;

		public BaseExternalView()
		{
			if (_bottomBar == null)
			{
				var fsh = new FshFile(@"Simdata\Misc\MaskHi.fsh");
				_bottomBar = fsh.Header.Bitmaps.Find(a => a.Id == "b00b");
				_bottomFill = fsh.Header.Bitmaps.Find(a => a.Id == "0002");
				_tacho = fsh.Header.Bitmaps.Find(a => a.Id == "tach");
				_map = fsh.Header.Bitmaps.Find(a => a.Id == "mpbd");

				_tachLineTexture = new Texture2D(Engine.Instance.Device, (int)_needleWidth, 25);
				Color[] pixels = new Color[_tachLineTexture.Width * _tachLineTexture.Height];
				for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.Red;
				_tachLineTexture.SetData<Color>(pixels);
			}
		}

		public void RenderBackground(DrivableVehicle car)
		{
			float carRpm = car.Motor.Rpm / car.Motor.RedlineRpm;
			Engine.Instance.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);

			// Draw tacho
			Color color = new Color(255, 255, 255, 200);
			Engine.Instance.SpriteBatch.Draw(_tacho.Texture, new Rectangle(_tacho.Misc[2], _tacho.Misc[3], _size, _size), Color.White);

			float rotation = (float)(carRpm * Math.PI * 1.4f) - 2.56f;
			Engine.Instance.SpriteBatch.Draw(_tachLineTexture, new Vector2(_tacho.Misc[2] + _size / 2, _tacho.Misc[3] + _size / 2), null, color, rotation, new Vector2(_needleWidth/2, 25), new Vector2(1f, _needleLength), SpriteEffects.None, 0);

			// mini-map overlay
			//Engine.Instance.SpriteBatch.Draw(_map.Texture, _map.GetDisplayAt(), Color.White);

			// Draw bottom fill
			const int barHeight = 17;
			Engine.Instance.SpriteBatch.Draw(_bottomBar.Texture, new Vector2(0, 400), new Rectangle(0, 0, 640, barHeight), Color.White);
			Engine.Instance.SpriteBatch.Draw(_bottomFill.Texture, new Vector2(0, 400 + barHeight), new Rectangle(0, 0, 640, 60), Color.White);		

			Engine.Instance.SpriteBatch.End();
		}
	}
}
