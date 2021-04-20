using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using Programmer;

namespace Interpreter
{
    public class EventUtil
    {
        public static Program RecursiveProgrammer(ICompilerCodeConnection compiledEvent)
        {
            return RecursiveProgrammer(compiledEvent.CodeConnections?[0]);
        }

        public static Program RecursiveProgrammer(ICompilerCode initialCode)
        {
            var code = new List<ICode>();
            var currentCode = initialCode;
            while (currentCode != null)
            {
	            var c = currentCode.ConvertToCode();
                if (c != null) code.Add(c);
                currentCode = currentCode.CodeConnections[0];
            }
            return new Program(code.ToArray());
        }
    }
}