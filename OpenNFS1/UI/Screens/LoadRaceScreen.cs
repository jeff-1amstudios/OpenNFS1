using System;
using System.Collections.Generic;
using System.Text;
using NfsEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1.Parsers;
using OpenNFS1.Parsers.Track;
using Microsoft.Xna.Framework.Input;
using NfsEngine;
using OpenNFS1.Loaders;
using OpenNFS1.UI;
using OpenNFS1.Vehicles;
using OpenNFS1.Physics;
using System.Diagnostics;
using OpenNFS1.UI.Screens;
using System.Threading;
using OpenNFS1.Parsers;


namespace OpenNFS1
{
    class LoadRaceScreen : BaseUIScreen, IGameScreen
    {
        private Track _track;
		float _loadingTime;
		int _nbrDots = 0;

        public LoadRaceScreen() : base(false)
        {
            new Thread(LoadTrack).Start();
        }

        #region IDrawableObject Members

        public void Update(GameTime gameTime)
        {
			//LoadTrack();

            if (_track != null)
            {
                Engine.Instance.Mode = new DoRaceScreen(_track);
            }
			_loadingTime += Engine.Instance.FrameTime;
			if (_loadingTime > 0.1f)
			{
				_nbrDots++;
				_loadingTime = 0;
			}
        }

        public void Draw()
        {
            Engine.Instance.SpriteBatch.Begin();

			
            Engine.Instance.SpriteBatch.DrawString(Font, String.Format("Loading {0}" + new string('.', _nbrDots), GameConfig.SelectedTrack.Name), new Vector2(50, 200), Color.WhiteSmoke, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            Engine.Instance.SpriteBatch.End();
        }

        #endregion

        private void LoadTrack()
        {
			TriFile tri = new TriFile(GameConfig.SelectedTrack.FileName);
            _track = new TrackAssembler().Assemble(tri);
			_track.Description = GameConfig.SelectedTrack;
            
        }
    }
}
