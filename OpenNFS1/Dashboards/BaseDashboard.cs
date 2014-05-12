using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using NeedForSpeed.Parsers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NfsEngine;
using NeedForSpeed.Physics;
using System.Diagnostics;

namespace NeedForSpeed.Dashboards
{
    class DashboardBitmap
    {
        public BitmapEntry BitmapEntry;
        public Rectangle DrawRect;
        
        public DashboardBitmap(BitmapEntry bitmap, Vector2 screenSize)
        {
            BitmapEntry = bitmap;

            if (bitmap == null)
            {
                DrawRect = new Rectangle();
                return;
            }

            BitmapEntry.DisplayAt.X = (BitmapEntry.DisplayAt.X / 640) * screenSize.X;
            BitmapEntry.DisplayAt.Y = (BitmapEntry.DisplayAt.Y / 480) * screenSize.Y;

            DrawRect = new Rectangle((int)BitmapEntry.DisplayAt.X, (int)BitmapEntry.DisplayAt.Y,
                (int)(((float)BitmapEntry.Texture.Width / 640) * screenSize.X),
                (int)(((float)BitmapEntry.Texture.Height / 480) * screenSize.Y));

        }
    }

	abstract class BaseDashboard
	{
        protected Vehicle _car;
        protected Vector2 _screenSize;
		protected BitmapChunk _bitmapChunk;
        protected Texture2D _instrumentLine;
        GearboxAnimation _gearBoxAnimation;
        public bool IsVisible { get; set; }
        
        public DashboardBitmap Dash, Gate, Knob, Wstr, Wl06, Wl14, Wl22, Wl32, Wl45, Wr06, Wr14, Wr22, Wr32, Wr45;
        public DashboardBitmap Leather1, Leather2, Leather3;

        public BaseDashboard(Vehicle car, string filename)
        {
            _car = car;
            _car.Motor.Gearbox.GearChangeStarted += new EventHandler(Gearbox_GearChangeStarted);
            
            _gearBoxAnimation = new GearboxAnimation();
            
            _instrumentLine = Engine.Instance.ContentManager.Load<Texture2D>("Content\\SpeedoLine");

			string dashboardFile = Path.Combine(GameConfig.CdDataPath, filename);
            BinaryReader br = new BinaryReader(File.Open(dashboardFile, FileMode.Open));
            BitmapChunk.ResetPalette();
            _bitmapChunk = new BitmapChunk();
            _bitmapChunk.SkipHeader(br);
            _bitmapChunk.Read(br);
            br.Close();

            _screenSize.Y = Engine.Instance.Device.Viewport.Height;
            _screenSize.X = Engine.Instance.Device.Viewport.Width;

            Dash = new DashboardBitmap(_bitmapChunk.FindByName("dash"), _screenSize);
            Gate = new DashboardBitmap(_bitmapChunk.FindByName("gate"), _screenSize);
            Knob = new DashboardBitmap(_bitmapChunk.FindByName("nob1"), _screenSize);

            Leather1 = new DashboardBitmap(_bitmapChunk.FindByName("lth1"), _screenSize);
            Leather2 = new DashboardBitmap(_bitmapChunk.FindByName("lth2"), _screenSize);
            Leather3 = new DashboardBitmap(_bitmapChunk.FindByName("lth3"), _screenSize);

            Wstr = new DashboardBitmap(_bitmapChunk.FindByName("wstr"), _screenSize);
            Wl06 = new DashboardBitmap(_bitmapChunk.FindByName("wl06"), _screenSize);
            Wl14 = new DashboardBitmap(_bitmapChunk.FindByName("wl14"), _screenSize);
            Wl22 = new DashboardBitmap(_bitmapChunk.FindByName("wl22"), _screenSize);
            Wl32 = new DashboardBitmap(_bitmapChunk.FindByName("wl32"), _screenSize);
            if (Wl32.BitmapEntry == null)
                Wl32 = Wl22;
            Wl45 = new DashboardBitmap(_bitmapChunk.FindByName("wl45"), _screenSize);
            if (Wl45.BitmapEntry == null)
                Wl45 = Wl32;
            Wr06 = new DashboardBitmap(_bitmapChunk.FindByName("wr06"), _screenSize);
            Wr14 = new DashboardBitmap(_bitmapChunk.FindByName("wr14"), _screenSize);
            Wr22 = new DashboardBitmap(_bitmapChunk.FindByName("wr22"), _screenSize);
            Wr32 = new DashboardBitmap(_bitmapChunk.FindByName("wr32"), _screenSize);
            if (Wr32.BitmapEntry == null)
                Wr32 = Wr22;
            Wr45 = new DashboardBitmap(_bitmapChunk.FindByName("wr45"), _screenSize);
            if (Wr45.BitmapEntry == null)
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
            //if (_gearBoxAnimation.IsAnimating)
            //{
                _gearBoxAnimation.Update(gameTime);
            //}
        }

        public virtual void Render()
        {
            Engine.Instance.SpriteBatch.Draw(Dash.BitmapEntry.Texture, Dash.DrawRect, Color.White);

            if (_gearBoxAnimation.IsAnimating)
            {
                RenderGearstick();
            }
        }

        public void RenderGearstick()
        {
            Engine.Instance.SpriteBatch.Draw(Gate.BitmapEntry.Texture, Gate.DrawRect, Color.White);
            Rectangle rect;

            if (Leather1.BitmapEntry != null)
            {
                rect = Leather1.DrawRect;
                rect.X = Gate.DrawRect.X + Gate.DrawRect.Width / 2 - Leather1.DrawRect.Width / 2;
                rect.Y = Gate.DrawRect.Y + Gate.DrawRect.Height / 2 - Leather1.DrawRect.Width / 2;
                rect.Offset((int)(_gearBoxAnimation.CurrentPosition.X * 0.1f), (int)(_gearBoxAnimation.CurrentPosition.Y * 0.1f));
                Engine.Instance.SpriteBatch.Draw(Leather1.BitmapEntry.Texture, rect, Color.White);

                rect = Leather2.DrawRect;
                rect.X = Gate.DrawRect.X + Gate.DrawRect.Width / 2 - Leather2.DrawRect.Width / 2;
                rect.Y = Gate.DrawRect.Y + Gate.DrawRect.Height / 2 - Leather2.DrawRect.Width / 2;
                rect.Offset((int)(_gearBoxAnimation.CurrentPosition.X * 0.4f), (int)(_gearBoxAnimation.CurrentPosition.Y * 0.4f));
                Engine.Instance.SpriteBatch.Draw(Leather2.BitmapEntry.Texture, rect, Color.White);

                rect = Leather3.DrawRect;
                rect.X = Gate.DrawRect.X + Gate.DrawRect.Width / 2 - Leather3.DrawRect.Width / 2;
                rect.Y = Gate.DrawRect.Y + Gate.DrawRect.Height / 2 - Leather3.DrawRect.Width / 2;
                rect.Offset((int)(_gearBoxAnimation.CurrentPosition.X * 0.7f), (int)(_gearBoxAnimation.CurrentPosition.Y * 0.7f));
                Engine.Instance.SpriteBatch.Draw(Leather3.BitmapEntry.Texture, rect, Color.White);
            }

            rect = Knob.DrawRect;
            rect.X = Gate.DrawRect.X + Gate.DrawRect.Width / 2 - Knob.DrawRect.Width / 2;
            rect.Y = Gate.DrawRect.Y + Gate.DrawRect.Height / 2 - Knob.DrawRect.Width / 2;
            rect.Offset((int)_gearBoxAnimation.CurrentPosition.X, (int)_gearBoxAnimation.CurrentPosition.Y);
            Engine.Instance.SpriteBatch.Draw(Knob.BitmapEntry.Texture, rect, Color.White);
        }

        public void RenderSteeringWheel()
        {
            float steeringFactor = _car._steeringWheel / _car.MaxSteeringLock;

            if (steeringFactor < -0.8f)
                Engine.Instance.SpriteBatch.Draw(Wl45.BitmapEntry.Texture, Wl45.DrawRect, Color.White);
            else if (steeringFactor < -0.64f)
                Engine.Instance.SpriteBatch.Draw(Wl32.BitmapEntry.Texture, Wl32.DrawRect, Color.White);
            else if (steeringFactor < -0.48f)
                Engine.Instance.SpriteBatch.Draw(Wl22.BitmapEntry.Texture, Wl22.DrawRect, Color.White);
            else if (steeringFactor < -0.32f)
                Engine.Instance.SpriteBatch.Draw(Wl14.BitmapEntry.Texture, Wl14.DrawRect, Color.White);
            else if (steeringFactor < -0.16f)
                Engine.Instance.SpriteBatch.Draw(Wl06.BitmapEntry.Texture, Wl06.DrawRect, Color.White);

            else if (steeringFactor > 0.8f)
                Engine.Instance.SpriteBatch.Draw(Wr45.BitmapEntry.Texture, Wr45.DrawRect, Color.White);
            else if (steeringFactor > 0.64f)
                Engine.Instance.SpriteBatch.Draw(Wr32.BitmapEntry.Texture, Wr32.DrawRect, Color.White);
            else if (steeringFactor > 0.48f)
                Engine.Instance.SpriteBatch.Draw(Wr22.BitmapEntry.Texture, Wr22.DrawRect, Color.White);
            else if (steeringFactor > 0.32f)
                Engine.Instance.SpriteBatch.Draw(Wr14.BitmapEntry.Texture, Wr14.DrawRect, Color.White);
            else if (steeringFactor > 0.16f)
                Engine.Instance.SpriteBatch.Draw(Wr06.BitmapEntry.Texture, Wr06.DrawRect, Color.White);
            else
                Engine.Instance.SpriteBatch.Draw(Wstr.BitmapEntry.Texture, Wstr.DrawRect, Color.White);
        }
	}
}
