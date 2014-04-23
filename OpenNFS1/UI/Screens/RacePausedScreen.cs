using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.Physics;

namespace NeedForSpeed.UI.Screens
{
    class RacePausedScreen : BaseUIScreen, IGameScreen
    {
        DoRaceScreen _currentRace;
        Texture2D _background;
        Vehicle _playerCar;
        int _selectedOption = 0;

        public RacePausedScreen(DoRaceScreen currentRace, Vehicle playerCar)
            : base(false)
        {
            _playerCar = playerCar;
            _playerCar.StopDriving();
            _currentRace = currentRace;
            _background = ScreenEffects.TakeScreenshot();
        }

        #region IDrawableObject Members

        public void Update(GameTime gameTime)
        {
            if (UIController.Up && _selectedOption > 0)
                _selectedOption--;
            else if (UIController.Down && _selectedOption < 1)
                _selectedOption++;


            if (UIController.Back)
            {
                _playerCar.InitializeForDriving();
                Engine.Instance.Mode = _currentRace;
                return;
            }

            if (UIController.Ok)
            {
                if (_selectedOption == 0)
                {
                    _playerCar.InitializeForDriving();
                    Engine.Instance.Mode = _currentRace;
                }
                else
                {
                    Engine.Instance.Mode = new ChooseTrackScreen();
                }
            }
        }

        public void Draw()
        {
            Engine.Instance.SpriteBatch.Begin();

            Engine.Instance.SpriteBatch.Draw(_background, Vector2.Zero, new Color(255, 255, 255, 100));

            int y = 40;
            Engine.Instance.SpriteBatch.DrawString(Font, "Race Paused", new Vector2(50, y), Color.WhiteSmoke, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);

            y += 400;
            Engine.Instance.SpriteBatch.DrawString(Font, " Continue", new Vector2(200, y), _selectedOption == 0 ? Color.Yellow : Color.WhiteSmoke, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
            y += 50;
            Engine.Instance.SpriteBatch.DrawString(Font, " Main Menu", new Vector2(200, y), _selectedOption == 1 ? Color.Yellow : Color.WhiteSmoke, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);

            Engine.Instance.SpriteBatch.End();
        }

        #endregion
    }
}
