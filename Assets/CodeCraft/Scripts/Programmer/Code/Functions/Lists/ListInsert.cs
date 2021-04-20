using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class ListInsert : ICode
	{
		private IValue variable;
		private IValue index;
		private IValue value;

		public ListInsert(IValue variable, IValue index, IValue value)
		{
			this.variable = variable;
			this.index = index;
			this.value = value;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			var possibleList = variable?.GetValue(obj);
			if (possibleList is Table tbl)
			{
				tbl.Insert(index?.GetValue(obj), value?.GetValue(obj));
			}

			return ReturnCode.None;
		}
	}
}