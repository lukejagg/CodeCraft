using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class VariableValue : IValue
	{
		public bool ReadOnly => false;

		public string variableIndex;

		public VariableValue(string name)
		{
			this.variableIndex = name;

		}

		public object GetValue(CodeExecutor obj)
		{
			if (obj.Variables.TryGetValue(variableIndex, out var value))
			{
				return value;
			}

			return null;
		}

		public void SetValue(IValue newValue, CodeExecutor obj)
		{
			if (obj.Variables.ContainsKey(variableIndex))
			{
				obj.Variables[variableIndex] = newValue?.GetValue(obj);
			}
			else
			{
				obj.Variables.Add(variableIndex, newValue?.GetValue(obj));
			}
		}

		public void SetValueToObject(object newValue, CodeExecutor obj)
		{
			if (obj.Variables.ContainsKey(variableIndex))
			{
				obj.Variables[variableIndex] = newValue;
			}
			else
			{
				obj.Variables.Add(variableIndex, newValue);
			}
		}
	}
}