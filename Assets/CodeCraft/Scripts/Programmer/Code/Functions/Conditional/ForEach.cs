using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class ForEach : ICode
	{
		private IValue list;
		private IValue index;
		private IValue value;
		private Program scope1;

		public ForEach(IValue list, IValue index, IValue value, Program scope1)
		{
			this.list = list;
			this.index = index;
			this.value = value;
			this.scope1 = scope1;
		}

		//private List<object> keyEnumerator = new List<object>();

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			var possibleList = list?.GetValue(obj);
			if (possibleList is Table tbl)
			{
				var i = index as VariableValue;
				var v = value as VariableValue;

				var hasIndex = i != null;
				var hasValue = v != null;

				// Iterate List
				for (int a = 0; a < tbl.Length; a++)
				{
					if (hasIndex) i.SetValueToObject(a, obj);
					if (hasValue) v.SetValueToObject(tbl.List[a], obj);

					var r = await scope1?.Execute(obj);
					switch (r)
					{
						case ReturnCode.Break: return ReturnCode.None;
						case ReturnCode.Return: return ReturnCode.Return;
					}
				}

				// Iterate Hash
				var keyEnumerator = new List<object>();
				//keyEnumerator.Clear();
				keyEnumerator.AddRange(tbl.Hash.Keys);

				foreach (var key in keyEnumerator)
				{
					if (hasIndex) i.SetValueToObject(key, obj);
					if (hasValue) v.SetValueToObject(tbl.Hash[key], obj);

					var r = await scope1?.Execute(obj);
					switch (r)
					{
						case ReturnCode.Break: return ReturnCode.None;
						case ReturnCode.Return: return ReturnCode.Return;
					}
				}

			}

			else if (possibleList is string str)
			{
				var i = index as VariableValue;
				var v = value as VariableValue;

				var hasIndex = i != null;
				var hasValue = v != null;

				// Iterate List
				for (int a = 0; a < str.Length; a++)
				{
					if (hasIndex) i.SetValueToObject(a, obj);
					if (hasValue) v.SetValueToObject(str[a].ToString(), obj);

					var r = await scope1?.Execute(obj);
					switch (r)
					{
						case ReturnCode.Break: return ReturnCode.None;
						case ReturnCode.Return: return ReturnCode.Return;
					}
				}
			}

			return ReturnCode.None;
		}
	}
}