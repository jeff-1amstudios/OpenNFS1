using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using OpenNFS1.Physics;
using NfsEngine;
using Microsoft.Xna.Framework.Graphics;

namespace OpenNFS1
{
    class TyreSmokeParticleSystem : ParticleSystem
    {
        Vehicle _car;

        int _wheelSwitch;


        static TyreSmokeParticleSystem _instance;
        public static TyreSmokeParticleSystem Instance
        {
            get
            {
				if (_instance == null)
				{
					_instance = new TyreSmokeParticleSystem();
					_instance.InitializeSystem();
				}
                return _instance;
            }
        }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.Texture = Engine.Instance.ContentManager.Load<Texture2D>("Content/smoke");

            settings.MaxParticles = 200;
            
            settings.Duration = TimeSpan.FromSeconds(0.5f);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 15;

            settings.MinVerticalVelocity = 5;
            settings.MaxVerticalVelocity = 10;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(0, -3, 0);

            settings.EndVelocity = 0.75f;

            //settings.MinRotateSpeed = -1;
            //settings.MaxRotateSpeed = 1;

			settings.MinStartSize = 1;
			settings.MaxStartSize = 4;

			settings.MinEndSize = 3;
			settings.MaxEndSize = 7;
        }
    }
}
