using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace OpenNFS1
{
    class AverageValue
    {
        int _nbrValues;
        List<float> _values = new List<float>();

        public AverageValue(int nbrVaues)
        {
            _nbrValues = nbrVaues;
        }

        public void AddValue(float value)
        {
            _values.Add(value);
            if (_values.Count > _nbrValues)
                _values.RemoveAt(0);
        }

        public float GetAveragedValue()
        {
            float average = 0;
            foreach (float value in _values)
            {
                average += value;
            }
            return average / _values.Count;
        }

        public void Clear()
        {
            _values.Clear();
        }
    }

    
}
