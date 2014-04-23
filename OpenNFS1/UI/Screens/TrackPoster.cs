using System;
using System.Collections.Generic;

using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NfsEngine;
using NeedForSpeed.Parsers.Track;
using Microsoft.Xna.Framework.Input;

namespace NeedForSpeed.UI.Screens
{
    class TrackPoster : MenuPoster
    {
        TrackDescription _track;
        int _stage = 1;

        internal TrackDescription Track
        {
            get
            {
                if (!_track.IsOpenRoad)
                    return _track;
                else
                {
                    if (_stage == 1)
                        return _track;
                    else
                        return TrackDescription.Descriptions.Find(delegate(TrackDescription description)
                        {
                            return description.Name == _track.Name + " " + _stage;
                        });
                }
            }
        }
        
        public TrackPoster(TrackDescription track, Vector3 position)
            : base("", "", position)
        {
            _track = track;
        }

        public override void Render(AlphaTestEffect effect, bool selected)
        {
            if (selected)
            {
                if (UIController.Down && _stage < 3)
                    _stage++;
                else if (UIController.Up && _stage > 1)
                    _stage--;
            }
            _textureName = GetTextureName();
            base.Render(effect, selected);
        }

        private string GetTextureName()
        {
            if (_track.IsOpenRoad)
                return _track.ImageFile + "-" + _stage.ToString();
            else
                return _track.ImageFile;
        }

        public override string Title
        {
            get
            {
                if (_track.IsOpenRoad)
                    return "Open road - stage " + _stage;
                else
                    return "Circuit track - 2 laps";
            }
        }
    }
}
