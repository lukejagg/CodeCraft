using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
	public class Magnitude : IValue
	{
		public bool ReadOnly => true;
		private IValue value = null;

		public Magnitude(IValue value)
		{
			this.value = value;
		}

		public object GetValue(CodeExecutor obj)
		{
			var v1 = value?.GetValue(obj);
			if (v1 != null)
			{
				if (v1 is Vector3Int vi)
				{
					return vi.magnitude;
				}

				if (v1 is Vector3 v)
				{
					return v.magnitude;
				}
			}

			return 0;
		}
	}
}