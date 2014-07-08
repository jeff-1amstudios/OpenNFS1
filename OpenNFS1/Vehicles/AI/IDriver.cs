using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenNFS1.Physics;

namespace OpenNFS1.Vehicles.AI
{
	interface IDriver
	{
		Vehicle Vehicle { get; }
		void Update(List<IDriver> otherDrivers);
	}
}
