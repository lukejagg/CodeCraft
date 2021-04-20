using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Interpreter
{
    public class B_TouchBeganEvent : MonoBehaviour, ICompilerEvent
    {
        public CodeType CodeType => CodeType.Event;

        public string SaveId => "TBegan";
        public string Save()
        {
            return SaveId;
        }

        public ICompilerEvent NextConnection { get; set; }

        public Program ConvertToCode()
        {
            return EventUtil.RecursiveProgrammer(this);
        }

        public Transform NextConnectionObject => nextConnectionObject;
        public Transform nextConnectionObject;

        public IOffset Offset { get; set; }

        public ICompilerCode[] CodeConnections => codeConnections;
        public ICompilerCode[] codeConnections = new ICompilerCode[1];
        public Transform[] CodeConnectionObjects => codeConnectionObjects;
        public Transform[] codeConnectionObjects = new Transform[1];

        public CodeConnection? GetCodeConnection(Transform t)
        {
            for (int i = 0; i < codeConnectionObjects.Length; i++)
            {
                if (codeConnectionObjects[i] == t)
                {
                    return new CodeConnection(i, codeConnectionObjects[i], codeConnections[i]);
                }
            }
            return null;
        }

        public float Width => 0;
        public float Height { get; private set; }

        public RectTransform scope, bottomConnection;

        public void UpdateSize(int depth)
        {
            Height = 200;
            ICompilerCode connection = codeConnections[0];
            while (connection != null)
            {
                Height += connection.Height;
                connection = connection.CodeConnections[0];
            }

            scope.sizeDelta = new Vector2(40, Height - 120);
            bottomConnection.anchoredPosition = new Vector2(0, -Height - 80);
        }
    }
}