using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine; 

namespace Interpreter
{
    public interface ICompilerValue : ICompilerValues, IUseOffset, IBlockScale, ICodeType
    {
	    void LoadValue(string str);
        IValue ConvertToValue();
    }
}