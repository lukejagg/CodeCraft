using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Interpreter
{
    /// <summary>
    /// The beginning object for any coding thing
    /// </summary>
    public class B_BeginEvent : MonoBehaviour, ICompilerEvent
    {
        public CodeType CodeType => CodeType.Event;

        public string SaveId => "Code:";
        public string Save()
        {
            return SaveId;
        }

        public ICompilerEvent NextConnection { get; set; }

        public Program ConvertToCode()
        {
            return null;
            //return EventUtil.RecursiveProgrammer(this);
        }

        public ICompilerCode[] CodeConnections => throw new System.NotImplementedException();
        public Transform[] CodeConnectionObjects => throw new System.NotImplementedException();
        public CodeConnection? GetCodeConnection(Transform t)
        {
            throw new System.NotImplementedException();
        }

        public Transform NextConnectionObject => nextConnectionObject;
        public Transform nextConnectionObject;

        public IOffset Offset { get { return null; } set { } }

        public float Width => 0;
        public float Height => 0;

        public void UpdateSize(int depth)
        {

        }
    }
}