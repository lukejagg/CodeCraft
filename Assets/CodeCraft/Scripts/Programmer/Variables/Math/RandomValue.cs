using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using ObjectEditor;
using UnityEngine;
using Random = System.Random;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
	public class RandomValue : IValue
	{
		public bool ReadOnly => true;

		IValue value1 = null;
		IValue value2 = null;


		Random rnd = new Random();

		public RandomValue(IValue value1, IValue value2)
		{
			this.value1 = value1;
			this.value2 = value2;
		}

		float RandomFloat(float x, float y)
		{
			return (float) (x + rnd.NextDouble() * (y - x));
		}

		int RandomInt(int x, int y)
		{
			return x + (int)(rnd.NextDouble() * (y - x));
		}

		public object GetValue(CodeExecutor obj)
		{
			var v1 = value1?.GetValue(obj);
			var v2 = value2?.GetValue(obj);

			if (v1 is int i1)
			{
				if (v2 is int j)
					return i1 + (int) (rnd.NextDouble() * (j - i1));
				if (v2 is BigInteger bj)
					return i1 + new BigInteger(rnd.NextDouble() * (float)(bj - i1));
				if (v2 is float f)
					return (float)(i1 + rnd.NextDouble() * (f - i1));
			}
			else if (v1 is BigInteger b1)
			{
				if (v2 is int j)
					return b1 + new BigInteger(rnd.NextDouble() * (float)(j - b1));
				if (v2 is BigInteger bj)
					return b1 + new BigInteger(rnd.NextDouble() * (float)(bj - b1));
				if (v2 is float f)
					return (float)((float)b1 + rnd.NextDouble() * (f - (float)b1));
			}
			else if (v1 is float f1)
			{
				if (v2 is int j)
					return (float)(f1 + rnd.NextDouble() * (j - f1));
				if (v2 is BigInteger bj)
					return (float)(f1 + rnd.NextDouble() * ((float)bj - f1));
				if (v2 is float f)
					return (float)(f1 + rnd.NextDouble() * (f - f1));
			}
			else if (v1 is Vector3 vec)
			{
				if (v2 is Vector3 vec2)
					return new Vector3(RandomFloat(vec.x, vec2.x), RandomFloat(vec.y, vec2.y), RandomFloat(vec.z, vec2.z));
				if (v2 is Vector3Int vec2i)
					return new Vector3(RandomFloat(vec.x, vec2i.x), RandomFloat(vec.y, vec2i.y), RandomFloat(vec.z, vec2i.z));
			}
			else if (v1 is Vector3Int veci)
			{
				if (v2 is Vector3 vec2)
					return new Vector3(RandomFloat(veci.x, vec2.x), RandomFloat(veci.y, vec2.y), RandomFloat(veci.z, vec2.z));
				if (v2 is Vector3Int vec2i)
					return new Vector3Int(RandomInt(veci.x, vec2i.x), RandomInt(veci.y, vec2i.y), RandomInt(veci.z, vec2i.z));
			}
			else if (v1 is string str)
			{
				if (v2 is string str2)
				{
					// Todo: Random strings
					//var ret = "";
					//for (var i = 0; i < len; i++)
				}
			}

			return null;
		}

		public void SetValue(IValue newValue)
		{

		}
	}
}