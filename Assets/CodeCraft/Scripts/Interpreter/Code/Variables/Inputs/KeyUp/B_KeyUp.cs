using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_KeyUp : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        public string SaveId => "KeyUp";
        public string Save()
        {
            return SaveId + Saving.CodeValueQuote + dropdown.value;
        }

        public void LoadValue(string str)
        {
	        if (int.TryParse(str, out var i))
	        {
		        dropdown.value = i;
	        }
        }

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[0];

        public IOffset Offset { get; set; }

        public CodeValue? GetValue(Transform obj)
        {
	        return null;
        }

        public Image blockImage;
        public Dropdown dropdown;
        public Text dropdownText;

        public IValue ConvertToValue()
        {
            // Todo: KeyCode Primitive type
            return new KeyUp((PlayInputCode)dropdown.value);
            //return new KeyPressed(keyCode);
        }

        public void RescaleText(int depth)
        {
	        var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
	        var selfSize = 76 * scale;
	        Height = selfSize;

	        var prefWidth = Mathf.Max(dropdownText.preferredWidth + 27, 60);

	        var w1 = 40;
	        Width = w1;

	        //var rt = (RectTransform)dropdownText.transform;
	        //rt.sizeDelta = new Vector2(prefWidth, 60);
	        //rt.anchoredPosition = Vector2.zero;
	        var len = prefWidth + 7;
	        length.sizeDelta = new Vector2(w1 + len, 76);
	        (dropdown.transform as RectTransform).sizeDelta = new Vector2(prefWidth - 11, 60);
	        //length.localScale = Vector3.one;
	        Width += len * scale;

	        //blockImage.color = CodeUtil.ChangeValueColor(Colors.InputColor, depth);
        }

        public float Width { get; set; }
        public float Height { get; set; }

        public RectTransform length;

        public void UpdateSize(int depth)
        {
	        RescaleText(depth);
        }

        public Transform Transform => transform;

        public void Start()
        {
	        dropdown.onValueChanged.AddListener(delegate
	        {
		        RescaleText(-1);
		        CodeView.Instance.editor.ReconnectAll();
	        });
        }
    }
}