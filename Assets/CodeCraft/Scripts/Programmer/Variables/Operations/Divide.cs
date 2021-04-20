using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
    public class Divide : IValue
    {
        public bool ReadOnly => true;

        IValue value1 = null;
        IValue value2 = null;

        public Divide(IValue value1, IValue value2)
        {
            this.value1 = value1;
            this.value2 = value2;
        }

        public object GetValue(CodeExecutor obj)
        {
	        var v1 = value1?.GetValue(obj);
	        var v2 = value2?.GetValue(obj);

	        if (v1 is int i)
	        {
		        if (v2 is int j)
			        return j == 0 ? 0 : i / j;
		        if (v2 is BigInteger bj)
			        return bj == 0 ? 0 : i / bj;
		        if (v2 is float f)
			        return f == 0 ? 0 : i / f;
	        }
	        else if (v1 is float fi)
	        {
		        if (v2 is int j)
			        return j == 0 ? 0 : fi / j;
		        if (v2 is BigInteger bj)
			        return bj == 0 ? 0 : fi / (float)bj;
		        if (v2 is float f)
			        return f == 0 ? 0 : fi / f;
	        }
	        else if (v1 is BigInteger bi)
	        {
		        if (v2 is int j)
			        return j == 0 ? 0 : bi / j;
		        if (v2 is BigInteger bj)
			        return bj == 0 ? 0 : bi / bj;
		        if (v2 is float f)
			        return f == 0 ? 0 : (float)bi / f;
	        }
	        else if (v1 is Vector3 vec1)
	        {
		        if (v2 is int j)
			        return vec1 / j;
		        if (v2 is BigInteger bj)
			        return vec1 / (int)bj;
		        if (v2 is float f)
			        return vec1 / f;
		        if (v2 is Quaternion q2)
			        return Quaternion.Inverse(q2) * vec1;
			}
	        else if (v1 is Vector3Int vec1i)
	        {
		        if (v2 is int j)
			        return vec1i / j;
		        if (v2 is BigInteger bj)
			        return vec1i / (int)bj;
		        if (v2 is float f)
			        return (Vector3)vec1i / f;
		        if (v2 is Quaternion q2)
			        return Quaternion.Inverse(q2) * vec1i;
			}
			else if (v1 is Quaternion q)
	        {
		        if (v2 is Quaternion q2)
			        return q * Quaternion.Inverse(q2);

		        if (v2 is Vector3 vec2)
			        return Quaternion.Inverse(q) * vec2;
		        if (v2 is Vector3Int vec2i)
			        return Quaternion.Inverse(q) * vec2i;
			}

			return 0;
        }
    }
}