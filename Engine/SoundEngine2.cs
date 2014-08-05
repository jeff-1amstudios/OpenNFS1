using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace GameEngine
{
    class SoundEffectDescriptor 
    {
        public SoundEffectInstance Effect;
        public float RemainingDuration;
    }

    public class SoundEngine2
    {
        private static SoundEngine2 _instance;

        public static SoundEngine2 Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SoundEngine2();
                return _instance;
            }
        }

        private List<SoundEffectDescriptor> _effects = new List<SoundEffectDescriptor>(); 

        private SoundEngine2()
        {
        }

        public void PlayEffect(SoundEffectInstance effect, float duration)
        {
            SoundEffectDescriptor sd = new SoundEffectDescriptor();
            sd.Effect = effect;
            sd.RemainingDuration = duration;
            _effects.Add(sd);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                _effects[i].RemainingDuration -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_effects[i].RemainingDuration <= 0)
                {
                    _effects[i].Effect.Stop();
                    _effects.RemoveAt(i);
                }
            }
        }
    }
}
