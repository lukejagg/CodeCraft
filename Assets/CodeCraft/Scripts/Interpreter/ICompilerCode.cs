using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Interpreter
{
    public interface ICompilerCode : ICompilerCodeConnection, ICompilerValues, IUseOffset, IBlockScale, ICodeType
    {
        //ICompilerCode PreviousConnection { get; set; }
        //ICompilerCode NextConnection { get; set; }
        //ICompilerValue[] Values { get; }
        //int GetValueIndex(Transform obj);
        ICode ConvertToCode();

        //Transform NextConnectionObject { get; }
    }
}