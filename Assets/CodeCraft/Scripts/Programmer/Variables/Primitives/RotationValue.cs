using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
	// YXZ Euler Angles
	public class RotationValue : IValue
	{
		public bool ReadOnly => false;

		private IValue value1, value2, value3;

		public RotationValue(IValue value1, IValue value2, IValue value3)
		{
			this.value1 = value1;
			this.value2 = value2;
			this.value3 = value3;
		}

		bool IsInt(object i)
		{
			return i is int || i is BigInteger;
		}

		public object GetValue(CodeExecutor obj)
		{
			try
			{
				var v1 = value1 == null ? 0 : value1.GetValue(obj);
				var v2 = value2 == null ? 0 : value2.GetValue(obj);
				var v3 = value3 == null ? 0 : value3.GetValue(obj);

				v1 = v1 == null ? 0 : v1;
				v2 = v2 == null ? 0 : v2;
				v3 = v3 == null ? 0 : v3;

				bool isI1 = IsInt(v1), isI2 = IsInt(v2), isI3 = IsInt(v3);

				if (isI1 && isI2 && isI3)
				{
					return Quaternion.Euler(new Vector3Int(
						v1 is BigInteger ? (int)(BigInteger)v1 : (int)v1,
						v2 is BigInteger ? (int)(BigInteger)v2 : (int)v2,
						v3 is BigInteger ? (int)(BigInteger)v3 : (int)v3
					));
				}

				isI1 = isI1 || v1 is float;
				isI2 = isI2 || v2 is float;
				isI3 = isI3 || v3 is float;

				if (isI1 && isI2 && isI3)
				{
					return Quaternion.Euler(new Vector3(
						v1 is float ? (float)v1 : v1 is BigInteger ? (float)(BigInteger)v1 : (int)v1,
						v2 is float ? (float)v2 : v2 is BigInteger ? (float)(BigInteger)v2 : (int)v2,
						v3 is float ? (float)v3 : v3 is BigInteger ? (float)(BigInteger)v3 : (int)v3
					));
				}

			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}

			return Quaternion.identity;
		}

		public void SetValue(IValue newValue)
		{
			//value = newValue.GetValue();
		}
	}
}