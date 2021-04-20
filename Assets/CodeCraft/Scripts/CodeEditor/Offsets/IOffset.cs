using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeEditor
{
    public interface IOffset
    {
        Transform Connection { get; set; }
        void Connect();
        //void Connect(bool storeParent);
    }
}