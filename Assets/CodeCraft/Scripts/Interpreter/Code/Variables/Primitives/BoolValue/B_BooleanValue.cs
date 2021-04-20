using CodeEditor;
using ObjectEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_BooleanValue : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        public string SaveId => "B:";
        public string Save()
        {
            return SaveId + Saving.CodeValueQuote + (toggle.isOn ? 1 : 0);
        }

        public void LoadValue(string str)
        {
	        toggle.isOn = str.Length > 0 && str[0] == '1';
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

        public Toggle toggle;

        public IValue ConvertToValue()
        {
            return new BooleanValue(toggle.isOn);
        }


        public float Width { get; private set; }
        public float Height { get; private set; }

        public RectTransform length;

        public void UpdateSize(int depth)
        {
            var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
            var selfSize = 76 * scale;

            Width = selfSize;
            Height = selfSize;

            length.sizeDelta = new Vector2(76, 76);
            length.localScale = Vector3.one;
        }

        public Transform Transform => transform;
    }
}