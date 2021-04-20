using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
    public class Add : IValue
    {
        public bool ReadOnly => true;

        IValue value1 = null;
        IValue value2 = null;

        public Add(IValue value1, IValue value2)
        {
            this.value1 = value1;
            this.value2 = value2;
        }

        public object GetValue(CodeExecutor obj)
        {
	        var v1 = value1?.GetValue(obj);
	        var v2 = value2?.GetValue(obj);

	        if (v1 is string str)
	        {
		        return $"{str}{v2}";
	        }

	        if (v2 is string str2)
	        {
		        return $"{v1}{str2}";
	        }

			if (v1 is int i)
			{
				if (v2 is int j)
					return i + j;
				if (v2 is BigInteger bj)
					return i + bj;
				if (v2 is float f)
					return i + f;
			}
			else if (v1 is float fi)
			{
				if (v2 is int j)
					return fi + j;
				if (v2 is BigInteger bj)
					return fi + (float)bj;
				if (v2 is float f)
					return fi + f;
			}
			else if (v1 is BigInteger bi)
			{
				if (v2 is int j)
					return bi + j;
				if (v2 is BigInteger bj)
					return bi + bj;
				if (v2 is float f)
					return (float)bi + f;
			}
			else if (v1 is Vector3 vec1)
			{
				if (v2 is Vector3 vec2)
					return vec1 + vec2;
				if (v2 is Vector3Int vec2i)
					return vec1 + vec2i;
			}
			else if (v1 is Vector3Int vec1i)
			{
				if (v2 is Vector3 vec2)
					return vec1i + vec2;
				if (v2 is Vector3Int vec2i)
					return vec1i + vec2i;
			}

			return 0;
        }
    }
}