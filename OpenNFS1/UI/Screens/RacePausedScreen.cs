using System;
using System.Collections.Generic;
using System.Text;
using GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Physics;

namespace OpenNFS1.UI.Screens
{
    class RacePausedScreen : BaseUIScreen, IGameScreen
    {
        DoRaceScreen _currentRace;
        int _selectedOption = 0;

        public RacePausedScreen(DoRaceScreen currentRace)
            : base()
        {
            _currentRace = currentRace;
        }

        #region IDrawableObject Members

        public void Update(GameTime gameTime)
        {
			//Engine.Instance.Device.Viewport = FullViewport;

            if (UIController.Up && _selectedOption > 0)
                _selectedOption--;
            else if (UIController.Down && _selectedOption < 1)
                _selectedOption++;

            if (UIController.Back)
            {
                Engine.Instance.Screen = _currentRace;
                return;
            }

            if (UIController.Ok)
            {
                if (_selectedOption == 0)
                {
                    Engine.Instance.Screen = _currentRace;
					_currentRace.Resume();
                }
                else
                {
                    Engine.Instance.Screen = new HomeScreen();
                }
            }
        }

		public override void Draw()
		{
			base.Draw();
			
			WriteLine("Race paused", Color.White, 20, 30, TitleSize);
			WriteLine("Continue", _selectedOption == 0, 60, 30, SectionSize);
			WriteLine("Main menu", _selectedOption == 1, 40, 30, SectionSize);

            Engine.Instance.SpriteBatch.End();
        }

        #endregion
    }
}
