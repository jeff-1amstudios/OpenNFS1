using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameEngine
{
	public class AverageValueVector3
	{
		int _nbrValues;
		List<Vector3> _values = new List<Vector3>();

		public AverageValueVector3(int nbrVaues)
		{
			_nbrValues = nbrVaues;
		}

        public void Reset(int nbrValues)
        {
            _nbrValues = nbrValues;
            _values.Clear();
        }

		public void AddValue(Vector3 value)
		{
			_values.Add(value);
			if (_values.Count > _nbrValues)
				_values.RemoveAt(0);
		}

		public Vector3 GetAveragedValue()
		{
			Vector3 average = new Vector3();
			foreach (Vector3 value in _values)
			{
				average += value;
			}
			return average / _values.Count;
		}
	}
}
