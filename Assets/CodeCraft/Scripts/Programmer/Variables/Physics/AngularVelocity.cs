using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class AngularVelocity : IValue
	{
		public bool ReadOnly => false;

		private IValue value;

		public AngularVelocity(IValue value)
		{
			this.value = value;
		}

		public object GetValue(CodeExecutor obj2)
		{
			if (value != null)
			{
				var obj = value?.GetValue(obj2);
				if (obj != null && obj is CodeExecutor ce)
				{
					if (ce.rigidbody != null)
						return ce.rigidbody.angularVelocity;
				}

				return Vector3.zero;
			}
			if (obj2.rigidbody != null)
				return obj2.rigidbody.angularVelocity;

			return Vector3.zero;
		}
	}
}