using System;
using System.Collections.Generic;
using System.Text;
using GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Parsers.Track;

namespace OpenNFS1.UI.Screens
{
    class RaceFinishedScreen : BaseUIScreen, IGameScreen
    {
        Race _race;
        Texture2D _background;
        Track _raceTrack;

        int _selectedOption = 0;

        public RaceFinishedScreen(Race race, Track raceTrack)
            : base()
        {
            _race = race;
            _raceTrack = raceTrack;
            _background = ScreenEffects.TakeScreenshot();
            GC.Collect(); //force some memory freeness here.
        }


        #region IDrawableObject Members

        public void Update(GameTime gameTime)
        {
            if (UIController.Back)
            {
                Engine.Instance.Screen = new HomeScreen();
            }

            if (UIController.Up && _selectedOption > 0)
                _selectedOption--;
            else if (UIController.Down && _selectedOption < 1)
                _selectedOption++;

            if (UIController.Ok)
            {
                if (_selectedOption == 1)
                    Engine.Instance.Screen = new HomeScreen();
                else
                {
					if (!GameConfig.SelectedTrackDescription.IsOpenRoad)
                        Engine.Instance.Screen = new DoRaceScreen(_raceTrack);
                    else
                    {
                        GameConfig.SelectedTrackDescription = TrackDescription.GetNextOpenRoadStage(GameConfig.SelectedTrackDescription);
                        if (GameConfig.SelectedTrackDescription == null)
                            Engine.Instance.Screen = new HomeScreen();
                        else
                            Engine.Instance.Screen = new LoadRaceScreen();
                    }
                }
            }
        }

		public override void Draw()
		{
			base.Draw();

            Engine.Instance.SpriteBatch.Draw(_background, Vector2.Zero, new Color(255, 255, 255, 100));

			if (GameConfig.SelectedTrackDescription.IsOpenRoad)
                DrawOpenRoadResult();
            else
                DrawCircuitResult();

            Engine.Instance.SpriteBatch.End();
        }

        #endregion

        private void DrawCircuitResult()
        {
			WriteLine(GameConfig.SelectedTrackDescription.Name + "- Race completed", Color.Gray, 20, 30, TitleSize);
            
            int totalSeconds = 0;
            for (int i = 0; i < _race.PlayerStats.LapTimes.Count; i++)
            {
				WriteLine("Lap " + (i + 1) + ": " + TimeSpan.FromSeconds(_race.PlayerStats.LapTimes[i]).ToString());
                totalSeconds += _race.PlayerStats.LapTimes[i];
            }

			WriteLine("Total time: " + TimeSpan.FromSeconds(totalSeconds));
			
			WriteLine(" Race again", _selectedOption == 0, 60, 30, SectionSize);
			WriteLine(" Main menu", _selectedOption == 1, 30, 30, SectionSize);
        }

        private void DrawOpenRoadResult()
        {
			WriteLine(GameConfig.SelectedTrackDescription.Name + " - Stage completed", Color.Gray, 20, 30, TitleSize);
            
			WriteLine("Time: " + TimeSpan.FromSeconds(_race.PlayerStats.LapTimes[0]));

            if (TrackDescription.GetNextOpenRoadStage(GameConfig.SelectedTrackDescription) != null)
			{
				WriteLine("Continue to next stage", _selectedOption == 0, 60, 30, SectionSize);
            }

			WriteLine(" Main menu", _selectedOption == 1, 30, 30, SectionSize);
        }
    }
}
