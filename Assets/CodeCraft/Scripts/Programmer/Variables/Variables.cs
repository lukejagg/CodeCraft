using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Programmer
{
    public class Variables : MonoBehaviour
    {
        Dictionary<string, IValue> values = new Dictionary<string, IValue>();
        
        public IValue this[string name]
        {
            get { return values[name]; }
            set
            {
                if (values.ContainsKey(name))
                {
                    values[name] = value;
                }
                else
                {
                    values.Add(name, value);
                }
            }
        }
    }
}