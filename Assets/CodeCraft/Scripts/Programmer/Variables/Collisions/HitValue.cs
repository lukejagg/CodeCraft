using System.Collections;
using System.Collections.Generic;
using Interpreter;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class HitValue : IValue
	{
		public bool ReadOnly => true;

		public HitValue()
		{

		}

		public object GetValue(CodeExecutor obj)
		{
			return obj.lastHit;
		}
	}
}