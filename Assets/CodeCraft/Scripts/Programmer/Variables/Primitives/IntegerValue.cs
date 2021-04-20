using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public class IntegerValue : IValue
    {
        public bool ReadOnly => false;

        BigInteger value;

        public IntegerValue(BigInteger value)
        {
            this.value = value;
        }

        public object GetValue(CodeExecutor obj)
        {
            return value;
        }
    }
}