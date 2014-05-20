using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNFS1.Physics;

namespace OpenNFS1.Vehicles
{
	class AIVehicle : Vehicle
	{
		public AIVehicle(VehicleDescription desc)
			: base(desc.Mass, desc.Name)
		{
		}
	}
}
