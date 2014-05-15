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
//	class ToyotaSupra : Vehicle
//	{
//		private CfmFile _model;
        
//		public ToyotaSupra()
//			: base(1580, "ToyotaSupra")
//		{
            
//			_model = CarModelCache.GetModel(@"SIMDATA\CARFAMS\tsupra.CFM");
            
//			_wheels[0] = new VehicleWheel(this, new Vector3(-2.4f, 0f, 4.9f), _model.TyreTexture, 6.4f);
//			_wheels[1] = new VehicleWheel(this, new Vector3(2.4f, 0f, 4.9f), _model.TyreTexture, 6.4f);
//			_wheels[2] = new VehicleWheel(this, new Vector3(-2.4f, 0f, -4.6f), _model.TyreTexture, 6.4f);
//			_wheels[3] = new VehicleWheel(this, new Vector3(2.4f, 0f, -4.6f), _model.TyreTexture, 6.4f);

//			_bodyRideHeight = -1.0f;
//		}


//				public override void Render()
//				{
//					RenderShadow();
//					_effect.View = Engine.Instance.Camera.View;
//					_effect.Projection = Engine.Instance.Camera.Projection;
//					_effect.CurrentTechnique.Passes[0].Apply();
//					_effect.World = Matrix.CreateScale(0.04f) * _renderMatrix;
//					Engine.Instance.Device.RasterizerState = new RasterizerState { FillMode = FillMode.WireFrame };
//					_model.Render(_effect, VehicleController.Brake > 0);
//					Engine.Instance.Device.RasterizerState = RasterizerState.CullCounterClockwise;
//					base.Render();
//				}

//		internal override BaseDashboard Dashboard
//		{
//			get { return new SupraDashboard(this); }
//		}

//		public override void InitializeForDriving()
//		{
//			base.InitializeForDriving();
//			List<float> power = new List<float>(new float[] { 0.2f, 0.3f, 0.4f, 0.7f, 0.8f, 1.0f, 0.8f, 0.8f, 0 });
//			List<float> ratios = new List<float>(new float[] { 3.827f, 2.360f, 1.685f, 1.312f, 1.000f, 0.793f });

//			BaseGearbox gearbox = BaseGearbox.Create(GameConfig.ManualGearbox, ratios, 0.4f);
//			_motor = new Motor(power, 6, 7f, gearbox);
//			_motor.Gearbox.GearChangeStarted += new EventHandler(Gearbox_GearChanged);
//			_traction = (_motor.GetPowerAtRpmForGear(_motor.RedlineRpm, 2) * 30) - 30;
//		}

//		public override string Name
//		{
//			get
//			{
//				return "1993 Toyota Supra Turbo";
//			}
//		}

//		public override string Description
//		{
//			get
//			{
//				return "Max power: 320hp\r\nEngine: 3.0l Inline 6\r\nWeight: 1580kg";
//			}
//		}
//	}
//}