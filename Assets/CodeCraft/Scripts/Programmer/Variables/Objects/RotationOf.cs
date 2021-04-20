using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class RotationOf : IValue
	{
		public bool ReadOnly => false;

		private IValue value;

		public RotationOf(IValue value)
		{
			this.value = value;
		}

		public object GetValue(CodeExecutor obj2)
		{
			if (value != null)
			{
				var obj = value?.GetValue(obj2);
				if (obj != null)
				{
					if (obj is CodeExecutor c)
						return c.transform.rotation;
					if (obj is Vector3 v)
						return Quaternion.Euler(v);
					if (obj is Vector3Int vi)
						return Quaternion.Euler(vi);
				}

				return Quaternion.identity;
			}
			return obj2.transform.rotation;
		}
	}
}