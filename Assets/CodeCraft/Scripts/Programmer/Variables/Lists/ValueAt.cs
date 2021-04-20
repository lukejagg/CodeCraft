using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class ValueAt : IValue
	{
		public bool ReadOnly => true;

		IValue list;
		IValue value2;

		public ValueAt(IValue value2, IValue list)
		{
			this.list = list;
			this.value2 = value2;
		}

		public object GetValue(CodeExecutor obj)
		{
			var v1 = list?.GetValue(obj);
			var v2 = value2?.GetValue(obj);
			if (v1 is Table tbl)
			{
				return tbl[v2];
			}

			if (v1 is string str)
			{
				var ind = -1;
				if (v2 is int i)
					ind = i;
				else if (v2 is BigInteger b)
					ind = (int) b;
				else if (v2 is float f)
					ind = Mathf.RoundToInt(f);

				return ind < 0 || ind >= str.Length ? null : str[ind].ToString();
			}

			return null;
		}
	}
}