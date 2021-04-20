using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_ComponentOf : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        public VectorComponent component;
        public string SaveId => "com";
        public string Save()
        {
            return SaveId + Saving.CodeValueQuote + (int)component;
        }

        public void LoadValue(string str)
        {
	        component = (VectorComponent)int.Parse(str);
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
            return new ComponentOf(values[0].ConvertToValue(), component);
        }


        public float Width { get; private set; }
        public float Height { get; private set; }

        public RectTransform block;
        public Image blockImage;
        public Text mainText;

        public void UpdateSize(int depth)
        {
            var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
            var selfSize = 76 * scale;
            var valueSize = selfSize * 0.9f;

            Width = 59 * scale;

            for (int i = 0; i < values.Length; i++)
            {
                values[i]?.UpdateSize(depth + 1);
                Width += values[i]?.Width ?? valueSize;
            }

            Height = selfSize;

            block.sizeDelta = new Vector2(Width / scale, 76);

            blockImage.color = CodeUtil.ChangeValueColor(Colors.VariableValueColor, depth);

            switch (component)
            {
	            case VectorComponent.X:
		            mainText.text = "x";
		            break;
	            case VectorComponent.Y:
		            mainText.text = "y";
		            break;
                case VectorComponent.Z:
	                mainText.text = "z";
	                break;
            }
        }

        public Transform Transform => transform;
    }
}