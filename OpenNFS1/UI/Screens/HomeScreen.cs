using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NfsEngine;
using OpenNFS1.Parsers;
using OpenNFS1.Vehicles;

namespace OpenNFS1.UI.Screens
{
	class VehicleUIControl
	{
		public BitmapEntry Bitmap { get; private set; }
		public VehicleDescription Descriptor { get; private set; }

		public VehicleUIControl(VehicleDescription desc)
		{
			Descriptor = desc;
			QfsFile qfs = new QfsFile(@"Frontend\Art\Control\" + desc.UIImageFile);
			Bitmap = qfs.Fsh.Header.Bitmaps.Find(a => a.Id == "0000");
		}
	}

	class TrackUIControl
	{
		public BitmapEntry Bitmap { get; private set; }
		public TrackDescription Descriptor { get; private set; }

		public TrackUIControl(TrackDescription desc)
		{
			Descriptor = desc;
			QfsFile qfs = new QfsFile(@"Frontend\Art\Control\" + desc.ImageFile);
			Bitmap = qfs.Fsh.Header.Bitmaps.Find(a => a.Id == "0000");
		}
	}

	enum SelectedControlType
	{
		Vehicle,
		Track
	}

	class HomeScreen : IGameScreen
	{
		BitmapEntry _background, _vehicleSelection, _trackSelection, _raceButtonSelection;

		const int VehicleSelected = 0;
		const int TrackSelected = 1;
		const int RaceButtonSelected = 2;

		List<VehicleUIControl> _vehicles = new List<VehicleUIControl>();
		List<TrackUIControl> _track = new List<TrackUIControl>();
		int _currentVehicle = 2;
		int _currentTrack = 4;
		int _selectedControl = RaceButtonSelected;


		public HomeScreen()
			: base()
		{
			QfsFile qfs = new QfsFile(@"FRONTEND\ART\control\central.qfs");
			_background = qfs.Fsh.Header.FindByName("bgnd");
			_vehicleSelection = qfs.Fsh.Header.FindByName("Tlb1");
			_trackSelection = qfs.Fsh.Header.FindByName("Brb4");
			_raceButtonSelection = qfs.Fsh.Header.FindByName("Ra1l");

			foreach (var vehicle in VehicleDescription.Descriptions)
			{
				_vehicles.Add(new VehicleUIControl(vehicle));
			}

			foreach (var track in TrackDescription.Descriptions)
			{
				if (!track.HideFromMenu)
				{
					_track.Add(new TrackUIControl(track));
				}
			}

			if (GameConfig.SelectedTrackDescription != null)
				_currentTrack = _track.FindIndex(a => a.Descriptor == GameConfig.SelectedTrackDescription);
			if (GameConfig.SelectedVehicle != null)
				_currentVehicle = _vehicles.FindIndex(a => a.Descriptor == GameConfig.SelectedVehicle);

			if (_currentTrack == -1) _currentTrack = 0;
		}

		public void Update(GameTime gameTime)
		{
			switch (_selectedControl)
			{
				case VehicleSelected:
					if (Engine.Instance.Input.WasPressed(Keys.Left))
						_currentVehicle--; if (_currentVehicle < 0) _currentVehicle = _vehicles.Count-1;
					else if (Engine.Instance.Input.WasPressed(Keys.Right))
						_currentVehicle++; _currentVehicle %= _vehicles.Count;
					break;

				case TrackSelected:
					if (Engine.Instance.Input.WasPressed(Keys.Left))
						_currentTrack--; if (_currentTrack < 0) _currentTrack = _track.Count - 1;
					else if (Engine.Instance.Input.WasPressed(Keys.Right))
						_currentTrack = (_currentTrack + 1) % _track.Count;
					break;
			}

			if (Engine.Instance.Input.WasPressed(Keys.Down))
			{
				_selectedControl++; _selectedControl %= 3;
			}
			else if (Engine.Instance.Input.WasPressed(Keys.Up))
			{
				_selectedControl--; if (_selectedControl < 0) _selectedControl = 2;
			}
			else if (Engine.Instance.Input.WasPressed(Keys.Enter) && _selectedControl == RaceButtonSelected)
			{
				GameConfig.SelectedVehicle = _vehicles[_currentVehicle].Descriptor;
				GameConfig.SelectedTrackDescription = _track[_currentTrack].Descriptor;
				Engine.Instance.Screen = new RaceOptionsScreen();
			}
			else if (Engine.Instance.Input.WasPressed(Keys.Escape))
			{
				Engine.Instance.Game.Exit();
			}
		}

		public void Draw()
		{
			Engine.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
			Engine.Instance.SpriteBatch.Draw(_background.Texture, Vector2.Zero, Color.White);
			Engine.Instance.SpriteBatch.Draw(_vehicles[_currentVehicle].Bitmap.Texture, _vehicles[_currentVehicle].Bitmap.GetDisplayAt(), Color.White);
			Engine.Instance.SpriteBatch.Draw(_track[_currentTrack].Bitmap.Texture, _track[_currentTrack].Bitmap.GetDisplayAt(), Color.White);

			switch (_selectedControl)
			{
				case VehicleSelected:
					Engine.Instance.SpriteBatch.Draw(_vehicleSelection.Texture, _vehicleSelection.GetDisplayAt(), Color.White);
					break;
				case TrackSelected:
					Engine.Instance.SpriteBatch.Draw(_trackSelection.Texture, _trackSelection.GetDisplayAt(), Color.White);
					break;
				case RaceButtonSelected:
					Engine.Instance.SpriteBatch.Draw(_raceButtonSelection.Texture, _raceButtonSelection.GetDisplayAt(), Color.White);
					break;
			}
			Engine.Instance.SpriteBatch.End();
		}
	}
}
