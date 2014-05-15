//using System;
//using System.Collections.Generic;
//using System.Text;
//using NeedForSpeed.Physics;
//using Microsoft.Xna.Framework;
//using NeedForSpeed.Parsers;
//using NfsEngine;
//using NeedForSpeed.Dashboards;
//using Microsoft.Xna.Framework.Graphics;

//namespace NeedForSpeed.Vehicles
//{
//	class LamboDiablo : Vehicle
//	{
//		private CfmFile _model;

//		public LamboDiablo()
//			: base(1480, "LamboDiablo")
//		{

//			_model = CarModelCache.GetModel(@"SIMDATA\CARFAMS\ldiabl.CFM");

//			_wheels[0] = new VehicleWheel(this, new Vector3(-9.1f, 0f, 15.9f), _model.TyreTexture, 6.3f);
//			_wheels[1] = new VehicleWheel(this, new Vector3(9.1f, 0f, 15.9f), _model.TyreTexture, 6.3f);
//			_wheels[2] = new VehicleWheel(this, new Vector3(-10.35f, 0f, -15.9f), _model.TyreTexture, 7.0f);
//			_wheels[3] = new VehicleWheel(this, new Vector3(10.35f, 0f, -15.9f), _model.TyreTexture, 7.0f);

//			_bodyRideHeight = -0.5f;
//		}


//				public override void Render()
//				{
//					RenderShadow();
//					_effect.View = Engine.Instance.Camera.View;
//					_effect.Projection = Engine.Instance.Camera.Projection;
//					_effect.CurrentTechnique.Passes[0].Apply();
//					_effect.World = Matrix.CreateScale(0.09f) * _renderMatrix;
//					_model.Render(_effect, VehicleController.Brake > 0);
//					base.Render();
//				}

//		internal override BaseDashboard Dashboard
//		{
//			get { return new DiabloDashboard(this); }
//		}

//		public override void InitializeForDriving()
//		{
//			base.InitializeForDriving();
//			List<float> power = new List<float>(new float[] { 0.2f, 0.3f, 0.4f, 0.7f, 0.8f, 1.0f, 1.0f, 1.0f, 1.0f });
//			List<float> ratios = new List<float>(new float[] { 3.827f, 2.360f, 1.685f, 1.312f, 1.000f, 0.743f });

//			BaseGearbox gearbox = BaseGearbox.Create(GameConfig.ManualGearbox, ratios, 0.4f);
//			_motor = new Motor(power, 9, 7.5f, gearbox);
//			_motor.Gearbox.GearChangeStarted += new EventHandler(Gearbox_GearChanged);
//			_traction = (_motor.GetPowerAtRpmForGear(_motor.RedlineRpm, 2) * 30) - 30;
//		}

//		public override string Name
//		{
//			get
//			{
//				return "1993 Lamborghini Diablo VT";
//			}
//		}

//		public override string Description
//		{
//			get
//			{
//				return "Max power: 500hp\r\nEngine: 5.7l V12\r\nWeight: 1625kg";
//			}
//		}
//	}
//}