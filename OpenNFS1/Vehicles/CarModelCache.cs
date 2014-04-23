using System;
using System.Collections.Generic;
using System.Text;
using NeedForSpeed.Parsers;

namespace NeedForSpeed.Vehicles
{
    static class CarModelCache
    {
        static Dictionary<string, CarModel> _cache = new Dictionary<string,CarModel>();

        public static CarModel GetModel(string filename)
        {
            if (!_cache.ContainsKey(filename))
            {
                CarModel model = new CarModel(filename);
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
