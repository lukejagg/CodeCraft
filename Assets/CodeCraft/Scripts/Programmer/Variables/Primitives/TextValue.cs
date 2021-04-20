using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public class TextValue : IValue
    {
        public bool ReadOnly => false;

        string value;

        public TextValue(string value)
        {
            this.value = value;
        }

        public object GetValue(CodeExecutor obj)
        {
            return value;
        }
    }
}