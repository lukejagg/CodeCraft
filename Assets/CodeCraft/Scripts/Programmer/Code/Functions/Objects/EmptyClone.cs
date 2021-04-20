using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class EmptyClone : ICode
	{
		private IValue parameter;

		public EmptyClone(IValue parameter)
		{
			this.parameter = parameter;
		}

		public async Task CloneObj(CodeExecutor original)
		{
			if (original != null)
			{
				var clone = await SceneExecutor.Instance.DelayedCreateObject(original, original.original);
				if (clone == null) return;

				//clone.transform.parent = original.transform.parent;
				//clone.transform.position = original.transform.position;
				//clone.transform.rotation = original.transform.rotation;
				//clone.transform.localScale = original.transform.localScale;

				clone.Variables = null;

				original.lastClone = clone;
				clone.lastClone = original;
				clone.isClone = true;

				clone.startProgram = null;
				clone.updateProgram = null;
				clone.fixedUpdateProgram = null;

				clone.collisionEnterPrograms = null;
				clone.collisionEndPrograms = null;

				clone.deletedPrograms = null;

				clone.broadcastPrograms = null;

				clone.RefreshColor();
			}
		}

		public async Task<ReturnCode> Run(CodeExecutor obj2)
		{
			if (parameter != null)
			{
				var v = parameter?.GetValue(obj2);
				if (v is CodeExecutor obj)
				{
					await CloneObj(obj);
				}
			}
			else
			{
				await CloneObj(obj2);
			}

			return ReturnCode.None;
		}
	}
}