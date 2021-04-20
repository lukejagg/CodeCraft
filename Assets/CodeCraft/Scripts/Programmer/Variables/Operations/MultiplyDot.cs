using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
	public class MultiplyDot : IValue
	{
		public bool ReadOnly => true;

		IValue value1 = null;
		IValue value2 = null;

		public MultiplyDot(IValue value1, IValue value2)
		{
			this.value1 = value1;
			this.value2 = value2;
		}

		public string RepeatString(float f, string str)
		{
			var ret = "";
			f = Mathf.RoundToInt(f);
			for (int i = 0; i < f; i++)
			{
				ret += str;
			}

			return ret;
		}

		public object GetValue(CodeExecutor obj)
		{
			var v1 = value1?.GetValue(obj);
			var v2 = value2?.GetValue(obj);

			if (v1 is int i)
			{
				if (v2 is int j)
					return i * j;
				if (v2 is BigInteger bj)
					return i * bj;
				if (v2 is float f)
					return i * f;
				if (v2 is Vector3 v)
					return i * v;
				if (v2 is Vector3Int vi)
					return i * vi;
				if (v2 is string str)
					return RepeatString(i, str);
			}
			else if (v1 is float fi)
			{
				if (v2 is int j)
					return fi * j;
				if (v2 is BigInteger bj)
					return fi * (float)bj;
				if (v2 is float f)
					return fi * f;
				if (v2 is Vector3 v)
					return fi * v;
				if (v2 is Vector3Int vi)
					return fi * (Vector3)vi;
				if (v2 is string str)
					return RepeatString(fi, str);
			}
			else if (v1 is BigInteger bi)
			{
				if (v2 is int j)
					return bi * j;
				if (v2 is BigInteger bj)
					return bi * bj;
				if (v2 is float f)
					return (float)bi * f;
				if (v2 is Vector3 v)
					return (float)bi * v;
				if (v2 is Vector3Int vi)
					return (int)bi * vi;
				if (v2 is string str)
					return RepeatString((float)bi, str);
			}
			else if (v1 is Vector3 vec1)
			{
				if (v2 is Vector3 vec2)
					return Vector3.Dot(vec1, vec2);
				if (v2 is Vector3Int vec2i)
					return Vector3.Dot(vec1, vec2i);

				if (v2 is int j)
					return vec1 * j;
				if (v2 is BigInteger bj)
					return vec1 * (int)bj;
				if (v2 is float f)
					return vec1 * f;

				if (v2 is Quaternion q2)
					return q2 * vec1;
			}
			else if (v1 is Vector3Int vec1i)
			{
				if (v2 is Vector3 vec2)
					return Vector3.Dot(vec1i, vec2);
				if (v2 is Vector3Int vec2i)
					return vec1i.x * vec2i.x + vec1i.y * vec2i.y + vec1i.z * vec2i.z;

				if (v2 is int j)
					return vec1i * j;
				if (v2 is BigInteger bj)
					return vec1i * (int)bj;
				if (v2 is float f)
					return (Vector3)vec1i * f;

				if (v2 is Quaternion q2)
					return q2 * vec1i;
			}

			else if (v1 is Quaternion q)
			{
				if (v2 is Quaternion q2)
					return Quaternion.Dot(q, q2);

				if (v2 is Vector3 vec2)
					return q * vec2;
				if (v2 is Vector3Int vec2i)
					return q * vec2i;
			}

			return 0;
		}
	}
}