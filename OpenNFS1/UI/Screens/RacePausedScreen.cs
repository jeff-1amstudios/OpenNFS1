using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Physics;

namespace OpenNFS1.UI.Screens
{
    class RacePausedScreen : BaseUIScreen, IGameScreen
    {
        DoRaceScreen _currentRace;
        Texture2D _background;
        int _selectedOption = 0;

        public RacePausedScreen(DoRaceScreen currentRace)
            : base(false)
        {
            _currentRace = currentRace;
            _background = ScreenEffects.TakeScreenshot();	
        }

        #region IDrawableObject Members

        public void Update(GameTime gameTime)
        {
			Engine.Instance.Device.Viewport = FullViewport;

            if (UIController.Up && _selectedOption > 0)
                _selectedOption--;
            else if (UIController.Down && _selectedOption < 1)
                _selectedOption++;

            if (UIController.Back)
            {
                Engine.Instance.Mode = _currentRace;
                return;
            }

            if (UIController.Ok)
            {
                if (_selectedOption == 0)
                {
                    Engine.Instance.Mode = _currentRace;
					_currentRace.Resume();
                }
                else
                {
                    Engine.Instance.Mode = new HomeScreen2();
                }
            }
        }

        public void Draw()
        {
            Engine.Instance.SpriteBatch.Begin();

            Engine.Instance.SpriteBatch.Draw(_background, Vector2.Zero, new Color(255, 255, 255, 255));

            int y = 20;
            Engine.Instance.SpriteBatch.DrawString(Font, "Race Paused", new Vector2(50, y), Color.WhiteSmoke, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);

            y += 200;
            Engine.Instance.SpriteBatch.DrawString(Font, " Continue", new Vector2(200, y), _selectedOption == 0 ? Color.Yellow : Color.WhiteSmoke, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
            y += 25;
            Engine.Instance.SpriteBatch.DrawString(Font, " Main Menu", new Vector2(200, y), _selectedOption == 1 ? Color.Yellow : Color.WhiteSmoke, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);

            Engine.Instance.SpriteBatch.End();
        }

        #endregion
    }
}
