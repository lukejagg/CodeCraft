using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public class If : ICode
    {
        IValue condition;
        Program scope1;

        public If(IValue condition, Program scope1)
        {
            this.condition = condition;
            this.scope1 = scope1;
        }

        public async Task<ReturnCode> Run(CodeExecutor obj)
        {
	        if (condition == null)
		        return ReturnCode.None;

            var val = condition?.GetValue(obj);
            if (val != null && (val is bool b && b || (!(val is bool) && val != null)))
            {
                var r = await scope1?.Execute(obj);

                if (r != ReturnCode.None)
	                return r;
            }

            return ReturnCode.None;
        }
    }
}