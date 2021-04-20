using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	// Todo: Implement input values (Primitives)
	public class ObjectValue : IValue
	{
		public bool ReadOnly => true;

		public ObjectValue()
		{

		}

		public object GetValue(CodeExecutor obj)
		{
			return obj;
		}
	}
}