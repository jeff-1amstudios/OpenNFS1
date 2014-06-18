using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNFS1.Vehicles
{
	class VehicleDescription
	{
		public string Name;
		public string UIImageFile;
		public string ModelFile;
		public int Horsepower;
		public float Redline;
		public int Mass;

		public static List<VehicleDescription> Descriptions;

		static VehicleDescription()
		{
			Descriptions = new List<VehicleDescription>();

			Descriptions.Add(new VehicleDescription
			{
				Name = "RX7",
				UIImageFile = "rx71.qfs",
				ModelFile = @"SIMDATA\CARFAMS\mrx7.cfm",
				Horsepower = 255, //4,
				Mass = 1280,
				Redline = 8f
			});

			Descriptions.Add(new VehicleDescription
			{
				Name = "NSX",
				UIImageFile = "nsx1.qfs",
				ModelFile = @"SIMDATA\CARFAMS\ansx.cfm",
				Horsepower = 270,
				Mass = 1380,
				Redline = 7.5f
			});

			Descriptions.Add(new VehicleDescription
			{
				Name = "Supra",
				UIImageFile = "sup1.qfs",
				ModelFile = @"SIMDATA\CARFAMS\tsupra.cfm",
				Horsepower = 320,  // was 6 (x46)
				Mass = 1580,
				Redline = 7f
			});

			Descriptions.Add(new VehicleDescription
			{
				Name = "911",
				UIImageFile = "9111.qfs",
				ModelFile = @"SIMDATA\CARFAMS\p911.cfm",
				Horsepower = 270,
				Mass = 1380,
				Redline = 6.6f
			});

			Descriptions.Add(new VehicleDescription
			{
				Name = "ZR1",
				UIImageFile = "vet1.qfs",
				ModelFile = @"SIMDATA\CARFAMS\czr1.cfm",
				Horsepower = 405,
				Mass = 1380,
				Redline = 6.5f
			});

			Descriptions.Add(new VehicleDescription
			{
				Name = "Viper",
				UIImageFile = "vip1.qfs",
				ModelFile = @"SIMDATA\CARFAMS\dviper.cfm",
				Horsepower = 400,
				Mass = 1380,
				Redline = 6f
			});

			Descriptions.Add(new VehicleDescription
			{
				Name = "F512",
				UIImageFile = "5121.qfs",
				ModelFile = @"SIMDATA\CARFAMS\f512tr.cfm",
				Horsepower = 421,
				Mass = 1380,
				Redline = 8f
			});

			Descriptions.Add(new VehicleDescription
			{
				Name = "Diablo",
				UIImageFile = "dia1.qfs",
				ModelFile = @"SIMDATA\CARFAMS\ldiabl.cfm",
				Horsepower = 490,
				Mass = 1380,
				Redline = 7.5f
			});

			Descriptions.Add(new VehicleDescription
			{
				Name = "Warrior",
				UIImageFile = "war1.qfs",
				ModelFile = @"SIMDATA\CARFAMS\traffc.cfm",
				Horsepower = 700,
				Mass = 1380,
				Redline = 7f
			});
		}
	}
}
