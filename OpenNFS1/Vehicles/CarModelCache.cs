using System;
using System.Collections.Generic;
using System.Text;
using OpenNFS1.Parsers;

namespace OpenNFS1.Vehicles
{
    static class CarModelCache
    {
		static Dictionary<string, CarMesh> _cache = new Dictionary<string, CarMesh>();

		public static CarMesh GetCfm(string filename)
        {
            if (!_cache.ContainsKey(filename))
            {
				CfmFile cfm = new CfmFile(filename);
                _cache.Add(filename, cfm.Mesh);
                return cfm.Mesh;
            }
            else
            {
                return _cache[filename];
            }
        }
    }
}
