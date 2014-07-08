using System;
using System.Collections.Generic;

using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Vehicles.AI;
using OpenNFS1.Parsers;
using OpenNFS1.Physics;

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

			int height = Engine.Instance.Device.Viewport.Height;
			int width = Engine.Instance.Device.Viewport.Width;

            _backgroundRectangle = new Rectangle(0, 0, width, 30);
			Color[] pixel = new Color[1] { Color.Black };
			_backgroundTexture = new Texture2D(Engine.Instance.Device, 1, 1);
			_backgroundTexture.SetData<Color>(pixel);
            _font = Engine.Instance.ContentManager.Load<SpriteFont>("Content\\ArialBlack");
        }

        public void Render()
        {
            Engine.Instance.SpriteBatch.Begin();

            int secondsTillStart = _race.SecondsTillStart;
            if (secondsTillStart > 0)
            {
                Engine.Instance.SpriteBatch.DrawString(_font, _race.SecondsTillStart.ToString(), new Vector2(375, 50), Color.Yellow, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            }
            else if (secondsTillStart == 0)
            {
                Engine.Instance.SpriteBatch.DrawString(_font, "Go!", new Vector2(375, 50), Color.Yellow, 0, Vector2.Zero, 2f, SpriteEffects.None, 0);
            }
			
            Engine.Instance.SpriteBatch.Draw(_backgroundTexture, _backgroundRectangle, Color.White);
			
            string msg = String.Format("{0}:{1}", _race.PlayerStats.CurrentLapTime.Minutes.ToString("00"), _race.PlayerStats.CurrentLapTime.Seconds.ToString("00"));
            Engine.Instance.SpriteBatch.DrawString(_font, msg, new Vector2(15, 0), Color.GreenYellow, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);

            msg = String.Format("{0} kph", Math.Abs((int)_race.Player.Vehicle.Speed));
            Engine.Instance.SpriteBatch.DrawString(_font, msg, new Vector2(270, -5), Color.WhiteSmoke, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
            Engine.Instance.SpriteBatch.DrawString(_font, msg, new Vector2(271, -4), Color.Red, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

            msg = String.Format("G:{0}", GearToString(((DrivableVehicle)_race.Player.Vehicle).Motor.Gearbox.CurrentGear));
            Engine.Instance.SpriteBatch.DrawString(_font, msg, new Vector2(410, 0), Color.GreenYellow, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);

			if (!GameConfig.SelectedTrackDescription.IsOpenRoad)
            {
				msg = String.Format("L:{0}/{1}", Math.Min(_race.PlayerStats.CurrentLap, _race.NbrLaps), _race.NbrLaps);
                Engine.Instance.SpriteBatch.DrawString(_font, msg, new Vector2(480, 0), Color.GreenYellow, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            }

			msg = String.Format("P:{0}/{1}", _race.PlayerStats.Position + 1, _race.Drivers.Count);
			Engine.Instance.SpriteBatch.DrawString(_font, msg, new Vector2(550, 0), Color.GreenYellow, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
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
