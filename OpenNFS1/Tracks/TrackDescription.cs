using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace OpenNFS1
{
    class TrackDescription
    {
        static List<TrackDescription> _trackDescriptions;

        public static List<TrackDescription> Descriptions
        {
            get { return TrackDescription._trackDescriptions; }
        }

        public string Name;
        public string FileName;
        public bool IsOpenRoad;
        public string ImageFile;
        public bool HideFromMenu;
		public string AlternativeTimeOfDay;

		static TrackDescription()
		{
			_trackDescriptions = new List<TrackDescription>();

			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\al1.tri",
				Name = "Alpine",
				IsOpenRoad = true,
				ImageFile = "alpine.qfs",
				AlternativeTimeOfDay = "Morning"
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\cl2.tri",
				Name = "Coastal",
				IsOpenRoad = true,
				ImageFile = "coast.qfs",
				AlternativeTimeOfDay = "Morning"
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\cy1.tri",
				Name = "City",
				IsOpenRoad = true,
				ImageFile = "city.qfs",
				AlternativeTimeOfDay = "Morning"
			});

			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\tr1.tri",
				Name = "Rusty Springs",
				IsOpenRoad = false,
				ImageFile = "springs.qfs",
				AlternativeTimeOfDay = "Morning"
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\tr2.tri",
				Name = "Autumn Valley",
				IsOpenRoad = false,
				ImageFile = "avalley.qfs",
				AlternativeTimeOfDay = "Morning"
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\tr3.tri",
				Name = "Vertigo Ridge",
				IsOpenRoad = false,
				ImageFile = "vridge.qfs",
				AlternativeTimeOfDay = "Morning"
			});

			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\tr5.tri",
				Name = "Oasis Springs (hidden track)",
				IsOpenRoad = false,
				ImageFile = "springsr.qfs",
				AlternativeTimeOfDay = "Morning"
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\tr6.tri",
				Name = "Burnt Sienna",
				IsOpenRoad = false,
				ImageFile = "bsienna.qfs",
				AlternativeTimeOfDay = "Morning"
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\tr7.tri",
				Name = "Transpolis",
				IsOpenRoad = false,
				ImageFile = "tpolis.qfs",
				AlternativeTimeOfDay = "Morning"
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\tr4.tri",
				Name = "Lost Vegas",
				IsOpenRoad = false,
				ImageFile = "lvegas.qfs",
			});


			//Open road stages
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\al2.tri",
				Name = "Alpine 2",
				IsOpenRoad = true,
				ImageFile = "alpine.qfs",
				HideFromMenu = true
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\al3.tri",
				Name = "Alpine 3",
				IsOpenRoad = true,
				ImageFile = "alpine.qfs",
				HideFromMenu = true
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\cy2.tri",
				Name = "City 2",
				IsOpenRoad = true,
				ImageFile = "city.qfs",
				HideFromMenu = true
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\cy3.tri",
				Name = "City 3",
				IsOpenRoad = true,
				ImageFile = "city.qfs",
				HideFromMenu = true
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\cl2.tri",
				Name = "Coastal 2",
				IsOpenRoad = true,
				ImageFile = "coast.qfs",
				HideFromMenu = true
			});
			_trackDescriptions.Add(new TrackDescription()
			{
				FileName = "SIMDATA\\MISC\\cl3.tri",
				Name = "Coastal 3",
				IsOpenRoad = true,
				ImageFile = "coast.qfs",
				HideFromMenu = true
			});
		}

        /// <summary>
        /// Quick and very dirty way of getting the next stage in an open road race
        /// </summary>
        /// <param name="openRoadTrack"></param>
        /// <returns></returns>
        public static TrackDescription GetNextOpenRoadStage(TrackDescription openRoadTrack)
        {
            string lookForNumber = Path.GetFileNameWithoutExtension(openRoadTrack.FileName).Substring(2);
            lookForNumber = (int.Parse(lookForNumber)+1).ToString();

            return _trackDescriptions.Find(delegate(TrackDescription track)
            {
                if (Path.GetFileName(track.FileName).StartsWith(Path.GetFileName(openRoadTrack.FileName).Substring(0,
2)))
                {
                    if (Path.GetFileNameWithoutExtension(track.FileName).EndsWith(lookForNumber))
                        return true;
                }
                return false;
            });
        }
    }
}
