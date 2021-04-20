using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class ListAdd : ICode
	{
		private IValue variable;
		private IValue value;

		public ListAdd(IValue variable, IValue value)
		{
			this.variable = variable;
			this.value = value;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			var possibleList = variable?.GetValue(obj);
			if (possibleList is Table tbl)
			{
				tbl.Add(value?.GetValue(obj));
			}

			return ReturnCode.None;
		}
	}
}