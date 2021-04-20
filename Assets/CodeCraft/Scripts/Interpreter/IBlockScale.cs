using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interpreter
{
    public interface IBlockScale
    {
        float Width { get; }
        float Height { get; }
        void UpdateSize(int depth);
    }
}