using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Interpreter
{
    public interface ICompilerEvent : ICompilerCodeConnection, IUseOffset, IBlockScale, ICodeType
    {
        //ICompilerEvent PreviousConnection { get; set; }
        ICompilerEvent NextConnection { get; set; }
        //ICompilerCode Code { get; set; }

        // Has CodeExecutor so that scripts that require transform/variables can cache them
        Program ConvertToCode();

        Transform NextConnectionObject { get; }
        //BlockOffset BlockOffset { get; set; }
    }
}