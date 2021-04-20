using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class SetTimescale : ICode
	{
		private bool enableRigidbody;

		private IValue value;

		public SetTimescale(IValue value)
		{
			this.value = value;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			if (SceneExecutor.Instance.Running && value != null)
			{
				var val = value.GetValue(obj);

				if (val is int i) Time.timeScale = i;
				else if (val is float f) Time.timeScale = f;
				else if (val is BigInteger g) Time.timeScale = (float) g;

				Time.fixedDeltaTime = Time.timeScale / 60f;
			}

			return ReturnCode.None;
		}
	}
}