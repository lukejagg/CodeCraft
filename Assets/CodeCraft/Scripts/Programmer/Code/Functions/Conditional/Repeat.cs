using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class Repeat : ICode
	{
		IValue condition;
		Program scope1;

		public Repeat(IValue condition, Program scope1)
		{
			this.condition = condition;
			this.scope1 = scope1;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			var val = condition?.GetValue(obj);
			if (val is float f)
			{
				for (int i = 0; i < f; i++)
				{
					var r = await scope1?.Execute(obj);
					
					switch (r)
					{
						case ReturnCode.Break: return ReturnCode.None;
						case ReturnCode.Return: return ReturnCode.Return;
					}
				}
			}
			else if (val is BigInteger b)
			{
				for (int i = 0; i < b; i++)
				{
					var r = await scope1?.Execute(obj);

					switch (r)
					{
						case ReturnCode.Break: return ReturnCode.None;
						case ReturnCode.Return: return ReturnCode.Return;
					}
				}
			}
			else if (val is int d)
			{
				for (int i = 0; i < d; i++)
				{
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