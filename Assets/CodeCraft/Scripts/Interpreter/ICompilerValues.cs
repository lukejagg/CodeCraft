using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interpreter
{
    public struct CodeValue
    {
        public int valueIndex;
        public Transform connectObject;
        public ICompilerValue value;

        public CodeValue(int valueIndex, Transform connectObject, ICompilerValue value)
        {
            this.valueIndex = valueIndex;
            this.connectObject = connectObject;
            this.value = value;
        }
    }

    public interface ICompilerValues
    {
        Transform Transform { get; }

        ICompilerValue[] Values { get; }
        CodeValue? GetValue(Transform t);
    }
}