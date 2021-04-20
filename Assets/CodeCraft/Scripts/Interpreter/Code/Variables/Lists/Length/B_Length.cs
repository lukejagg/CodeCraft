using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_Length : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        public string SaveId => "Len";
        public string Save()
        {
            return SaveId;
        }

        public void LoadValue(string str)
        {

        }

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[1];

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
            return new ListLength(values[0]?.ConvertToValue());
        }


        public float Width { get; private set; }
        public float Height { get; private set; }

        public RectTransform block;
        public Image blockImage;

        public void UpdateSize(int depth)
        {
            var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
            var selfSize = 76 * scale;
            var valueSize = selfSize * 0.9f;

            Width = 130 * scale;

            for (int i = 0; i < values.Length; i++)
            {
	            values[i]?.UpdateSize(depth + 1);
	            Width += values[i]?.Width ?? valueSize;
            }

            Height = selfSize;

            var w1 = 76 * 0.9f;
            if (values[0] != null)
            {
                w1 = values[0].Width / scale;
            }

            block.sizeDelta = new Vector2(Width / scale, 76);

            blockImage.color = CodeUtil.ChangeValueColor(Colors.VariableValueColor, depth);
        }

        public Transform Transform => transform;
    }
}