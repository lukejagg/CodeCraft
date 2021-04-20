using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
	public class Not : IValue
	{
		public bool ReadOnly => true;

		public IValue value1 = null;

		public Not(IValue value1)
		{
			this.value1 = value1;
		}

		public object GetValue(CodeExecutor obj)
		{
			var v1 = value1?.GetValue(obj);

			if (v1 == null)
				return true;

			if (v1 is bool b)
				return !b;
			//if (v1 is float || v1 is BigInteger || v1 is Vector3 || v1 is Vector3Int) return v1 * -1;

			if (v1 is Quaternion q)
				return Quaternion.Inverse(q);

			if (v1 is Vector3 v)
				return -v;
			if (v1 is Vector3Int vi)
				return -vi;

			return false;
		}
	}
}