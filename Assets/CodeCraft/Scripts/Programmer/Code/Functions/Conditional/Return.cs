using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using ObjectEditor;

namespace Programmer
{
	public class Return : ICode
	{
		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			// Todo: UI print implementation
			return ReturnCode.Return;
		}
	}
}