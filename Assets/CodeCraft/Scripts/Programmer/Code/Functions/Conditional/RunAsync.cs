using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class RunAsync : ICode
	{
		Program scope1;

		public RunAsync(Program scope1)
		{
			this.scope1 = scope1;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			scope1?.Execute(obj);

			return ReturnCode.None;
		}
	}
}