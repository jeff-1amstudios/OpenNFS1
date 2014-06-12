using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenNFS1;
using OpenNFS1.Dashboards;
using OpenNFS1.Physics;
using OpenNFS1.Vehicles;
using NfsEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OpenNFS1.Vehicles
{
	class DrivableVehicle : Vehicle
	{
		private CarMesh _model;
		public VehicleDescription Descriptor { get; private set; }

		public DrivableVehicle(VehicleDescription desc)
			: base(desc.Mass, desc.Name)
		{
			Descriptor = desc;
			var cfm = CarModelCache.GetCfm(desc.ModelFile, true);
			_model = (CarMesh)cfm.Mesh;		
			
			InitializePhysics();

		}

		public void InitializePhysics()
		{
			float offset = VehicleWheel.Width / 2 - 0.1f;
			_wheels[0] = new VehicleWheel(this, _model.LeftFrontWheelPos, _model.FrontWheelSize, _model.WheelTexture, offset);
			_wheels[1] = new VehicleWheel(this, _model.RightFrontWheelPos, _model.FrontWheelSize, _model.WheelTexture, -offset);
			_wheels[2] = new VehicleWheel(this, _model.LeftRearWheelPos, _model.RearWheelSize, _model.WheelTexture, offset);
			_wheels[3] = new VehicleWheel(this, _model.RightRearWheelPos, _model.RearWheelSize, _model.WheelTexture, -offset);

			List<float> power = new List<float>(new float[] { 0.2f, 0.3f, 0.4f, 0.7f, 0.8f, 1.0f, 0.8f, 0.8f, 0.8f, 0.3f });
			List<float> ratios = new List<float>(new float[] { 3.827f, 2.360f, 1.685f, 1.312f, 1.000f, 0.793f });

			BaseGearbox gearbox = BaseGearbox.Create(GameConfig.ManualGearbox, ratios, 0.2f);
			_motor = new Motor(power, Descriptor.Horsepower, Descriptor.Redline, gearbox);
			_motor.Gearbox.GearChangeStarted += new EventHandler(Gearbox_GearChanged);
			_traction = (_motor.GetPowerAtRpmForGear(_motor.RedlineRpm, 2) * 30) - 30;
		}		

		public override void Render()
		{
			_effect.View = Engine.Instance.Camera.View;
			_effect.Projection = Engine.Instance.Camera.Projection;
			_effect.World = _renderMatrix;
			
			Engine.Instance.Device.RasterizerState = RasterizerState.CullNone;
			Engine.Instance.Device.BlendState = BlendState.Opaque;
			_effect.CurrentTechnique.Passes[0].Apply();
			_model.Render(_effect, BrakePedalInput > 0);
			base.Render();			
		}
	}
}
