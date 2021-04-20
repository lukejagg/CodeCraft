using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class ListLength : IValue
	{
		public bool ReadOnly => true;

		IValue value1 = null;

		public ListLength(IValue value1)
		{
			this.value1 = value1;
		}

		public object GetValue(CodeExecutor obj)
		{
			var v1 = value1?.GetValue(obj);
			if (v1 is Table tbl)
				return new BigInteger(tbl.Length);
			if (v1 is string str)
				return new BigInteger(str.Length);

			return new BigInteger(0);
		}

		public void SetValue(IValue newValue)
		{

		}
	}
}