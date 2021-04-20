using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public interface IValue
    {
        bool ReadOnly { get; }
        object GetValue(CodeExecutor obj);
        //void SetValue(IValue newValue);
    }
}