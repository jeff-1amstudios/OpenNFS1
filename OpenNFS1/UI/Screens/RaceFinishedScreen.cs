using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.Parsers.Track;

namespace NeedForSpeed.UI.Screens
{
    class RaceFinishedScreen : BaseUIScreen, IGameScreen
    {
        Race _race;
        Texture2D _background;
        Track _raceTrack;

        int _selectedOption = 0;

        public RaceFinishedScreen(Race race, Track raceTrack)
            : base(false)
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
                Engine.Instance.Mode = new ChooseTrackScreen();
            }

            if (UIController.Up && _selectedOption > 0)
                _selectedOption--;
            else if (UIController.Down && _selectedOption < 1)
                _selectedOption++;

            if (UIController.Ok)
            {
                if (_selectedOption == 0)
                    Engine.Instance.Mode = new ChooseTrackScreen();
                else
                {
                    if (!_race.TrackDescription.IsOpenRoad)
                        Engine.Instance.Mode = new DoRaceScreen(_raceTrack);
                    else
                    {
                        GameConfig.SelectedTrack = TrackDescription.GetNextOpenRoadStage(GameConfig.SelectedTrack);
                        if (GameConfig.SelectedTrack == null)
                            Engine.Instance.Mode = new ChooseTrackScreen();
                        else
                            Engine.Instance.Mode = new LoadRaceScreen();
                    }
                }
            }
        }

        public void Draw()
        {
            Engine.Instance.SpriteBatch.Begin();

            Engine.Instance.SpriteBatch.Draw(_background, Vector2.Zero, new Color(255, 255, 255, 100));

            if (_race.TrackDescription.IsOpenRoad)
                DrawOpenRoadResult();
            else
                DrawCircuitResult();

            Engine.Instance.SpriteBatch.End();
        }

        #endregion

        private void DrawCircuitResult()
        {
            int y = 80;
            Engine.Instance.SpriteBatch.DrawString(Font, "Race completed!", new Vector2(50, y), Color.WhiteSmoke, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);
            y += 70;
            Engine.Instance.SpriteBatch.DrawString(Font, _race.TrackDescription.Name, new Vector2(100, y), Color.WhiteSmoke, 0, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
            y += 20;
            int totalSeconds = 0;
            for (int i = 0; i < _race.LapTimes.Count; i++)
            {
                y += 40;
                Engine.Instance.SpriteBatch.DrawString(Font, "Lap " + (i + 1) + ": " + TimeSpan.FromSeconds(_race.LapTimes[i]).ToString(), new Vector2(150, y), Color.WhiteSmoke, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);
                totalSeconds += _race.LapTimes[i];
            }

            y += 50;
            Engine.Instance.SpriteBatch.DrawString(Font, "Total: " + TimeSpan.FromSeconds(totalSeconds), new Vector2(150, y), Color.WhiteSmoke, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);

            y += 100;
            Engine.Instance.SpriteBatch.DrawString(Font, " Main menu", new Vector2(200, y), _selectedOption == 0 ? Color.Yellow : Color.WhiteSmoke, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
            y += 50;
            Engine.Instance.SpriteBatch.DrawString(Font, " Play again", new Vector2(200, y), _selectedOption == 1 ? Color.Yellow : Color.WhiteSmoke, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
            
        }

        private void DrawOpenRoadResult()
        {
            int y = 80;
            Engine.Instance.SpriteBatch.DrawString(Font, "Stage completed!", new Vector2(50, y), Color.WhiteSmoke, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);
            y += 70;
            Engine.Instance.SpriteBatch.DrawString(Font, _race.TrackDescription.Name, new Vector2(100, y), Color.WhiteSmoke, 0, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
            y += 20;
                        
            y += 50;
            Engine.Instance.SpriteBatch.DrawString(Font, "Time: " + TimeSpan.FromSeconds(_race.LapTimes[0]), new Vector2(150, y), Color.WhiteSmoke, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0);

            y += 100;
            
            if (TrackDescription.GetNextOpenRoadStage(GameConfig.SelectedTrack) != null)
            {
                Engine.Instance.SpriteBatch.DrawString(Font, " Main menu", new Vector2(200, y), _selectedOption == 0 ? Color.Yellow : Color.WhiteSmoke, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
                y += 50;
                Engine.Instance.SpriteBatch.DrawString(Font, " Continue to next stage", new Vector2(200, y), _selectedOption == 1 ? Color.Yellow : Color.WhiteSmoke, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
            }
            else
                Engine.Instance.SpriteBatch.DrawString(Font, " Main menu", new Vector2(200, y), Color.Yellow, 0, Vector2.Zero, 1.2f, SpriteEffects.None, 0);
        }
    }
}
