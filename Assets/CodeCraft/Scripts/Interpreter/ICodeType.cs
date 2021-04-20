using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interpreter
{
    public interface ICodeType
    {
        CodeType CodeType { get; }
        string SaveId { get; }

        string Save();
    }
}