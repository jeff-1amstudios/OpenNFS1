using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.Physics;
using Microsoft.Xna.Framework.Media;

namespace NeedForSpeed.UI.Screens
{
    class ChooseGearboxScreen : BaseUIScreen, IGameScreen
    {
        Texture2D _background;
        int _selectedOption = 0;

        public ChooseGearboxScreen()
            : base(false)
        {
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
                Engine.Instance.Mode = new ChooseCarScreen();
            }

            if (UIController.Ok)
            {
                GameConfig.ManualGearbox = _selectedOption == 1;
                MediaPlayer.Stop();
                ScreenEffects.Instance.FadeCompleted += new EventHandler(Instance_FadeCompleted);
                ScreenEffects.Instance.FadeScreen();
            }
        }

        public void Draw()
        {
            Engine.Instance.SpriteBatch.Begin();

            Engine.Instance.SpriteBatch.Draw(_background, Vector2.Zero, new Color(255, 255, 255, 100));

            int y = 340;
            Engine.Instance.SpriteBatch.DrawString(Font, " Auto gearbox", new Vector2(200, y), _selectedOption == 0 ? Color.Yellow : Color.WhiteSmoke, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
            y += 50;
            Engine.Instance.SpriteBatch.DrawString(Font, " Manual gearbox", new Vector2(200, y), _selectedOption == 1 ? Color.Yellow : Color.WhiteSmoke, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);

            Engine.Instance.SpriteBatch.End();
        }

        #endregion

        void Instance_FadeCompleted(object sender, EventArgs e)
        {
            ScreenEffects.Instance.FadeCompleted -= Instance_FadeCompleted;
            Engine.Instance.Mode = new LoadRaceScreen();
        }
    }
}
