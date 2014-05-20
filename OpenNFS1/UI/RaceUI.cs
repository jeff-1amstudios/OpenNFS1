using System;
using System.Collections.Generic;

using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenNFS1.UI
{
    class RaceUI
    {
        Race _race;

        Rectangle _backgroundRectangle;
        Texture2D _backgroundTexture;
        SpriteFont _font;

        public RaceUI(Race race)
        {
            _race = race;

            int height =Engine.Instance.Device.Viewport.Height;
            int width = Engine.Instance.Device.Viewport.Width;

            _backgroundRectangle = new Rectangle(0, 0, width, 54);
            _backgroundTexture = Engine.Instance.ContentManager.Load<Texture2D>("Content\\RaceUI-background");
            _font = Engine.Instance.ContentManager.Load<SpriteFont>("Content\\ArialBlack");
        }

        public void Render()
        {
            Engine.Instance.SpriteBatch.Begin();

            int secondsTillStart = _race.SecondsTillStart;
            if (secondsTillStart > 0)
            {
                Engine.Instance.SpriteBatch.DrawString(_font, _race.SecondsTillStart.ToString(), new Vector2(500, 50), Color.Yellow, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            }
            else if (secondsTillStart == 0)
            {
                Engine.Instance.SpriteBatch.DrawString(_font, "Go!", new Vector2(500, 50), Color.Yellow, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            }

            //Color color = new Color(255, 255, 255, 150);
            //Engine.Instance.SpriteBatch.Draw(_backgroundTexture, _backgroundRectangle, color);

            string msg = String.Format("{0}:{1}", _race.CurrentLapTime.Minutes.ToString("00"), _race.CurrentLapTime.Seconds.ToString("00"));
            Engine.Instance.SpriteBatch.DrawString(_font, msg, new Vector2(20, 0), Color.GreenYellow, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);

			if (!GameConfig.SelectedTrack.IsOpenRoad)
                Engine.Instance.SpriteBatch.DrawString(_font, String.Format("{0}:{1}", _race.RaceTime.Minutes.ToString("00"), _race.RaceTime.Seconds.ToString("00")), new Vector2(20, 20), Color.DimGray, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);

            msg = String.Format("{0} kph", Math.Abs((int)_race.PlayerVehicle.Speed));
            Engine.Instance.SpriteBatch.DrawString(_font, msg, new Vector2(401, -5), Color.WhiteSmoke, 0, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
            Engine.Instance.SpriteBatch.DrawString(_font, msg, new Vector2(400, -4), Color.Red, 0, Vector2.Zero, 1.1f, SpriteEffects.None, 0);

            msg = String.Format("[{0}]", GearToString(_race.PlayerVehicle.Motor.Gearbox.CurrentGear));
            Engine.Instance.SpriteBatch.DrawString(_font, msg, new Vector2(580, 2), Color.GreenYellow, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

			if (!GameConfig.SelectedTrack.IsOpenRoad)
            {
                msg = String.Format("Lap {0}/{1}", Math.Min(_race.CurrentLap, _race.NbrLaps), _race.NbrLaps);
                Engine.Instance.SpriteBatch.DrawString(_font, msg, new Vector2(800, 5), Color.GreenYellow, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
            }
            Engine.Instance.SpriteBatch.End();
        }

        private string GearToString(int gear)
        {
            if (gear == -1)
                return "R";
            else if (gear == 0)
                return "N";
            else
                return gear.ToString();
        }
    }
}
