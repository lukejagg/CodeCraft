using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_MathOperator : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        // 1 Operator
        public Dropdown dropdown;
        public Text dropdownText;
        public string SaveId => "m1";
        public string Save()
        {
            return SaveId + Saving.CodeValueQuote + dropdown.value;
        }

        public void LoadValue(string str)
        {
	        dropdown.value = int.Parse(str);
        }

        public Transform[] valueObjects;

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[1];

        public IOffset Offset { get; set; }

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

        public IValue ConvertToValue()
        {
            return new MathOperator(values[0]?.ConvertToValue(), (MathOperators)dropdown.value);
        }



        public float Width { get; set; }
        public float Height { get; set; }

        public RectTransform length;
        public Image blockImage;

        public void UpdateSize(int depth)
        {
	        values[0]?.UpdateSize(depth + 1);
            RescaleText(depth);
        }

        public Transform Transform => transform;

        public void RescaleText(int depth)
        {
	        var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
	        var selfSize = 76 * scale;
	        Height = selfSize;

	        var prefWidth = Mathf.Max(dropdownText.preferredWidth + 27, 60);

            var w1 = values[0]?.Width ?? selfSize * 0.9f;
            Width = w1;

            (valueObjects[0] as RectTransform).anchoredPosition = new Vector2(prefWidth + 3, 0);

            //var rt = (RectTransform)dropdownText.transform;
            //rt.sizeDelta = new Vector2(prefWidth, 60);
            //rt.anchoredPosition = Vector2.zero;
            var len = prefWidth + 7;
	        length.sizeDelta = new Vector2(w1 / scale + len, 76);
	        (dropdown.transform as RectTransform).sizeDelta = new Vector2(prefWidth - 11, 60);
	        //length.localScale = Vector3.one;
	        Width += len * scale;

            blockImage.color = CodeUtil.ChangeValueColor(Colors.NotColor, depth);
        }

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