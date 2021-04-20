using CodeEditor;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Interpreter
{
    public class B_LastClone : MonoBehaviour, ICompilerValue
    {
        public CodeType CodeType => CodeType.Value;

        public string SaveId => "last";
        public string Save()
        {
            return SaveId;
        }

        public void LoadValue(string str)
        {
        }

        public ICompilerValue[] Values => values;
        public ICompilerValue[] values = new ICompilerValue[0];

        public IOffset Offset { get; set; }

        public CodeValue? GetValue(Transform obj)
        {
	        return null;
        }

        public IValue ConvertToValue()
        {
            return new CloneValue();
        }



        public float Width { get; set; }
        public float Height { get; set; }

        public RectTransform length;

        public void UpdateSize(int depth)
        {
	        var scale = transform.lossyScale.x / CodeView.Instance.rectTransform.lossyScale.x;
	        var selfSize = 76 * scale;

	        Width = selfSize;
	        Height = selfSize;

	        length.sizeDelta = new Vector2(76, 76);
	        length.localScale = Vector3.one;
        }

        public Transform Transform => transform;
    }
}