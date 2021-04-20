using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class ListRemove : ICode
	{
		private IValue variable;
		private IValue index;

		public ListRemove(IValue variable, IValue index)
		{
			this.variable = variable;
			this.index = index;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			var possibleList = variable?.GetValue(obj);
			if (possibleList is Table tbl)
			{
				tbl.RemoveAt(index?.GetValue(obj));
			}

			return ReturnCode.None;
		}
	}
}