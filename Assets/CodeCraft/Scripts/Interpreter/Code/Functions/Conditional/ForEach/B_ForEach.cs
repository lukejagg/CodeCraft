using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_ForEach : MonoBehaviour, ICompilerCode
    {
        public CodeType CodeType => CodeType.Function;

        public string SaveId => "ForEach";
        public string Save()
        {
            return SaveId;
        }

        public IOffset Offset { get; set; }

        public Transform[] valueObjects;

        public ICode ConvertToCode()
        {
            return new ForEach(Values[2]?.ConvertToValue(), Values[0]?.ConvertToValue(), Values[1]?.ConvertToValue(), EventUtil.RecursiveProgrammer(codeConnections[1]));
        }

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[3];

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


        public ICompilerCode[] CodeConnections => codeConnections;
        public ICompilerCode[] codeConnections = new ICompilerCode[2];
        public Transform[] CodeConnectionObjects => codeConnectionObjects;
        public Transform[] codeConnectionObjects = new Transform[2];

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

        public float Height { get; private set; }

        public RectTransform block, scope, endConnection;
        public Image[] blockImages;

        public RectTransform text3;

        public void UpdateSize(int depth)
        {
	        Width = 0;

	        float w1 = 0, w2 = 0;

	        for (int i = 0; i < values.Length; i++)
	        {
		        if (i == 1) w1 = Width;
		        else if (i == 2) w2 = Width;

		        values[i]?.UpdateSize(0);
		        Width += values[i]?.Width ?? CodeUtil.SubWidth;
	        }

	        text3.anchoredPosition = new Vector2(w2 + 198, 0);
	        (valueObjects[1] as RectTransform).anchoredPosition = new Vector2(w1 + 174, 0);
	        (valueObjects[2] as RectTransform).anchoredPosition = new Vector2(w2 + 222, 0);

            if (depth >= 0)
            {
                Height = 200;
                ICompilerCode connection = codeConnections[1];
                while (connection != null)
                {
                    connection.UpdateSize(depth + 1);
                    Height += connection.Height;
                    connection = connection.CodeConnections[0];
                }

                scope.sizeDelta = new Vector2(40, Height - 120);
                endConnection.anchoredPosition = new Vector2(0, -Height);
            }

            Width += 224;
            block.sizeDelta = new Vector2(Mathf.Ceil(Width), 76);


                var c = CodeUtil.ChangeFunctionColor(Colors.ConditionalColor, depth);
                foreach (var i in blockImages)
                {
                    i.color = c;
                }
        }

        public Transform Transform => transform;
    }
}