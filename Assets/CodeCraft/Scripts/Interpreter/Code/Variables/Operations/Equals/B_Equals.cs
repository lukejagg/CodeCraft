using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_Equals : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        public string SaveId => "Eq";
        public string Save()
        {
            return SaveId + Saving.CodeValueQuote + dropdown.value;
        }

        public void LoadValue(string str)
        {
	        if (int.TryParse(str, out var i))
		        dropdown.value = i;
	        else
				dropdown.value = 0;
        }

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[2];

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

        public IValue ConvertToValue()
        {
            return new Equal(values[0]?.ConvertToValue(), values[1]?.ConvertToValue(), (EqualType)dropdown.value);
        }


        public float Width { get; private set; }
        public float Height { get; private set; }

        public RectTransform block, dropdownRect, val2;
        public Image theme;

        public Dropdown dropdown;

        public void UpdateSize(int depth)
        {
            var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
            var selfSize = 76 * scale;
            var valueSize = selfSize * 0.9f;

            Width = 61 * scale;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] != null)
                {
                    values[i].UpdateSize(depth + 1);
                    Width += values[i].Width;// - valueSize;
                }
                else
                {
                    Width += valueSize;
                }
            }

            Height = selfSize;

            var w1 = 76 * 0.9f;
            if (values[0] != null)
            {
                w1 = values[0].Width / scale;
            }

            dropdownRect.anchoredPosition = new Vector2(w1 + 8, 0);
            val2.anchoredPosition = new Vector2(w1 + 57, 0);
            block.sizeDelta = new Vector2(Width / scale, 76);

            theme.color = CodeUtil.ChangeValueColor(Colors.OperatorColor, depth);
        }

        public Transform Transform => transform;
    }
}