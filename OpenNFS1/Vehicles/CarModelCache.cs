using System;
using System.Collections.Generic;
using System.Text;
using NeedForSpeed.Parsers;

namespace NeedForSpeed.Vehicles
{
    static class CarModelCache
    {
        static Dictionary<string, CfmFile> _cache = new Dictionary<string,CfmFile>();

        public static CfmFile GetCfm(string filename)
        {
            if (!_cache.ContainsKey(filename))
            {
                CfmFile model = new CfmFile(filename);
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
