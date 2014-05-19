using System;
using System.Collections.Generic;

using System.Text;
using Microsoft.Xna.Framework.Audio;
using NfsEngine;

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
        
        private EnvironmentAudioProvider()
        {
            _collisions = new List<SoundEffect>();
            _collisions.Add(Engine.Instance.ContentManager.Load<SoundEffect>("Content/Audio/Environment/collision1"));
            _collisions.Add(Engine.Instance.ContentManager.Load<SoundEffect>("Content/Audio/Environment/collision2"));
            _collisions.Add(Engine.Instance.ContentManager.Load<SoundEffect>("Content/Audio/Environment/collision3"));
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
