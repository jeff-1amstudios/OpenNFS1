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
    class Warrior : Vehicle
    {
        private CarModel _model;

        public Warrior()
            : base(1580, "Warrior")
        {

            _model = CarModelCache.GetModel(@"Data\Cars\traffc.CFM");

            _wheels[0] = new VehicleWheel(this, new Vector3(-10.8f, 0f, 12.9f), _model.TyreTexture, 7.2f);
            _wheels[1] = new VehicleWheel(this, new Vector3(10.8f, 0f, 12.9f), _model.TyreTexture, 7.2f);
            _wheels[2] = new VehicleWheel(this, new Vector3(-9.4f, 0f, -19.2f), _model.TyreTexture, 7.2f);
            _wheels[3] = new VehicleWheel(this, new Vector3(9.4f, 0f, -19.2f), _model.TyreTexture, 7.2f);

            _bodyRideHeight = -0.0f;
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
            get { return new WarriorDashboard(this); }
        }

        public override void InitializeForDriving()
        {
            base.InitializeForDriving();
            List<float> power = new List<float>(new float[] { 0.2f, 0.3f, 0.4f, 0.7f, 0.8f, 1.0f, 0.8f, 0.8f, 0 });
            List<float> ratios = new List<float>(new float[] { 3.827f, 2.360f, 1.585f, 1.212f, 0.900f, 0.643f });

            AutoGearbox gearbox = new AutoGearbox(ratios, 0.4f);
            _motor = new Motor(power, 14, 7f, gearbox);
            _motor.Gearbox.GearChangeStarted += new EventHandler(Gearbox_GearChanged);
            _traction = (_motor.GetPowerAtRpmForGear(_motor.RedlineRpm, 2) * 30) - 30;
        }

        public override string Name
        {
            get
            {
                return "Warrior (Secret car)";
            }
        }

        public override string Description
        {
            get
            {
                return "Max power: ?\r\nEngine: ?\r\nWeight: ?";
            }
        }
    }
}