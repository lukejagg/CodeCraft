using CodeEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interpreter
{
    public interface IUseOffset
    {
        IOffset Offset { get; set; }
    }
}