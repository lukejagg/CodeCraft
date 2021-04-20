using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_Run : MonoBehaviour, ICompilerCode
    {
        public CodeType CodeType => CodeType.Function;

        public string SaveId => "RunAsync";
        public string Save()
        {
            return SaveId;
        }

        public IOffset Offset { get; set; }

        public Transform[] valueObjects;

        public ICode ConvertToCode()
        {
            return new RunAsync(EventUtil.RecursiveProgrammer(codeConnections[1]));
        }

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[0];

        public CodeValue? GetValue(Transform obj)
        {
            for (int i = 0; i < valueObjects.Length; i++)
            {
                if (valueObjects[i] == obj)
                {
                    return new CodeValue(i, valueObjects[i], values[i]);
                }
            }
            return null;
        }


        public ICompilerCode[] CodeConnections => codeConnections;
        public ICompilerCode[] codeConnections = new ICompilerCode[2];
        public Transform[] CodeConnectionObjects => codeConnectionObjects;
        public Transform[] codeConnectionObjects = new Transform[2];

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



        public float Width { get; private set; }

        public float Height { get; private set; }

        public RectTransform block, scope, endConnection;
        public Image[] blockImages;

        public void UpdateSize(int depth)
        {
            Width = 193;

            if (depth >= 0)
            {
                Height = 200;
                ICompilerCode connection = codeConnections[1];
                while (connection != null)
                {
                    connection.UpdateSize(depth + 1);
                    Height += connection.Height;
                    connection = connection.CodeConnections[0];
                }

                scope.sizeDelta = new Vector2(40, Height - 120);
                endConnection.anchoredPosition = new Vector2(0, -Height);
            }

            block.sizeDelta = new Vector2(Mathf.Ceil(Width), 76);


            var c = CodeUtil.ChangeControlColor(Colors.ControlColor, depth);
            foreach (var i in blockImages)
            {
                i.color = c;
            }
        }

        public Transform Transform => transform;
    }
}