using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class LookAt : ICode
	{
		private IValue parameter;

		public LookAt(IValue parameter)
		{
			this.parameter = parameter;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj2)
		{
			if (parameter != null)
			{
				var v = parameter?.GetValue(obj2);
				if (v is CodeExecutor obj)
				{
					obj2.transform.LookAt(obj.transform);
				}

				if (v is Vector3 pos)
				{
					obj2.transform.rotation = Quaternion.FromToRotation(Vector3.forward, pos - obj2.transform.position);
				}

				if (v is Vector3Int pos2)
				{
					obj2.transform.rotation = Quaternion.FromToRotation(Vector3.forward, pos2 - obj2.transform.position);
				}

				return ReturnCode.None;
			}

			obj2.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -obj2.transform.position);

			return ReturnCode.None;
		}
	}
}