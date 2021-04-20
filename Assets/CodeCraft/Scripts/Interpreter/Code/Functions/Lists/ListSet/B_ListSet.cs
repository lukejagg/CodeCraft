using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_ListSet : MonoBehaviour, ICompilerCode
    {
        public CodeType CodeType => CodeType.Function;

        public string SaveId => "ListSet";
        public string Save()
        {
            return SaveId;
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

        public ICode ConvertToCode()
        {
            return new ListSet(Values[2]?.ConvertToValue(), Values[0]?.ConvertToValue(), Values[1]?.ConvertToValue());
        }


        public ICompilerCode[] CodeConnections => codeConnections;
        public ICompilerCode[] codeConnections = new ICompilerCode[1];
        public Transform[] CodeConnectionObjects => codeConnectionObjects;
        public Transform[] codeConnectionObjects = new Transform[1];

        public CodeConnection? GetCodeConnection(Transform t)
        {
            for (int i = 0; i < codeConnectionObjects.Length; i++)
            {
                if (codeConnectionObjects[i] == t)
                {
                    return new CodeConnection(i, codeConnectionObjects[i], codeConnections[i]);
                }
            }
            return null;
        }



        public float Width { get; private set; }
        public float Height => 80;

        public RectTransform block;
        public Image blockImage;

        public RectTransform text2, text3;

        public void UpdateSize(int depth)
        {
	        var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
	        Width = 0;

	        float w1 = 0, w2 = 0;

	        for (int i = 0; i < values.Length; i++)
	        {
		        if (i == 1) w1 = Width;
                else if (i == 2) w2 = Width;

			    values[i]?.UpdateSize(0);
			    Width += values[i]?.Width ?? CodeUtil.SubWidth;
	        }

            text2.anchoredPosition = new Vector2(w1 + 115, 0);
            text3.anchoredPosition = new Vector2(w2 + 171, 0);
            (valueObjects[1] as RectTransform).anchoredPosition = new Vector2(w1 + 143, 0);
	        (valueObjects[2] as RectTransform).anchoredPosition = new Vector2(w2 + 196, 0);
            block.sizeDelta = new Vector2(Mathf.Ceil(Width + 198), 76);

	        blockImage.color = CodeUtil.ChangeFunctionColor(Colors.ListColor, depth);
        }

        public Transform Transform => transform;
    }
}