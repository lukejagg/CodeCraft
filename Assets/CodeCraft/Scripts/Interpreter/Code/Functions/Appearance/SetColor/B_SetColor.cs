using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_SetColor : MonoBehaviour, ICompilerCode
    {
        public CodeType CodeType => CodeType.Function;

        public string SaveId => "SetColor";
        public string Save()
        {
            return SaveId;
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

        public ICode ConvertToCode()
        {
            return new SetColor(Values?[0]?.ConvertToValue());
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

        public void UpdateSize(int depth)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i]?.UpdateSize(0);
            }

            for (int i = 1; i < codeConnections.Length; i++)
            {
                codeConnections[i]?.UpdateSize(depth + 1);
            }

            Width = 180;
            ICompilerValue value = values[0];
            if (value == null)
            {
                Width += CodeUtil.SubWidth;
            }
            else 
            { 
                Width += value.Width;
            }

            block.sizeDelta = new Vector2(Mathf.Ceil(Width), 76);

            //blockImage.color = CodeUtil.ChangeFunctionColor(Colors.GeneralColor, depth);
            
        }

        public Transform Transform => transform;
    }
}