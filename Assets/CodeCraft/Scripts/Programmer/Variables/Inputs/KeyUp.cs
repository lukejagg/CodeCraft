using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	// Todo: Implement input values (Primitives)
	public class KeyUp : IValue
	{
		public bool ReadOnly => true;

		private PlayInputCode playInputCode;

		public KeyUp(PlayInputCode keyCode)
		{
			playInputCode = keyCode;
		}

		public object GetValue(CodeExecutor obj)
		{
			switch (playInputCode)
			{
				case PlayInputCode.Primary:
					return !PlayInputs.LeftClick && PlayInputs.PrevLeftClick;
				case PlayInputCode.Secondary:
					return !PlayInputs.RightClick && PlayInputs.PrevRightClick;
				case PlayInputCode.Jump:
					return !PlayInputs.JumpClick && PlayInputs.PrevJumpClick;
				case PlayInputCode.Action:
					return !PlayInputs.ActionClick && PlayInputs.PrevActionClick;
			}

			return false;
		}
	}
}