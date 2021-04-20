using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public class IfElse : ICode
    {
        IValue condition;
        Program scope1;
        Program scope2;

        public IfElse(IValue condition, Program scope1, Program scope2)
        {
            this.condition = condition;
            this.scope1 = scope1;
            this.scope2 = scope2;
        }

        public async Task<ReturnCode> Run(CodeExecutor obj)
        {
            var var = condition?.GetValue(obj);
            if (var is bool b && b || (!(var is bool) && var != null))
            {
                var r = await scope1?.Execute(obj);

                if (r != ReturnCode.None)
	                return r;
            }
            else
            {
                var r = await scope2?.Execute(obj);

                if (r != ReturnCode.None)
	                return r;
            }

            return ReturnCode.None;
        }
    }
}