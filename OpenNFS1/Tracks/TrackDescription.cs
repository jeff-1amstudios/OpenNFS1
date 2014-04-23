using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace NeedForSpeed
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

        static TrackDescription()
        {
            _trackDescriptions = new List<TrackDescription>();

            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\al1.tri", Name = "Alpine", IsOpenRoad = true, ImageFile = "Content/TrackImages/alpine" });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\cl1.tri", Name = "Coastal", IsOpenRoad = true, ImageFile = "Content/TrackImages/coast" });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\cy1.tri", Name = "City", IsOpenRoad = true, ImageFile = "Content/TrackImages/city" });
            
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\tr1.tri", Name = "Rusty Springs", IsOpenRoad = false, ImageFile="Content/TrackImages/RustySprings" });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\tr2.tri", Name = "Autumn Valley", IsOpenRoad = false, ImageFile = "Content/TrackImages/AutumnValley" });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\tr3.tri", Name = "Vertigo Ridge", IsOpenRoad = false, ImageFile = "Content/TrackImages/VertigoRidge" });
            
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\tr5.tri", Name = "Oasis Springs (hidden track)", IsOpenRoad = false, ImageFile = "Content/TrackImages/Transpolis" });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\tr6.tri", Name = "Burnt Sienna", IsOpenRoad = false, ImageFile = "Content/TrackImages/BurntSienna" });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\tr7.tri", Name = "Transpolis", IsOpenRoad = false, ImageFile = "Content/TrackImages/Transpolis" });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\tr4.tri", Name = "Lost Vegas", IsOpenRoad = false, ImageFile = "Content/TrackImages/LostVegas" });
            
            
            //Open road stages
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\al2.tri", Name = "Alpine 2", IsOpenRoad = true, ImageFile = "Content/TrackImages/alpine", HideFromMenu = true  });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\al3.tri", Name = "Alpine 3", IsOpenRoad = true, ImageFile = "Content/TrackImages/alpine", HideFromMenu = true });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\cy2.tri", Name = "City 2", IsOpenRoad = true, ImageFile = "Content/TrackImages/city", HideFromMenu = true });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\cy3.tri", Name = "City 3", IsOpenRoad = true, ImageFile = "Content/TrackImages/city", HideFromMenu = true });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\cl2.tri", Name = "Coastal 2", IsOpenRoad = true, ImageFile = "Content/TrackImages/coast", HideFromMenu = true });
            _trackDescriptions.Add(new TrackDescription() { FileName = "Data\\Tracks\\cl3.tri", Name = "Coastal 3", IsOpenRoad = true, ImageFile = "Content/TrackImages/coast", HideFromMenu = true });
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
                if (Path.GetFileName(track.FileName).StartsWith(Path.GetFileName(openRoadTrack.FileName).Substring(0,2)))
                {
                    if (Path.GetFileNameWithoutExtension(track.FileName).EndsWith(lookForNumber))
                        return true;
                }
                return false;
            });
        }
    }
}
