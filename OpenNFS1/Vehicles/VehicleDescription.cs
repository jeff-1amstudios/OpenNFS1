using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenNFS1.Vehicles
{
	class VehicleDescription
	{
		public string Name;
		public string Description;
		public string Filename;
		public int Horsepower;
		public float Redline;
		public int Mass;

		public static List<VehicleDescription> Descriptions;

		static VehicleDescription()
		{
			Descriptions = new List<VehicleDescription>();
			Descriptions.Add(new VehicleDescription { Name = "ZR1", Filename = @"SIMDATA\CARFAMS\czr1.cfm", Horsepower = 6, Mass = 1380, Redline = 6.5f });
			Descriptions.Add(new VehicleDescription { Name = "Viper", Filename = @"SIMDATA\CARFAMS\dviper.cfm", Horsepower = 6, Mass = 1380, Redline = 6f });
			Descriptions.Add(new VehicleDescription { Name = "F512", Filename = @"SIMDATA\CARFAMS\f512tr.cfm", Horsepower = 8, Mass = 1380, Redline = 8f });
			Descriptions.Add(new VehicleDescription { Name = "NSX", Filename = @"SIMDATA\CARFAMS\ansx.cfm", Horsepower = 6, Mass = 1380, Redline = 7.5f });
			Descriptions.Add(new VehicleDescription { Name = "Diablo", Filename = @"SIMDATA\CARFAMS\ldiabl.cfm", Horsepower = 9, Mass = 1380, Redline = 7.5f });
			Descriptions.Add(new VehicleDescription { Name = "RX7", Filename = @"SIMDATA\CARFAMS\mrx7.cfm", Horsepower = 4, Mass = 1280, Redline = 8f });
			Descriptions.Add(new VehicleDescription { Name = "911", Filename = @"SIMDATA\CARFAMS\p911.cfm", Horsepower = 6, Mass = 1380, Redline = 6.6f });
			Descriptions.Add(new VehicleDescription { Name = "ToyotaSupra", Filename = @"SIMDATA\CARFAMS\tsupra.cfm", Horsepower = 6, Mass = 1580, Redline = 7f });
			Descriptions.Add(new VehicleDescription { Name = "Warrior", Filename = @"SIMDATA\CARFAMS\traffc.cfm", Horsepower = 14, Mass = 1380, Redline = 7f });
		}
	}
}
