using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine;
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
		TriFile _tri;
		TrackAssembler _assembler;

        public LoadRaceScreen() : base()
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
				Engine.Instance.Screen = new DoRaceScreen(GameConfig.CurrentTrack);
            }
			_loadingTime += Engine.Instance.FrameTime;
			if (_loadingTime > 0.1f)
			{
				_nbrDots++;
				_loadingTime = 0;
			}
        }

		public override void Draw()
		{
			base.Draw();
			WriteLine(GameConfig.SelectedTrackDescription.Name, Color.White, 20, 30, TitleSize);
			WriteLine(String.Format("Loading {0}", new string('.', _nbrDots)), TextColor, 50, 30, TextSize);
			
			WriteLine("Reading track file " + GameConfig.SelectedTrackDescription.FileName, TextColor, 20, 30, TextSize);
			if (_assembler != null)
				WriteLine(_assembler.ProgressMessage, TextColor, 20, 30, TextSize);
			
			Engine.Instance.SpriteBatch.End();
		}

        #endregion

        private void LoadTrack()
        {
			_tri = new TriFile(GameConfig.SelectedTrackDescription.FileName);
			_assembler = new TrackAssembler();
			GameConfig.CurrentTrack = _assembler.Assemble(_tri);
			GameConfig.CurrentTrack.Description = GameConfig.SelectedTrackDescription;
            
        }
    }
}
