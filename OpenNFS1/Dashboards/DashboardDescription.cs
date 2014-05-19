using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OpenNFS1.Dashboards
{
	class DashboardDescription
	{
		public string Filename;
		public Vector2 TachPosition;
		public float TachRpmMultiplier;
		public float TachIdlePosition;
		public float TachNeedleLength;

		public static List<DashboardDescription> Descriptions = new List<DashboardDescription>();

		static DashboardDescription()
		{
			//ZR1
			Descriptions.Add(new DashboardDescription
			{
				Filename = "czr1dh.fsh",
				TachPosition = new Vector2(581, 130),
				TachRpmMultiplier = 0.54f,
				TachIdlePosition = 1.9f,
				TachNeedleLength = 2.5f
			});

			//Diablo
			Descriptions.Add(new DashboardDescription
			{
				Filename = "ldiabldh.fsh",
				TachPosition = new Vector2(444, 237),
				TachRpmMultiplier = 1.25f,
				TachIdlePosition = 2.42f,
				TachNeedleLength = 1.8f
			});
			//Viper
			Descriptions.Add(new DashboardDescription
			{
				Filename = "dviperdh.fsh",
				TachPosition = new Vector2(358, 182),
				TachRpmMultiplier = 1f,
				TachIdlePosition = 2.62f,
				TachNeedleLength = 1.8f
			});
			//F512
			Descriptions.Add(new DashboardDescription
			{
				Filename = "f512trdh.fsh",
				TachPosition = new Vector2(410, 185),
				TachRpmMultiplier = 1.3f,
				TachIdlePosition = 2.52f,
				TachNeedleLength = 1.8f
			});
			//NSX
			Descriptions.Add(new DashboardDescription
			{
				Filename = "ansxdh.fsh",
				TachPosition = new Vector2(564, 187),
				TachRpmMultiplier = 1.3f,
				TachIdlePosition = 2.6f,
				TachNeedleLength = 1.8f
			});
			//911
			Descriptions.Add(new DashboardDescription
			{
				Filename = "p911dh.fsh",
				TachPosition = new Vector2(512, 187),
				TachRpmMultiplier = 1.37f,
				TachIdlePosition = 2.61f,
				TachNeedleLength = 1.8f
			});
			//RX7
			Descriptions.Add(new DashboardDescription
			{
				Filename = "mrx7dh.fsh",
				TachPosition = new Vector2(518, 185),
				TachRpmMultiplier = 1.5f,
				TachIdlePosition = 2.62f,
				TachNeedleLength = 1.8f
			});
			//Supra
			Descriptions.Add(new DashboardDescription
			{
				Filename = "tsupradh.fsh",
				TachPosition = new Vector2(506, 206),
				TachRpmMultiplier = 1.28f,
				TachIdlePosition = 2.4f,
				TachNeedleLength = 1.8f
			});
			//Warrior
			Descriptions.Add(new DashboardDescription
			{
				Filename = "traffcdh.fsh",
				TachPosition = new Vector2(515, 166),
				TachRpmMultiplier = 0.63f,
				TachIdlePosition = 1.8f,
				TachNeedleLength = 2.3f
			});
		}
	}
}
