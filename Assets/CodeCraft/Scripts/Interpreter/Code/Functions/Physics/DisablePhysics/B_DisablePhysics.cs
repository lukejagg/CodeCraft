using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_DisablePhysics : MonoBehaviour, ICompilerCode
    {
        public CodeType CodeType => CodeType.Function;

        public string SaveId => "DisablePhysics";
        public string Save()
        {
            return SaveId;
        }

        public ICompilerValue[] Values => new ICompilerValue[0];

        public IOffset Offset { get; set; }

        public CodeValue? GetValue(Transform obj)
        {
            return null;
        }

        public ICode ConvertToCode()
        {
	        return new TogglePhysics(false);
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

        public void UpdateSize(int depth)
        {
	        Width = 295;

            block.sizeDelta = new Vector2(Mathf.Ceil(Width), 76);

               // blockImage.color = CodeUtil.ChangeFunctionColor(Colors.FunctionColor, depth);
            
        }

        public Transform Transform => transform;
    }
}