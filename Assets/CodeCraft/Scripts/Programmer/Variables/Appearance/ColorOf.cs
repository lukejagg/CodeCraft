using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class ColorOf : IValue
	{
		public bool ReadOnly => false;

		private IValue value;

		public ColorOf(IValue value)
		{
			this.value = value;
		}

		public object GetValue(CodeExecutor obj2)
		{
			if (value != null)
			{
				var obj = value?.GetValue(obj2);
				if (obj != null && obj is CodeExecutor c)
				{
					return c.color;
				}

				return Vector3.one;
			}
			return obj2.color;
		}

		public void SetValue(IValue newValue)
		{

		}
	}
}