using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public class Or : IValue
    {
        public bool ReadOnly => true;

        IValue value1 = null;
        IValue value2 = null;

        public Or(IValue value1, IValue value2)
        {
            this.value1 = value1;
            this.value2 = value2;
        }

        public object GetValue(CodeExecutor obj)
        {
	        var v1 = value1?.GetValue(obj);
	        if (v1 != null)
	        {
		        if (v1 is bool b)
		        {
			        if (b) return b;
		        }
                else return v1;
	        }

	        return value2?.GetValue(obj);
        }

        public void SetValue(IValue newValue)
        {

        }
    }
}