using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using ObjectEditor;

namespace Programmer
{
    public class Print : ICode
    {
        IValue parameter;

        public Print(IValue parameter)
        {
            this.parameter = parameter;
        }

        public async Task<ReturnCode> Run(CodeExecutor obj)
        {
	        if (parameter == null)
	        {
		        PlayOutput.Instance.Output("null");
	        }
	        else
	        {
		        var val = parameter.GetValue(obj);

		        if (val == null)
		        {
			        PlayOutput.Instance.Output("null");
				}
		        else
		        {
			        PlayOutput.Instance.Output(val.ToString());
		        }
	        }

	        return ReturnCode.None;
        }
    }
}