using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class SetRotation : ICode
	{
		IValue parameter;

		public SetRotation(IValue parameter)
		{
			this.parameter = parameter;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			var v = parameter?.GetValue(obj);
			if (v != null)
			{
				if (v is Quaternion q)
				{
					obj.transform.rotation = q;
				}
				else if (v is Vector3 f)
				{
					if (float.IsNaN(f.x) || float.IsNaN(f.y) || float.IsNaN(f.z))
						f = Vector3.zero;
					obj.transform.rotation = Quaternion.Euler(f);
				}
				else if (v is Vector3Int g)
				{
					if (float.IsNaN(g.x) || float.IsNaN(g.y) || float.IsNaN(g.z))
						g = Vector3Int.zero;
					obj.transform.rotation = Quaternion.Euler(g);
				}
			}

			return ReturnCode.None;
		}
	}
}