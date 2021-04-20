using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_Broadcast : MonoBehaviour, ICompilerCode
    {
        public CodeType CodeType => CodeType.Function;

        public string SaveId => "Broadcast";
        public InputField broadcastName;
        public string Save()
        {
            return SaveId + Saving.FunctionSplit + broadcastName.text; ;
        }

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[0];

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
            return new Broadcast(broadcastName.text);
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
	        //Width = 420;
	        //block.sizeDelta = new Vector2(Width, 76);
	        //blockImage.color = CodeUtil.ChangeFunctionColor(Colors.GeneralColor, depth);

	        UpdateWidth();
        }

        public Transform Transform => transform;

        public void UpdateWidth()
        {
	        var prefWidth = Mathf.Max(broadcastName.preferredWidth + 20, 80);

	        var rt = (RectTransform)broadcastName.transform;
	        rt.sizeDelta = new Vector2(prefWidth, 50);
	        block.sizeDelta = new Vector2(prefWidth + 228, 76);
        }

        public void Start()
        {
	        broadcastName.onValueChanged.AddListener(delegate
	        {
		        UpdateWidth();
	        });
        }
    }
}