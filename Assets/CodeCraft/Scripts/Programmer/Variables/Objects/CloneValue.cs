using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	// Todo: Implement input values (Primitives)
	public class CloneValue : IValue
	{
		public bool ReadOnly => true;

		public CloneValue()
		{

		}

		public object GetValue(CodeExecutor obj2)
		{
			return obj2.lastClone;
		}
	}
}