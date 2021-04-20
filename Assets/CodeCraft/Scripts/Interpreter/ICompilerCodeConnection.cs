using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interpreter
{
    public struct CodeConnection
    {
        public int codeIndex;
        public Transform connectObject;
        public ICompilerCode code;

        public CodeConnection(int index, Transform connectObject, ICompilerCode code)
        {
            this.codeIndex = index;
            this.connectObject = connectObject;
            this.code = code;
        }
    }

    /// <summary>
    /// Used for objects that create new scopes (eg events or if statements) or can add code to
    /// CodeConnections[0] is synonymous with NextConnection
    /// </summary>
    public interface ICompilerCodeConnection
    {
        ICompilerCode[] CodeConnections { get; }
        Transform[] CodeConnectionObjects { get; }
        CodeConnection? GetCodeConnection(Transform t);
    }
}