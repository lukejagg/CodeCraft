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
	public class SetAngularVelocity : ICode
	{
		private bool enableRigidbody;

		private IValue value;

		public SetAngularVelocity(IValue value)
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

					if (val is Vector3 i) obj.rigidbody.angularVelocity = i;
					else if (val is Vector3Int f) obj.rigidbody.angularVelocity = f;
				}
				else
				{
					obj.rigidbody.angularVelocity = Vector3.zero;
				}
			}

			return ReturnCode.None;
		}
	}
}