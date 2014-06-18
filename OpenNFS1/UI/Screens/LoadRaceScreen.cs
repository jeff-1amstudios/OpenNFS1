using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NfsEngine;
using OpenNFS1.Loaders;
using OpenNFS1.Parsers;
using OpenNFS1.Parsers.Track;
using OpenNFS1.UI.Screens;


namespace OpenNFS1
{
    class LoadRaceScreen : BaseUIScreen, IGameScreen
    {
		float _loadingTime;
		int _nbrDots = 0;

        public LoadRaceScreen() : base(false)
        {
			if (GameConfig.CurrentTrack != null)
			{
				GameConfig.CurrentTrack.Dispose();
				GameConfig.CurrentTrack = null;
			}
            new Thread(LoadTrack).Start();
        }

        #region IDrawableObject Members

        public void Update(GameTime gameTime)
        {
			if (GameConfig.CurrentTrack != null)
            {
				Engine.Instance.Mode = new DoRaceScreen(GameConfig.CurrentTrack);
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
            Engine.Instance.SpriteBatch.DrawString(Font, String.Format("Loading {0}" + new string('.', _nbrDots), GameConfig.SelectedTrackDescription.Name), new Vector2(50, 200), Color.WhiteSmoke, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            Engine.Instance.SpriteBatch.End();
        }

        #endregion

        private void LoadTrack()
        {
			TriFile tri = new TriFile(GameConfig.SelectedTrackDescription.FileName);
			GameConfig.CurrentTrack = new TrackAssembler().Assemble(tri);
			GameConfig.CurrentTrack.Description = GameConfig.SelectedTrackDescription;
            
        }
    }
}
