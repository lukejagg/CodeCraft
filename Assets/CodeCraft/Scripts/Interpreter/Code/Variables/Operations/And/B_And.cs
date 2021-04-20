using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_And : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        public string SaveId => "&";
        public string Save()
        {
            return SaveId;
        }

        public void LoadValue(string str)
        {

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
            return new And(values[0]?.ConvertToValue(), values[1]?.ConvertToValue());
        }


        public float Width { get; private set; }
        public float Height { get; private set; }

        public RectTransform block, text, val2;
        public Image theme;

        public void UpdateSize(int depth)
        {
            var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
            var selfSize = 76 * scale;
            var valueSize = selfSize * 0.9f;

            Width = 55 * scale;

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

            text.anchoredPosition = new Vector2(w1 + 28, 0);
            val2.anchoredPosition = new Vector2(w1 + 53, 0);
            block.sizeDelta = new Vector2(Width / scale, 76);

            theme.color = CodeUtil.ChangeValueColor(Colors.OperatorColor, depth);
        }

        public Transform Transform => transform;
    }
}