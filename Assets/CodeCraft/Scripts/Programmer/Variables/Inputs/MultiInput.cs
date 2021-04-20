using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public enum MultiInputCode
	{
		Move = 0,
		Look = 1,
	}

	// Todo: Implement input values (Primitives)
	public class MultiInput : IValue
	{
		public bool ReadOnly => true;

		private MultiInputCode inputCode;

		public MultiInput(MultiInputCode keyCode)
		{
			inputCode = keyCode;
		}

		public object GetValue(CodeExecutor obj)
		{
			switch (inputCode)
			{
				case MultiInputCode.Move:
					return PlayInputs.MoveDirection;
				case MultiInputCode.Look:
					return PlayInputs.CameraMovement;
			}

			return false;
		}
	}
}