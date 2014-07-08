using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NfsEngine;
using OpenNFS1.Parsers;

namespace OpenNFS1.UI.Screens
{
	class RaceOptionsScreen: BaseUIScreen, IGameScreen
	{
		int _selectedOption, _gearboxOption, _timeOfDayOption;

		public RaceOptionsScreen() : base()
		{
			_selectedOption = 2;
			_gearboxOption = GameConfig.ManualGearbox ? 1 : 0;
			_timeOfDayOption = GameConfig.AlternativeTimeOfDay ? 1 : 0;
		}

		public void Update(GameTime gameTime)
		{
			if (Engine.Instance.Input.WasPressed(Keys.Down))
			{
				_selectedOption = Math.Min(_selectedOption + 1, 2);
			}
			else if (Engine.Instance.Input.WasPressed(Keys.Up))
			{
				_selectedOption = Math.Max(_selectedOption - 1, 0);
			}
			else if (Engine.Instance.Input.WasPressed(Keys.Left))
			{
				if (_selectedOption == 0)
					_gearboxOption = 0;
				else
					_timeOfDayOption = 0;
			}
			else if (Engine.Instance.Input.WasPressed(Keys.Right))
			{
				if (_selectedOption == 0)
					_gearboxOption = 1;
				else
					_timeOfDayOption = 1;
			}
			else if (Engine.Instance.Input.WasPressed(Keys.Enter))
			{
				if (_selectedOption == 2)
				{
					GameConfig.ManualGearbox = _gearboxOption == 1;
					GameConfig.AlternativeTimeOfDay = _timeOfDayOption == 1;
					Engine.Instance.Screen = new LoadRaceScreen();
				}
			}
		}

		public override void Draw()
		{
			base.Draw();
			
			WriteLine("Race settings", Color.White, 20, 30, TitleSize);

			WriteLine("Gearbox", _selectedOption == 0, 60, 30, SectionSize);
			WriteLine("Auto", _gearboxOption == 0, 40, 40, TextSize);
			WriteLine("Manual", _gearboxOption == 1, 0, 120, TextSize);
			
			WriteLine("Time of day", _selectedOption == 1, 40, 30, SectionSize);
			WriteLine("Midday", _timeOfDayOption == 0, 40, 40, TextSize);
			if (GameConfig.SelectedTrackDescription.AlternativeTimeOfDay != null)
			{
				WriteLine(GameConfig.SelectedTrackDescription.AlternativeTimeOfDay, _timeOfDayOption == 1, 0, 120, TextSize);
			}

			WriteLine("Go!", _selectedOption == 2, 40, 30, SectionSize);

			Engine.Instance.SpriteBatch.End();
		}
	}
}
