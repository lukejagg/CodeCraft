using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class Remainder : IValue
	{
		public bool ReadOnly => true;

		IValue value1 = null;
		IValue value2 = null;

		public Remainder(IValue value1, IValue value2)
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
					return j == 0 ? 0 : i % j;
				if (v2 is BigInteger bj)
					return bj == 0 ? 0 : i % bj;
				if (v2 is float f)
					return f == 0 ? 0 : i % f;
			}
			else if (v1 is float fi)
			{
				if (v2 is int j)
					return j == 0 ? 0 : fi % j;
				if (v2 is BigInteger bj)
					return bj == 0 ? 0 : fi % (float)bj;
				if (v2 is float f)
					return f == 0 ? 0 : fi % f;
			}
			else if (v1 is BigInteger bi)
			{
				if (v2 is int j)
					return j == 0 ? 0 : bi % j;
				if (v2 is BigInteger bj)
					return bj == 0 ? 0 : bi % bj;
				if (v2 is float f)
					return f == 0 ? 0 : (float)bi % f;
			}

			return 0;
		}
	}
}