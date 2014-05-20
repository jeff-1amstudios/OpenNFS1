using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using OpenNFS1.Parsers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NfsEngine;
using OpenNFS1.Physics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace OpenNFS1.Dashboards
{
	class Dashboard
	{
        protected Vehicle _car;
        protected Texture2D _instrumentLine;
        GearboxAnimation _gearBoxAnimation;
        public bool IsVisible { get; set; }
		DashboardDescription _descriptor;
        
        BitmapEntry Dash, GearGate, GearKnob, Wstr, Wl06, Wl14, Wl22, Wl32, Wl45, Wr06, Wr14, Wr22, Wr32, Wr45;
		BitmapEntry Leather1, Leather2, Leather3;

		public Dashboard(Vehicle car, DashboardDescription descriptor)
        {
            _car = car;
			_descriptor = descriptor;
            _car.Motor.Gearbox.GearChangeStarted += new EventHandler(Gearbox_GearChangeStarted);
            
            _gearBoxAnimation = new GearboxAnimation();
            
            _instrumentLine = Engine.Instance.ContentManager.Load<Texture2D>("Content\\SpeedoLine");

			string dashboardFile = Path.Combine(GameConfig.CdDataPath, Path.Combine(@"SIMDATA\DASH", descriptor.Filename));
            BinaryReader br = new BinaryReader(File.Open(dashboardFile, FileMode.Open));
            BitmapChunk.ResetPalette();
            var bitmaps = new BitmapChunk();
			bitmaps.SkipHeader(br);
			bitmaps.Read(br);
            br.Close();

            Dash = bitmaps.FindByName("dash");
            GearGate = bitmaps.FindByName("gate");
            GearKnob = bitmaps.FindByName("nob1");

            Leather1 = bitmaps.FindByName("lth1");
            Leather2 = bitmaps.FindByName("lth2");
            Leather3 = bitmaps.FindByName("lth3");

			//steering wheel images, from straight to left, then to the right.  Not all cars have all steering angles
            Wstr = bitmaps.FindByName("wstr");
            Wl06 = bitmaps.FindByName("wl06");
            Wl14 = bitmaps.FindByName("wl14");
            Wl22 = bitmaps.FindByName("wl22");
            Wl32 = bitmaps.FindByName("wl32");
            if (Wl32 == null)
                Wl32 = Wl22;
            Wl45 = bitmaps.FindByName("wl45");
            if (Wl45 == null)
                Wl45 = Wl32;
            Wr06 = bitmaps.FindByName("wr06");
            Wr14 = bitmaps.FindByName("wr14");
            Wr22 = bitmaps.FindByName("wr22");
            Wr32 = bitmaps.FindByName("wr32");
            if (Wr32 == null)
                Wr32 = Wr22;
            Wr45 = bitmaps.FindByName("wr45");
            if (Wr45 == null)
                Wr45 = Wr32;
        }


        void Gearbox_GearChangeStarted(object sender, EventArgs e)
        {
            if (IsVisible)
            {
                _gearBoxAnimation.Current = _car.Motor.Gearbox.CurrentGear + 1;
                _gearBoxAnimation.Next = _car.Motor.Gearbox.NextGear + 1;
            }
        }

		public void Update(GameTime gameTime)
		{
			_gearBoxAnimation.Update(gameTime);
			if (Engine.Instance.Input.WasPressed(Keys.NumPad8))
				_descriptor.TachPosition.Y += Engine.Instance.FrameTime;
			if (Engine.Instance.Input.WasPressed(Keys.NumPad2))
				_descriptor.TachPosition.Y -= Engine.Instance.FrameTime;
			if (Engine.Instance.Input.WasPressed(Keys.NumPad4))
				_descriptor.TachPosition.X -= Engine.Instance.FrameTime;
			if (Engine.Instance.Input.WasPressed(Keys.NumPad6))
				_descriptor.TachPosition.X += Engine.Instance.FrameTime;
		}

        public void Render()
        {
            Engine.Instance.SpriteBatch.Draw(Dash.Texture, Dash.GetDisplayAt(), Color.White);

            if (_gearBoxAnimation.IsAnimating)
            {
                RenderGearstick();
            }

			Color color = new Color(165, 0, 0, 255);
			float rpmFactor = _car.Motor.Rpm / _car.Motor.RedlineRpm;
			Vector2 revCounterPosition = _descriptor.TachPosition;
			float rotation = (float)(rpmFactor * Math.PI * _descriptor.TachRpmMultiplier) - _descriptor.TachIdlePosition;

			RenderSteeringWheel();

			Engine.Instance.SpriteBatch.Draw(_instrumentLine, revCounterPosition, null, color, rotation, new Vector2(5, 25), new Vector2(0.8f, _descriptor.TachNeedleLength), SpriteEffects.None, 0);
        }

        public void RenderGearstick()
        {
            Engine.Instance.SpriteBatch.Draw(GearGate.Texture, GearGate.GetDisplayAt(), Color.White);

			// leather gearstick boot
            if (Leather1 != null)
            {
				var gearAnim = _gearBoxAnimation.CurrentPosition;
				Engine.Instance.SpriteBatch.Draw(Leather1.Texture, Leather1.GetDisplayAt() + gearAnim * 0.1f, Color.White);
				Engine.Instance.SpriteBatch.Draw(Leather2.Texture, Leather2.GetDisplayAt() + gearAnim * 0.4f, Color.White);
				Engine.Instance.SpriteBatch.Draw(Leather3.Texture, Leather3.GetDisplayAt() + gearAnim * 0.7f, Color.White);
            }

			//rect = Knob.GetDisplayAt();
			//rect.X = Gate.GetDisplayAt().X + Gate.GetDisplayAt().Width / 2 - Knob.GetDisplayAt().Width / 2;
			//rect.Y = Gate.GetDisplayAt().Y + Gate.GetDisplayAt().Height / 2 - Knob.GetDisplayAt().Width / 2;
			//rect.Offset((int)_gearBoxAnimation.CurrentPosition.X, (int)_gearBoxAnimation.CurrentPosition.Y);
			Vector2 disp = GearKnob.GetDisplayAt();
			disp += _gearBoxAnimation.CurrentPosition;
            Engine.Instance.SpriteBatch.Draw(GearKnob.Texture, disp, Color.White);
        }

        public void RenderSteeringWheel()
        {
            float steeringFactor = _car._steeringWheel / _car.MaxSteeringLock;

            if (steeringFactor < -0.8f)
                Engine.Instance.SpriteBatch.Draw(Wl45.Texture, Wl45.GetDisplayAt(), Color.White);
            else if (steeringFactor < -0.64f)
                Engine.Instance.SpriteBatch.Draw(Wl32.Texture, Wl32.GetDisplayAt(), Color.White);
            else if (steeringFactor < -0.48f)
                Engine.Instance.SpriteBatch.Draw(Wl22.Texture, Wl22.GetDisplayAt(), Color.White);
            else if (steeringFactor < -0.32f)
                Engine.Instance.SpriteBatch.Draw(Wl14.Texture, Wl14.GetDisplayAt(), Color.White);
            else if (steeringFactor < -0.16f)
                Engine.Instance.SpriteBatch.Draw(Wl06.Texture, Wl06.GetDisplayAt(), Color.White);

            else if (steeringFactor > 0.8f)
                Engine.Instance.SpriteBatch.Draw(Wr45.Texture, Wr45.GetDisplayAt(), Color.White);
            else if (steeringFactor > 0.64f)
                Engine.Instance.SpriteBatch.Draw(Wr32.Texture, Wr32.GetDisplayAt(), Color.White);
            else if (steeringFactor > 0.48f)
                Engine.Instance.SpriteBatch.Draw(Wr22.Texture, Wr22.GetDisplayAt(), Color.White);
            else if (steeringFactor > 0.32f)
                Engine.Instance.SpriteBatch.Draw(Wr14.Texture, Wr14.GetDisplayAt(), Color.White);
            else if (steeringFactor > 0.16f)
                Engine.Instance.SpriteBatch.Draw(Wr06.Texture, Wr06.GetDisplayAt(), Color.White);
            else
                Engine.Instance.SpriteBatch.Draw(Wstr.Texture, Wstr.GetDisplayAt(), Color.White);
        }
	}
}
