using Interpreter;
using ObjectEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CodeEditor
{
    public class SavedScript : MonoBehaviour
    {
        public SelectableObject obj;
        public B_BeginEvent beginEvent;

        string SaveValue(ICompilerValue value)
        {
            if (value == null) return Saving.CodeValueBegin.ToString() + Saving.CodeValueEnd.ToString();

            var str = Saving.CodeValueBegin.ToString();
            str += value.Save();
            foreach (var v in value.Values)
            {
                str += SaveValue(v);
            }
            str += Saving.CodeValueEnd.ToString();
            return str;
        }

        string SaveBlock(ICompilerCode code)
        {
            if (code == null) return Saving.CodeScopeBegin.ToString() + Saving.CodeScopeEnd.ToString();

            var str = Saving.CodeScopeBegin.ToString();
            while (code != null)
            {
                str += code.Save();
                if (code.Values.Length == 0)
                {
	                str += SaveValue(null);
                }
                else
                {
	                foreach (var v in code.Values)
	                {
		                str += SaveValue(v);
	                }
                }

                for (int i = 1; i < code.CodeConnections.Length; i++)
                {
                    str += SaveBlock(code.CodeConnections[i]);
                }

                code = code.CodeConnections[0];
            }
            str += Saving.CodeScopeEnd;
            return str;
        }

        public string Save()
        {
            var currentEvent = beginEvent.NextConnection;
            var save = new StringBuilder();
            while (currentEvent != null)
            {
                // serialize crap
                var str = "";
                str += currentEvent.Save();
                str += SaveBlock(currentEvent.CodeConnections?[0]);
                currentEvent = currentEvent.NextConnection;
                save.Append(str);
            }
            return save.ToString();
        }
    }
}