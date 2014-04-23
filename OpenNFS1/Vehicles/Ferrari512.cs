using System;
using System.Collections.Generic;
using System.Text;
using NeedForSpeed.Physics;
using Microsoft.Xna.Framework;
using NeedForSpeed.Parsers;
using NfsEngine;
using NeedForSpeed.Dashboards;
using Microsoft.Xna.Framework.Graphics;

namespace NeedForSpeed.Vehicles
{
    class Ferrari512 : Vehicle
    {
        private CarModel _model;

        public Ferrari512()
            : base(1380, "Ferrari512")
        {

            _model = CarModelCache.GetModel(@"Data\Cars\f512tr.CFM");

            _wheels[0] = new VehicleWheel(this, new Vector3(-9.5f, 0f, 12.2f), _model.TyreTexture, 6.3f);
            _wheels[1] = new VehicleWheel(this, new Vector3(9.5f, 0f, 12.2f), _model.TyreTexture, 6.3f);
            _wheels[2] = new VehicleWheel(this, new Vector3(-9.0f, 0f, -15.9f), _model.TyreTexture, 6.5f);
            _wheels[3] = new VehicleWheel(this, new Vector3(9.0f, 0f, -15.9f), _model.TyreTexture, 6.5f);

            _bodyRideHeight = 0.0f;
        }


				public override void Render()
				{
					RenderShadow();
					_effect.View = Engine.Instance.Camera.View;
					_effect.Projection = Engine.Instance.Camera.Projection;
					_effect.CurrentTechnique.Passes[0].Apply();
					_effect.World = Matrix.CreateScale(0.09f) * _renderMatrix;
					_model.Render(_effect, VehicleController.Brake > 0);
					base.Render();
				}

        internal override BaseDashboard Dashboard
        {
            get { return new F512Dashboard(this); }
        }

        public override void InitializeForDriving()
        {
            base.InitializeForDriving();
            List<float> power = new List<float>(new float[] { 0.2f, 0.3f, 0.4f, 0.7f, 0.8f, 1.0f, 1.0f, 1.0f, 0.8f, 0.7f });
            List<float> ratios = new List<float>(new float[] { 3.827f, 2.360f, 1.685f, 1.312f, 1.000f, 0.793f });

            BaseGearbox gearbox = BaseGearbox.Create(GameConfig.ManualGearbox, ratios, 0.4f);
            _motor = new Motor(power, 8, 8f, gearbox);
            _motor.Gearbox.GearChangeStarted += new EventHandler(Gearbox_GearChanged);
            _traction = (_motor.GetPowerAtRpmForGear(_motor.RedlineRpm, 2) * 30) - 30;
        }

        public override string Name
        {
            get
            {
                return "1993 Ferrari 512TR";
            }
        }

        public override string Description
        {
            get
            {
                return "Max power: 422hp\r\nEngine: 4.9l Flat 12\r\nWeight: 1656kg";
            }
        }
    }
}