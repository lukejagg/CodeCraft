using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public class NumberValue : IValue
    {
        public bool ReadOnly => false;

        float value;

        public NumberValue(float value)
        {
            this.value = value;
        }

        public object GetValue(CodeExecutor obj)
        {
            return value;
        }
    }
}