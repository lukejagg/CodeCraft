using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_SetVariable : MonoBehaviour, ICompilerCode
    {
        public CodeType CodeType => CodeType.Function;

        public string SaveId => "SetVar";
        public string Save()
        {
            return SaveId;
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

        public ICode ConvertToCode()
        {
            if (Values[0] != null)
				return new SetVariable(Values[0]?.ConvertToValue(), Values[1]?.ConvertToValue());

            return null;
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

        public RectTransform block;//, value1;
        public Image blockImage;

        public RectTransform text2;

        public void UpdateSize(int depth)
        {
	        var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
	        Width = 0;

	        float w1 = 0;

	        for (int i = 0; i < values.Length; i++)
	        {
		        if (i == 1)
		        {
			        w1 = Width;
		        }

                if (values[i] != null)
		        {
			        values[i].UpdateSize(0);
			        Width += values[i].Width;
		        }
		        else
		        {
			        Width += CodeUtil.SubWidth;
		        }
	        }

	        text2.anchoredPosition = new Vector2(w1 + 115, 0);
            (valueObjects[1] as RectTransform).anchoredPosition = new Vector2(w1 + 142, 0);
            Width += 146;
            block.sizeDelta = new Vector2(Mathf.Ceil(Width), 76);

                blockImage.color = CodeUtil.ChangeFunctionColor(Colors.VariableColor, depth);
            
        }

        public Transform Transform => transform;
    }
}