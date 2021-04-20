using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class ScaleOf : IValue
	{
		public bool ReadOnly => false;

		private IValue value;

		public ScaleOf(IValue value)
		{
			this.value = value;
		}

		public object GetValue(CodeExecutor obj2)
		{
			if (value != null)
			{
				var obj = value?.GetValue(obj2);
				if (obj != null && obj is CodeExecutor)
				{
					return (obj as CodeExecutor).transform.localScale;
				}

				return Quaternion.identity;
			}
			return obj2.transform.localScale;
		}
	}
}