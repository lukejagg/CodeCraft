using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public class Wait : ICode
    {
        IValue parameter;

        public Wait(IValue parameter)
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
					if (v is float f)
						await Task.Delay(Mathf.FloorToInt(f * 1000));
					else if (v is int i)
						await Task.Delay(i * 1000);
					else if (v is BigInteger b)
						await Task.Delay((int) b * 1000);
					else 
						await Task.Delay(1);
				}
		        else
		        {
			        await Task.Delay(1);
		        }
	        }
			else
			{
				await Task.Delay(1);
			}
	        return ReturnCode.None;
        }
    }
}