using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_IfElse : MonoBehaviour, ICompilerCode
    {
        public CodeType CodeType => CodeType.Function;

        public string SaveId => "IfElse";
        public string Save()
        {
            return SaveId;
        }

        public IOffset Offset { get; set; }

        public Transform[] valueObjects;

        public ICode ConvertToCode()
        {
	        var val = Values[0]?.ConvertToValue();
	        //bool createProgram = B_If.CloneCompileOptimization(val, out var checks);
	        return new IfElse(val, 
		         EventUtil.RecursiveProgrammer(codeConnections[1]), 
		        EventUtil.RecursiveProgrammer(codeConnections[2]));
        }

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[1];

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
        public ICompilerCode[] codeConnections = new ICompilerCode[3];
        public Transform[] CodeConnectionObjects => codeConnectionObjects;
        public Transform[] codeConnectionObjects = new Transform[3];

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

        public RectTransform block, scope1, scope2, nextCon;
        public Image[] blockImages;





        void Start()
        {
            scope2.SetAsFirstSibling();
        }




        public Transform[] codeTest2 = new Transform[3];

        public void UpdateSize(int depth)
        {
            /*for (int i = 0; i < values.Length; i++)
            {
                values[i]?.UpdateSize();
            }

            for (int i = 1; i < codeConnections.Length; i++)
            {
                codeConnections[i]?.UpdateSize();
            }*/

            for (int i = 0; i < codeConnections.Length; i++)
            {
                codeTest2[i] = codeConnections[i]?.Transform;
            }

            Width = 260 - CodeUtil.SubWidth;
            ICompilerValue value = values[0];
            if (value != null)
            {
                value.UpdateSize(0);
                Width += value.Width;
                //value = value.Values?.Length > 0 ? value.Values?[0] : null;
            }
            else
            {
	            Width += CodeUtil.SubWidth;
            }

            if (depth >= 0)
            {
                var c1 = 0f;
                var c2 = 0f;

                Height = 360;
                ICompilerCode connection = codeConnections[1];
                while (connection != null)
                {
                    connection.UpdateSize(depth + 1);
                    Height += connection.Height;
                    c1 += connection.Height;
                    connection = connection.CodeConnections[0];
                }

                connection = codeConnections[2];
                while (connection != null)
                {
                    connection.UpdateSize(depth + 1);
                    Height += connection.Height;
                    c2 += connection.Height;
                    connection = connection.CodeConnections[0];
                }

                scope1.sizeDelta = new Vector2(40, codeConnections[1] != null ? c1 + 80 : 80);
                scope2.anchoredPosition = new Vector2(0, codeConnections[1] != null ? -c1 - 240 : -240);
                scope2.sizeDelta = new Vector2(40, codeConnections[2] != null ? c2 + 40 : 40);
                nextCon.anchoredPosition = new Vector2(0, -Height);
            }

            block.sizeDelta = new Vector2(Mathf.Ceil(Width), 76);

                var c = CodeUtil.ChangeControlColor(Colors.ControlColor, depth);
                foreach (var i in blockImages)
                {
                    i.color = c;
                }
        }

        public Transform Transform => transform;
    }
}