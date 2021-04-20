using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
	public class SetScale : ICode
	{
		IValue parameter;

		public SetScale(IValue parameter)
		{
			this.parameter = parameter;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			var v = parameter?.GetValue(obj);
			if (v != null)
			{
				if (v is Vector3 f)
				{
					if (float.IsNaN(f.x) || float.IsNaN(f.y) || float.IsNaN(f.z))
						f = Vector3.zero;
					obj.transform.localScale = f;
				}
				else if (v is Vector3Int g)
				{
					if (float.IsNaN(g.x) || float.IsNaN(g.y) || float.IsNaN(g.z))
						g = Vector3Int.zero;
					obj.transform.localScale = g;
				}
				else if (v is float fl)
				{
					obj.transform.localScale = Vector3.one * fl;
				}
				else if (v is int i)
				{
					obj.transform.localScale = Vector3.one * i;
				}
				else if (v is BigInteger bi)
				{
					obj.transform.localScale = Vector3.one * (float)bi;
				}
			}

			return ReturnCode.None;
		}
	}
}