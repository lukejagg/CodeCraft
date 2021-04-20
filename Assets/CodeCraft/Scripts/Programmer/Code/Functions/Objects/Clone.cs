using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class Clone : ICode
	{
		private IValue parameter;

		public Clone(IValue parameter)
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

				clone.Variables = new Dictionary<string, object>(original.Variables.Count);
				foreach (var keyValue in original.Variables)
				{
					if (clone.Variables.ContainsKey(keyValue.Key))
					{
						clone.Variables[keyValue.Key] = keyValue.Value;
					}
					else
					{
						clone.Variables.Add(keyValue.Key, keyValue.Value);
					}
				}

				original.lastClone = clone;
				clone.lastClone = original;

				clone.isClone = true;

				clone.startProgram = original.startProgram;
				clone.updateProgram = original.updateProgram;
				clone.fixedUpdateProgram = original.fixedUpdateProgram;

				clone.collisionEnterPrograms = original.collisionEnterPrograms;
				clone.collisionEndPrograms = original.collisionEndPrograms;

				clone.deletedPrograms = original.deletedPrograms;

				clone.broadcastPrograms = original.broadcastPrograms;

				// Todo: Add programs to SceneExecutor
				foreach (var bps in clone.broadcastPrograms)
					SceneExecutor.Instance.AddBroadcast(bps.Key, new BroadcastProgram(clone, bps.Value));
				
				if (clone.updateProgram != null && clone.updateProgram.Length > 0)
					SceneExecutor.Instance.UpdateCodeExecutors.Add(clone);

				if (clone.fixedUpdateProgram != null && clone.fixedUpdateProgram.Length > 0)
					SceneExecutor.Instance.FixedUpdateCodeExecutors.Add(clone);

				//clone.isClone = true;
				clone.RunStart();
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