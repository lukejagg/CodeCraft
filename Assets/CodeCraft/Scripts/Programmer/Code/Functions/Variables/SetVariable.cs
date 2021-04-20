using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using ObjectEditor;

namespace Programmer
{
    public class SetVariable : ICode
    {
	    private VariableValue value;
        IValue parameter = null;

        public SetVariable(IValue var, IValue parameter)
        {
	        if (var is VariableValue val)
	        {
		        this.value = val;
		        this.parameter = parameter;
	        }
        }

        public async Task<ReturnCode> Run(CodeExecutor obj)
        {
	        if (value != null)
	        {
		        //value.executor.Variables[value.variableIndex].SetValue(parameter);
		        value.SetValue(parameter, obj);
	        }

	        return ReturnCode.None;
        }
    }
}