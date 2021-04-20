using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class Delete : ICode
	{
		private IValue parameter;

		public Delete(IValue parameter)
		{
			this.parameter = parameter;
		}

		public async Task DeleteObject(CodeExecutor obj)
		{
			if (obj != null)
			{
				SceneExecutor.Instance.Delete(obj);
			}
		}

		public async Task<ReturnCode> Run(CodeExecutor obj2)
		{
			if (parameter != null)
			{
				var v = parameter?.GetValue(obj2);
				if (v is CodeExecutor obj)
				{
					await DeleteObject(obj);
				}
			}
			else
			{
				await DeleteObject(obj2);
			}

			return ReturnCode.None;
		}
	}
}