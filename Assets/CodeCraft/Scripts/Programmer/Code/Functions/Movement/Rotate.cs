using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class Rotate : ICode
	{
		IValue parameter;

		public Rotate(IValue parameter)
		{
			this.parameter = parameter;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			if (parameter != null)
			{
				var v = parameter.GetValue(obj);
				if (v != null)
				{
					if (v is Quaternion q)
					{
						obj.transform.rotation *= q;
					}
					else if (v is Vector3 f)
					{
						if (float.IsNaN(f.x) || float.IsNaN(f.y) || float.IsNaN(f.z))
							f = Vector3.zero;
						obj.transform.Rotate(f);
					}
					else if (v is Vector3Int g)
					{
						if (float.IsNaN(g.x) || float.IsNaN(g.y) || float.IsNaN(g.z))
							g = Vector3Int.zero;
						obj.transform.Rotate(g);
					}
				}
			}

			return ReturnCode.None;
		}
	}
}