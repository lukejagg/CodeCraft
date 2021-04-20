using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_VariableEvent : MonoBehaviour, ICompilerEvent
    {
        public CodeType CodeType => CodeType.Event;

        public string SaveId => "VarEvent";
        public InputField variableName;

        public string Save()
        {
            return SaveId + Saving.VariableSplit + variableName.text;
        }

        public ICompilerEvent NextConnection { get; set; }

        public Program ConvertToCode()
        {
            return EventUtil.RecursiveProgrammer(this);
        }

        public Transform NextConnectionObject => nextConnectionObject;
        public Transform nextConnectionObject;

        public IOffset Offset { get; set; }


        public ICompilerCode[] CodeConnections => null;
        public Transform[] CodeConnectionObjects => null;

        public CodeConnection? GetCodeConnection(Transform t)
        {
            return null;
        }


        public float Width => 0;
        public float Height => 0;

        public RectTransform block;
        public Image blockImage;

        public void UpdateSize(int depth)
        {
	        var prefWidth = Mathf.Max(variableName.preferredWidth + 20, 80);

	        var rt = (RectTransform)variableName.transform;
	        rt.sizeDelta = new Vector2(prefWidth, 50);
	        block.sizeDelta = new Vector2(prefWidth + 120, 76);

            blockImage.color = CodeUtil.ChangeFunctionColor(Colors.VariableValueColor, depth);
        }

        public void Start()
        {
	        variableName.onEndEdit.AddListener(delegate { UpdateVariables(); });
            variableName.onValueChanged.AddListener(delegate
            {
	            var prefWidth = Mathf.Max(variableName.preferredWidth + 20, 60);

	            var rt = (RectTransform)variableName.transform;
	            rt.sizeDelta = new Vector2(prefWidth, 50);
	            block.sizeDelta = new Vector2(prefWidth + 112, 76);
            });
        }

        public void UpdateVariables()
        {
	        CodeView.Instance.OpenVariables();
        }
    }
}