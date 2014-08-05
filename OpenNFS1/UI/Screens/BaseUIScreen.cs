using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Media;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OpenNFS1.Parsers;
using System.IO;

namespace OpenNFS1.UI.Screens
{
    class BaseUIScreen
    {
		protected SpriteFont Font { get; private set; }
		static Texture2D _background;
		int _currentLine;
		protected float TextSize = 0.5f;
		protected float TitleSize = 1;
		protected float SectionSize = 0.8f;
		protected Color TextColor = Color.Orange;

        public BaseUIScreen()
        {
			Font = Engine.Instance.ContentManager.Load<SpriteFont>("Content\\ArialBlack-Italic");
			_background = Engine.Instance.ContentManager.Load<Texture2D>("Content\\splash-screen.jpg");
        }

		public virtual void Draw()
		{
			Engine.Instance.SpriteBatch.Begin();
			if (_background != null)
			{
				Engine.Instance.SpriteBatch.Draw(_background, Vector2.Zero, Color.FromNonPremultiplied(50, 50, 50, 255));
			}
			_currentLine = 5;
		}

		public void WriteLine(string text)
		{
			WriteLine(text, Color.White, _currentLine, 30, TextSize);
		}

		public void WriteLine(string text, Color c)
		{
			WriteLine(text, c, _currentLine, 30, TextSize);
		}

		public void WriteLine(string text, bool selected, int lineOffset, int column, float size)
		{
			WriteLine(text, selected ? Color.Yellow : Color.LightGray, lineOffset, column, selected ? size * 1.1f : size);
		}

		public void WriteLine(string text, Color c, int lineOffset, int column, float size)
		{
			if (String.IsNullOrEmpty(text)) return;

			_currentLine += lineOffset;
			string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
			for (int i = 0; i <lines.Length; i++)
			{
				if (lines[i] != "")
				{
					Engine.Instance.SpriteBatch.DrawString(Font, lines[i], new Vector2(column + 2, _currentLine + 2), Color.FromNonPremultiplied(0, 0, 0, 150), 0, Vector2.Zero, size, SpriteEffects.None, 0);
					Engine.Instance.SpriteBatch.DrawString(Font, lines[i], new Vector2(column, _currentLine), c, 0, Vector2.Zero, size, SpriteEffects.None, 0);
				}
				if (i < lines.Length-1)
					_currentLine += 20;
			}
		}
    }
}
