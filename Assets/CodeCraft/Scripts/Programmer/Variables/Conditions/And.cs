using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public class And : IValue
    {
        public bool ReadOnly => true;

        IValue value1 = null;
        IValue value2 = null;

        public And(IValue value1, IValue value2)
        {
            this.value1 = value1;
            this.value2 = value2;
        }

        public object GetValue(CodeExecutor obj)
        {
	        var v1 = value1?.GetValue(obj);
	        if (v1 != null)
	        {
                // If bool and true
		        if (v1 is bool b)
		        {
			        if (b) return value2?.GetValue(obj);
		        }
		        else // not bool
		        {
			        return value2?.GetValue(obj);
                }
	        }

	        return false;
        }

        public void SetValue(IValue newValue)
        {

        }
    }
}