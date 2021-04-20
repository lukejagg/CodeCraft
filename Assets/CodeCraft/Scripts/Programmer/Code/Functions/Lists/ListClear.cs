using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class ListClear : ICode
	{
		private IValue variable;

		public ListClear(IValue variable)
		{
			this.variable = variable;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			var possibleList = variable?.GetValue(obj);
			if (possibleList is Table tbl)
			{
				tbl.Clear();
			}

			return ReturnCode.None;
		}
	}
}