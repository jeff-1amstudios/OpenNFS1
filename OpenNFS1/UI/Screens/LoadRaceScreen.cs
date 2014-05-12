using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed.Parsers;
using NeedForSpeed.Parsers.Track;
using Microsoft.Xna.Framework.Input;
using NfsEngine;
using NeedForSpeed.Loaders;
using NeedForSpeed.UI;
using NeedForSpeed.Vehicles;
using NeedForSpeed.Physics;
using System.Diagnostics;
using NeedForSpeed.UI.Screens;
using System.Threading;
using OpenNFS1.Parsers;


namespace NeedForSpeed
{
    class LoadRaceScreen : BaseUIScreen, IGameScreen
    {
        private Track _track;
        

        public LoadRaceScreen() : base(false)
        {
            //new Thread(LoadTrack).Start();
        }

        #region IDrawableObject Members

        public void Update(GameTime gameTime)
        {
			LoadTrack();
            if (_track != null)
            {
                Engine.Instance.Mode = new DoRaceScreen(_track);
            }
        }

        public void Draw()
        {
            Engine.Instance.SpriteBatch.Begin();

            Engine.Instance.SpriteBatch.DrawString(Font, String.Format("Loading {0}...", GameConfig.SelectedTrack.Name), new Vector2(50, 200), Color.WhiteSmoke, 0, Vector2.Zero, 1.3f, SpriteEffects.None, 0);
            Engine.Instance.SpriteBatch.End();
        }

        #endregion

        private void LoadTrack()
        {
			TriFile tri = new TriFile(GameConfig.SelectedTrack.FileName);
            _track = new TrackAssembler().Assemble(tri);
            
        }
    }
}
