using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class TableValue : IValue
	{
		public bool ReadOnly => false;
		
		public object GetValue(CodeExecutor obj)
		{
			return new Table();
		}
	}
}