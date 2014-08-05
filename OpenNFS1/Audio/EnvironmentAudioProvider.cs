using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using GameEngine;

namespace OpenNFS1.Audio
{
    class EnvironmentAudioProvider
    {
        private static EnvironmentAudioProvider _instance;

        public static EnvironmentAudioProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EnvironmentAudioProvider();
                }
                return _instance;
            }
        }

        List<SoundEffect> _collisions;
        SoundEffectInstance _collisionInstance;

		public string BasePath { get { return GameConfig.CdDataPath + "/ConvertedAudio"; } }
        
        private EnvironmentAudioProvider()
        {
            _collisions = new List<SoundEffect>();
            _collisions.Add(Engine.Instance.ContentManager.Load<SoundEffect>(BasePath + "/Environment/collision1"));
			_collisions.Add(Engine.Instance.ContentManager.Load<SoundEffect>(BasePath + "/Environment/collision2"));
			_collisions.Add(Engine.Instance.ContentManager.Load<SoundEffect>(BasePath + "/Environment/collision3"));
        }

        public void PlayVehicleFenceCollision()
        {
            if (_collisionInstance != null && _collisionInstance.State == SoundState.Playing)
            {
                return;
            }
            _collisionInstance = _collisions[Utility.RandomGenerator.Next(_collisions.Count)].CreateInstance();
			_collisionInstance.Play();
        }

        public void PlayCollision(int index)
        {
            _collisions[index].Play(0.5f, 0, 0);
        }

        
    }
}
