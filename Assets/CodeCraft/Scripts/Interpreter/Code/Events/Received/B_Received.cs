using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_Received : MonoBehaviour, ICompilerEvent
    {
        public CodeType CodeType => CodeType.Event;

        public string SaveId => "Receive";
        public InputField broadcastName;
        public string Save()
        {
            return SaveId + Saving.VariableSplit + broadcastName.text;
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

            UpdateWidth();
        }

        public RectTransform block;
        public InputField receivedName;

        public void UpdateWidth()
        {
	        var prefWidth = Mathf.Max(receivedName.preferredWidth + 20, 80);

	        var rt = (RectTransform)receivedName.transform;
	        rt.sizeDelta = new Vector2(prefWidth, 50);
	        block.sizeDelta = new Vector2(prefWidth + 228, 76);
        }

        public void Start()
        {
	        receivedName.onValueChanged.AddListener(delegate
	        {
		        UpdateWidth();
	        });
        }
    }
}