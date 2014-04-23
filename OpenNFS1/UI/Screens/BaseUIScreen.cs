using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Media;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;

namespace NeedForSpeed.UI.Screens
{
    class BaseUIScreen
    {
        private static SpriteFont _font;

        public static SpriteFont Font
        {
            get
            {
                if (_font == null)
                    _font = Engine.Instance.ContentManager.Load<SpriteFont>("Content\\ArialBlack");
                return _font;
            }
        }

        private static Song _music;

        protected static Song Music
        {
            get { return BaseUIScreen._music; }
            set { BaseUIScreen._music = value; }
        }

        public BaseUIScreen(bool playMusic)
        {
            if (playMusic)
            {
                if (_music == null)
                {
                    //_music = Engine.Instance.ContentManager.Load<Song>("Content/Audio/Music/menu");
                    //MediaPlayer.Play(_music);
                    //MediaPlayer.IsRepeating = true;
                }
            }
        }
    }
}
