using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public class BooleanValue : IValue
    {
        public bool ReadOnly => false;

        bool value;

        public BooleanValue(bool value)
        {
            this.value = value;
        }

        public object GetValue(CodeExecutor obj)
        {
            return value;
        }
    }
}