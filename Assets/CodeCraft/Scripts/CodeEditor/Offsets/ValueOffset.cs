using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeEditor
{
    public class ValueOffset : MonoBehaviour, IOffset
    {
        public Transform Connection { get; set; }

        public Transform oldParent;
        RectTransform rectTransform;
        //public Transform connection;

        public void Connect()
        {
            //transform.position = Connection.position
            if (rectTransform == null)
            {
	            rectTransform = GetComponent<RectTransform>();
            }
            rectTransform.anchoredPosition = Vector3.zero;

            if (transform.parent != Connection)
            {
                transform.parent = Connection;
            }
        }

        public void Connect(bool storeParent)
        {
            if (storeParent)
            {
                if (rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }

                oldParent = transform.parent;
                transform.SetParent(Connection, true);
                rectTransform.anchoredPosition = new Vector3(0, 40);
                transform.localScale = Vector3.one;
            }
            else
            {
                transform.SetParent(oldParent, false);
            }
        }
    }
}