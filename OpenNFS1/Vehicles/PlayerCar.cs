using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeedForSpeed;
using NeedForSpeed.Dashboards;
using NeedForSpeed.Physics;
using NeedForSpeed.Vehicles;
using NfsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNFS1.Vehicles
{
	class PlayerCar : Vehicle
	{
		private CarMesh _model;
		VehicleDescription _info;

		public PlayerCar(VehicleDescription desc)
			: base(desc.Mass, desc.Name)
		{
			_info = desc;
			var cfm = CarModelCache.GetCfm(desc.Filename);
			_model = cfm.Mesh;

			float offset = VehicleWheel.Width / 2;
			_wheels[0] = new VehicleWheel(this, _model.LeftFrontWheelPos, _model.FrontWheelSize, _model.WheelTexture, offset);
			_wheels[1] = new VehicleWheel(this, _model.RightFrontWheelPos, _model.FrontWheelSize, _model.WheelTexture, -offset);
			_wheels[2] = new VehicleWheel(this, _model.LeftRearWheelPos, _model.RearWheelSize, _model.WheelTexture, offset);
			_wheels[3] = new VehicleWheel(this, _model.RightRearWheelPos, _model.RearWheelSize, _model.WheelTexture, -offset);
		}

		internal override BaseDashboard Dashboard
		{
			get { return new SupraDashboard(this); }
		}

		public override void InitializeForDriving()
		{
			base.InitializeForDriving();
			List<float> power = new List<float>(new float[] { 0.2f, 0.3f, 0.4f, 0.7f, 0.8f, 1.0f, 0.8f, 0.8f, 0 });
			List<float> ratios = new List<float>(new float[] { 3.827f, 2.360f, 1.685f, 1.312f, 1.000f, 0.793f });

			BaseGearbox gearbox = BaseGearbox.Create(GameConfig.ManualGearbox, ratios, 0.4f);
			_motor = new Motor(power, _info.Horsepower, _info.Redline, gearbox);
			_motor.Gearbox.GearChangeStarted += new EventHandler(Gearbox_GearChanged);
			_traction = (_motor.GetPowerAtRpmForGear(_motor.RedlineRpm, 2) * 30) - 30;
		}

		public override void Render()
		{
			RenderShadow();
			_effect.View = Engine.Instance.Camera.View;
			_effect.Projection = Engine.Instance.Camera.Projection;
			_effect.CurrentTechnique.Passes[0].Apply();
			_effect.World = _renderMatrix;
			Engine.Instance.Device.RasterizerState = new RasterizerState { FillMode = FillMode.Solid, CullMode = CullMode.None };
			_model.Render(_effect);
			Engine.Instance.Device.RasterizerState = RasterizerState.CullCounterClockwise;
			base.Render();
		}

	}
}
