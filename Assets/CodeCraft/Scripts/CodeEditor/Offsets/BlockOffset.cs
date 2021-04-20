using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeEditor
{
    public class BlockOffset : MonoBehaviour, IOffset
    {
        public Transform Connection { get; set; }

        public Transform oldParent;
        //public Transform connection;

        public void Connect()
        {
	        if (Connection == null) return;

            transform.position = Connection.position;
            transform.SetAsLastSibling();
        }
    }
}