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
	class CorvetteZR1 : Vehicle
	{
		private CarModel _model;

		public CorvetteZR1()
			: base(1380, "CorvetteZR1")
		{

			_model = CarModelCache.GetModel(@"SIMDATA\CARFAMS\czr1.CFM");

			_wheels[0] = new VehicleWheel(this, new Vector3(-9.36f, 0f, 16.0f), _model.TyreTexture, 7.2f);
			_wheels[1] = new VehicleWheel(this, new Vector3(9.36f, 0f, 16.0f), _model.TyreTexture, 7.2f);
			_wheels[2] = new VehicleWheel(this, new Vector3(-9.33f, 0f, -13.5f), _model.TyreTexture, 7.2f);
			_wheels[3] = new VehicleWheel(this, new Vector3(9.33f, 0f, -13.5f), _model.TyreTexture, 7.2f);

			_bodyRideHeight = -0.3f;
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
			get { return new CorvetteZR1Dashboard(this); }
		}

		public override void InitializeForDriving()
		{
			base.InitializeForDriving();

			List<float> power = new List<float>(new float[] { 0.2f, 0.3f, 0.4f, 0.7f, 0.8f, 1.0f, 0.8f, 0.5f });
			List<float> ratios = new List<float>(new float[] { 3.827f, 2.360f, 1.685f, 1.312f, 1.000f, 0.793f });

			BaseGearbox gearbox = BaseGearbox.Create(GameConfig.ManualGearbox, ratios, 0.4f);
			_motor = new Motor(power, 6, 6.5f, gearbox);
			_motor.Gearbox.GearChangeStarted += new EventHandler(Gearbox_GearChanged);
			_traction = (_motor.GetPowerAtRpmForGear(_motor.RedlineRpm, 2) * 30) - 30;

		}

		public override string Name
		{
			get
			{
				return "1993 Corvette ZR1";
			}
		}

		public override string Description
		{
			get
			{
				return "Max power: 405hp\r\nEngine: 6.0l V8\r\nWeight: 1590kg";
			}
		}
	}
}