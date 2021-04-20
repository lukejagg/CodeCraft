using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;

namespace Programmer
{
	public class Broadcast : ICode
	{
		private string broadcastName;

		public Broadcast(string broadcastName)
		{
			this.broadcastName = broadcastName;
		}

		public async Task<ReturnCode> Run(CodeExecutor obj)
		{
			SceneExecutor.Instance.Broadcast(broadcastName);
			return ReturnCode.None;
		}
	}
}