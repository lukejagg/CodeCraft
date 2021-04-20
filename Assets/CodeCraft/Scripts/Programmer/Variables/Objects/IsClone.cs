using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class IsClone : IValue
	{
		public bool ReadOnly => false;

		public IValue value;

		public IsClone(IValue value)
		{
			this.value = value;
		}

		public object GetValue(CodeExecutor obj)
		{
			if (value != null)
			{
				var v1 = value?.GetValue(obj);
				if (v1 != null && v1 is CodeExecutor obj2)
				{
					return obj2.isClone;
				}

				return false;
			}
			return obj.isClone;
		}

		public void SetValue(IValue newValue)
		{

		}
	}
}