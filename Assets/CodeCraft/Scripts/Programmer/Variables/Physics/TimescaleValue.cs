using System.Collections;
using System.Collections.Generic;
using Interpreter;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class TimescaleValue : IValue
	{
		public bool ReadOnly => true;

		public object GetValue(CodeExecutor obj)
		{
			return Time.timeScale;
		}
	}
}