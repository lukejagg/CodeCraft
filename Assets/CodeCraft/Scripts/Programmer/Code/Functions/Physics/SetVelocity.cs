using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
	public class SetVelocity : ICode
	{
		private bool enableRigidbody;

		private IValue value;

		public SetVelocity(IValue value)
		{
			this.value = value;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			if (obj.rigidbody != null)
			{
				if (value != null)
				{
					var val = value.GetValue(obj);

					if (val is Vector3 i) obj.rigidbody.velocity = i;
					else if (val is Vector3Int f) obj.rigidbody.velocity = f;
				}
				else
				{
					obj.rigidbody.velocity = Vector3.zero;
				}
			}

			return ReturnCode.None;
		}
	}
}