using System;
using System.Collections.Generic;
using System.Text;
using OpenNFS1.Parsers;

namespace OpenNFS1.Vehicles
{
    static class CarModelCache
    {
        static Dictionary<string, CfmFile> _cache = new Dictionary<string,CfmFile>();

		public static CfmFile GetCfm(string filename, bool drivable)
        {
            if (!_cache.ContainsKey(filename))
            {
				CfmFile model = new CfmFile(filename, drivable);
                _cache.Add(filename, model);
                return model;
            }
            else
            {
                return _cache[filename];
            }
        }
    }
}
