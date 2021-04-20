using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class TogglePhysics : ICode
	{
		private bool enableRigidbody;

		public TogglePhysics(bool enableRigidbody)
		{
			this.enableRigidbody = enableRigidbody;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			obj.ToggleRigidbody(enableRigidbody);

			return ReturnCode.None;
		}
	}
}