using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_Variable : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        public string SaveId => "Var";
        public InputField variableName;
        public string Save()
        {
            return SaveId + Saving.CodeValueQuote + variableName.text;
        }

        public void LoadValue(string str)
        {
	        variableName.text = str;
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
            return new VariableValue(variableName.text);
        }



        public float Width { get; set; }
        public float Height { get; set; }

        public RectTransform length;
        public Image blockImage;

        public void UpdateSize(int depth)
        {
	        var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
	        var selfSize = 76 * scale;
	        Height = selfSize;

	        var prefWidth = Mathf.Max(variableName.preferredWidth + 20, 60);

	        var rt = (RectTransform) variableName.transform;
	        rt.sizeDelta = new Vector2(prefWidth, 60);
	        rt.anchoredPosition = Vector2.zero;
	        length.sizeDelta = new Vector2(prefWidth + 16, 76);
	        length.localScale = Vector3.one;

	        Width = (prefWidth + 16) * scale;

            blockImage.color = CodeUtil.ChangeValueColor(Colors.VariableValueColor, depth);
        }

        public Transform Transform => transform;

        public void ResizeAllText()
        {
            CodeView.Instance.editor.ReconnectAll();
        }
    }
}