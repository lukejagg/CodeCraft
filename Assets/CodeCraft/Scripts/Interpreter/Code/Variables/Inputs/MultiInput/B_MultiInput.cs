using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_MultiInput : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        public string SaveId => "MulIn";
        public string Save()
        {
            return SaveId + Saving.CodeValueQuote + (int)moveType;
        }

        public MultiInputCode moveType;

        public void LoadValue(string str)
        {
	        if (int.TryParse(str, out var i))
	        {
		        moveType = (MultiInputCode) i;
	        }
        }

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[0];

        public IOffset Offset { get; set; }

        public CodeValue? GetValue(Transform obj)
        {
	        return null;
        }

        public IValue ConvertToValue()
        {
            return new MultiInput(moveType);
        }



        public float Width { get; set; }
        public float Height { get; set; }

        public RectTransform length;
        public Image blockImage;

        public Text text;

        public void UpdateSize(int depth)
        {
	        text.text = $"<b>{(moveType == MultiInputCode.Look ? "look" : "move")}</b> input";

	        var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
	        var selfSize = 76 * scale;

	        Width = 160 * scale;
	        Height = selfSize;

	        length.sizeDelta = new Vector2(160, 76);
	        length.localScale = Vector3.one;
        }

        public Transform Transform => transform;
    }
}