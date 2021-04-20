using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class ListSet : ICode
	{
		private IValue variable;
		private IValue index;
		private IValue value;

		public ListSet(IValue variable, IValue index, IValue value)
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
				var v1 = index?.GetValue(obj);
				if (v1 != null)
				{
					tbl[v1] = value?.GetValue(obj);
				}
			}
			/*
			else if (variable is VariableValue varVal && possibleList is string str)
			{
				var ind = index?.GetValue(obj);
				var val = value?.GetValue(obj);

				if (ind != null)
				{
					if (ind is BigInteger b)
					{

					}
					else if (ind is string str2)
					{
						string valStr = val != null ? val.ToString() : "";
					}
				}
			}
			*/

			return ReturnCode.None;
		}
	}
}