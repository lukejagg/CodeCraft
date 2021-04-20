using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class PositionOf : IValue
	{
		public bool ReadOnly => false;

		private IValue value;

		public PositionOf(IValue value)
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
					return (obj as CodeExecutor).transform.position;
				}

				return Vector3.zero;
			}
			return obj2.transform.position;
		}
	}
}