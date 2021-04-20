﻿using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public class FixedDeltaTime : IValue
    {
        public bool ReadOnly => true;

        public object GetValue(CodeExecutor obj)
        {
            return Time.fixedDeltaTime;
        }
    }
}