using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interpreter
{
    public enum CodeType
    {
        Event,
        Function,
        Value
    }

    public class HierarchyObject : MonoBehaviour
    {
        public Transform codePrefab;
        public CodeType codeType;

        public string Description;
    }
}