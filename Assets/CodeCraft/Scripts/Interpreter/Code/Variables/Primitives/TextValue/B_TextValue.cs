using CodeEditor;
using ObjectEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_TextValue : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        public string SaveId => "T:";
        public string Save()
        {
            return SaveId + Saving.CodeValueQuote + inputField.text;
        }

        public void LoadValue(string str)
        {
	        inputField.text = str;
        }

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[0];

        public IOffset Offset { get; set; }

        public Transform[] valueObjects;

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

        public InputField inputField;

        public IValue ConvertToValue()
        {
            return new TextValue(inputField.text);
        }




        public float Width { get; private set; }
        public float Height { get; private set; }

        public RectTransform length;

        public RectTransform textBlock;

        public void UpdateSize(int depth)
        {
	        RescaleText();
        }

        public void RescaleText()
        {
	        var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
	        var selfSize = 76 * scale;
	        Height = selfSize;

	        var prefWidth = Mathf.Max(inputField.preferredWidth + 20, 60);

	        var rt = (RectTransform)inputField.transform;
	        rt.sizeDelta = new Vector2(prefWidth, 60);
	        rt.anchoredPosition = Vector2.zero;
	        length.sizeDelta = new Vector2(prefWidth + 16, 76);
	        length.localScale = Vector3.one;

	        Width = (prefWidth + 16) * scale;

	        textBlock.sizeDelta = Vector2.zero;

        }


        public Transform Transform => transform;

        public void ResizeAllText()
        {
	        CodeView.Instance.editor.ReconnectAll();
        }
    }
}