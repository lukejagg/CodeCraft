using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public enum PlayInputCode
	{
		Primary = 0,
		Secondary = 1,
		Jump = 2,
		Action = 3,
	}

	// Todo: Implement input values (Primitives)
	public class KeyPressed : IValue
	{
		public bool ReadOnly => true;

		private PlayInputCode playInputCode;

		public KeyPressed(PlayInputCode keyCode)
		{
			playInputCode = keyCode;
		}

		public object GetValue(CodeExecutor obj)
		{
			switch (playInputCode)
			{
				case PlayInputCode.Primary:
					return PlayInputs.LeftClick;
				case PlayInputCode.Secondary:
					return PlayInputs.RightClick;
				case PlayInputCode.Jump:
					return PlayInputs.JumpClick;
				case PlayInputCode.Action:
					return PlayInputs.ActionClick;
			}

			return false;
		}
	}
}