using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class CachedValue : IValue
	{
		public bool ReadOnly => false;

		public object value;

		public CachedValue(object value)
		{
			this.value = value;
		}

		public object GetValue(CodeExecutor obj)
		{
			return value;
		}

		public void SetValue(IValue newValue, CodeExecutor obj)
		{
			value = newValue.GetValue(obj);
		}
	}
}