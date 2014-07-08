using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Media;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpenNFS1.Parsers;

namespace OpenNFS1.UI.Screens
{
    class BaseUIScreen
    {
		protected SpriteFont Font { get; private set; }
		int _currentLine = 30;
		static BitmapEntry _background;
		protected float TextSize = 0.5f;
		protected float TitleSize = 1;
		protected float SectionSize = 0.8f;

        public BaseUIScreen()
        {
			Font = Engine.Instance.ContentManager.Load<SpriteFont>("Content\\ArialBlack");

			if (_background == null)
			{
				QfsFile qfs = new QfsFile(@"Frontend\IArt\InstLoad.qfs");
				_background = qfs.Fsh.Header.FindByName("bgnd");
			}

        }

		public virtual void Draw()
		{
			Engine.Instance.SpriteBatch.Begin();
			Engine.Instance.SpriteBatch.Draw(_background.Texture, Vector2.Zero, Color.FromNonPremultiplied(108, 108, 108, 255));
			_currentLine = 30;
		}

		public void WriteTitleLine(string text)
		{
			Engine.Instance.SpriteBatch.DrawString(Font, text, new Vector2(20, _currentLine), Color.Yellow, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
			_currentLine += 80;
		}

		public void WriteLine(string text)
		{
			WriteLine(text, Color.White, _currentLine, 30, 0.5f);
		}

		public void WriteLine(string text, Color c)
		{
			WriteLine(text, c, _currentLine, 30, 0.5f);
		}

		public void WriteLine(string text, bool selected, int lineOffset, int column, float size)
		{
			WriteLine(text, selected ? Color.Yellow : Color.LightGray, lineOffset, column, size);
		}

		public void WriteLine(string text, Color c, int lineOffset, int column, float size)
		{
			_currentLine += lineOffset * (int)(size * 2);
			Engine.Instance.SpriteBatch.DrawString(Font, text, new Vector2(column+2, _currentLine+2), Color.FromNonPremultiplied(0,0,0,150), 0, Vector2.Zero, size, SpriteEffects.None, 0);
			Engine.Instance.SpriteBatch.DrawString(Font, text, new Vector2(column, _currentLine), c, 0, Vector2.Zero, size, SpriteEffects.None, 0);
		}
    }
}
