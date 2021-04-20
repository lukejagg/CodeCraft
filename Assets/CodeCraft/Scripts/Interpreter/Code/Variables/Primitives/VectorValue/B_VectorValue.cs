using CodeEditor;
using ObjectEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Interpreter
{
    public class B_VectorValue : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        public string SaveId => "V:";
        public string Save()
        {
	        return SaveId;
	        //return $"{SaveId}{Saving.CodeValueQuote}{P(inputFields[0].dropdownRect)}:{P(inputFields[1].dropdownRect)}:{P(inputFields[2].dropdownRect)}";
        }

        public void LoadValue(string str)
        {
	        //var s = str.Split(':');
	        //for (int i = 0; i < 3; i++) inputFields[i].dropdownRect = s[i];
        }

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[3];

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

        public float C(string str)
        {
	        if (float.TryParse(str, out var num))
	        {
		        return num;
	        }

	        return 0;
        }

        public IValue ConvertToValue()
        {
            return new VectorValue(values[0]?.ConvertToValue(), values[1]?.ConvertToValue(), values[2]?.ConvertToValue());
        }




        public float Width { get; private set; }
        public float Height { get; private set; }

        public RectTransform length;

        public void UpdateSize(int depth)
        {
	        var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
	        var selfSize = 76 * scale;
	        var valueSize = selfSize * 0.9f;

	        Width = 0;

	        float w1 = 0f, w2 = 0f;

	        for (int i = 0; i < values.Length; i++)
	        {
		        if (i == 1) w1 = Width;
		        if (i == 2) w2 = Width;

			    values[i]?.UpdateSize(depth + 1);
			    Width += values[i]?.Width ?? valueSize;
	        }

	        Height = selfSize;

	        Width += 94 * scale;
            
            (valueObjects[1] as RectTransform).anchoredPosition = new Vector2(w1 / scale + 86, 0);
            (valueObjects[2] as RectTransform).anchoredPosition = new Vector2(w2 / scale + 90, 0);

            length.sizeDelta = new Vector2(Width / scale, 76);
        }


        public Transform Transform => transform;
    }
}