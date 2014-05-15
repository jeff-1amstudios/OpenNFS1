//using System;
//using System.Collections.Generic;
//using System.Text;
//using NeedForSpeed.Physics;
//using NeedForSpeed.Parsers;
//using Microsoft.Xna.Framework;
//using NfsEngine;
//using Microsoft.Xna.Framework.Audio;
//using NeedForSpeed.Dashboards;
//using Microsoft.Xna.Framework.Graphics;

//namespace NeedForSpeed.Vehicles
//{
//	class MazdaRx7: Vehicle
//	{
//		private CfmFile _model;
        
//		public MazdaRx7()
//			: base(1280, "MazdaRx7")
//		{

//			_model = CarModelCache.GetModel(@"SIMDATA\CARFAMS\mrx7.CFM");
            
//			_wheels[0] = new VehicleWheel(this, new Vector3(-9.45f, 0f, 14.2f), _model.TyreTexture, 6.5f);
//			_wheels[1] = new VehicleWheel(this, new Vector3(9.45f, 0f, 14.2f), _model.TyreTexture, 6.5f);
//			_wheels[2] = new VehicleWheel(this, new Vector3(-9.3f, 0f, -13.9f), _model.TyreTexture, 6.5f);
//			_wheels[3] = new VehicleWheel(this, new Vector3(9.3f, 0f, -13.9f), _model.TyreTexture, 6.5f);

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
//			get { return new Rx7Dashboard(this); }
//		}

//		public override void InitializeForDriving()
//		{
//			base.InitializeForDriving();

//			List<float> power = new List<float>(new float[] { 0.2f, 0.3f, 0.4f, 0.4f, 0.4f, 0.7f, 1.0f, 1.0f, 1.0f, 0 });
//			List<float> ratios = new List<float>(new float[] { 3.99f, 2.660f, 1.985f, 1.612f, 1.200f, 0.993f });

//			BaseGearbox gearbox = BaseGearbox.Create(GameConfig.ManualGearbox, ratios, 0.4f);
//			_motor = new Motor(power, 4f, 8f, gearbox);
//			_motor.Gearbox.GearChangeStarted += new EventHandler(Gearbox_GearChanged);
//			_traction = (_motor.GetPowerAtRpmForGear(_motor.RedlineRpm, 2) * 30) - 40;
//		}

//		public override string Name
//		{
//			get
//			{
//				return "1993 Mazda RX-7 Turbo";
//			}
//		}

//		public override string Description
//		{
//			get
//			{
//				return "Max power: 260hp\r\nEngine: 1.3l Rotary\r\nWeight: 1300kg";
//			}
//		}
//	}
//}