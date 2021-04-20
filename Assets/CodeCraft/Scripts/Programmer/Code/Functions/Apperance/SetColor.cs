using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using System.Threading.Tasks;
using ObjectEditor;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
    public class SetColor : ICode
    {
        IValue parameter;

        public SetColor(IValue parameter)
        {
            this.parameter = parameter;
        }

        public async Task<ReturnCode> Run(CodeExecutor obj)
        {
		    var val = parameter?.GetValue(obj);

		    if (val != null)
		    {
			    if (val is Vector3 v)
			    {
				    obj.SetColor(new Vector3(Mathf.Clamp01(v.x), Mathf.Clamp01(v.y), Mathf.Clamp01(v.z)));
			    }
			    else if (val is Vector3Int vi)
			    {
				    obj.SetColor(new Vector3(
					    Mathf.Clamp01(vi.x / 255f),
					    Mathf.Clamp01(vi.y / 255f),
					    Mathf.Clamp01(vi.z / 255f)
				    ));
			    }
			    else if (val is float f)
			    {
				    obj.SetColor(new Vector3(
					    Mathf.Clamp01(f),
					    Mathf.Clamp01(f),
					    Mathf.Clamp01(f)
				    ));
			    }
			    else if (val is BigInteger i)
			    {
				    obj.SetColor(new Vector3(
					    Mathf.Clamp01((float) i / 255f),
					    Mathf.Clamp01((float) i / 255f),
					    Mathf.Clamp01((float) i / 255f)
				    ));
			    }
			    else if (val is int ii)
			    {
				    obj.SetColor(new Vector3(
					    Mathf.Clamp01(ii / 255f),
					    Mathf.Clamp01(ii / 255f),
					    Mathf.Clamp01(ii / 255f)
				    ));
			    }
		    }

		    return ReturnCode.None;
        }
    }
}