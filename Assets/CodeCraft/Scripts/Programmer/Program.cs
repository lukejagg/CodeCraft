using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Interpreter;
using ObjectEditor;

namespace Programmer
{
    public class Program
    {
        public ICode[] code;

        public Program(ICode[] code)
        {
            this.code = code;
        }

        public async Task<ReturnCode> Execute(CodeExecutor obj)
        {
			// End the script if the game ended
	        if (!SceneExecutor.Instance.Running) return ReturnCode.Return;

	        for (int i = 0; i < code.Length; i++)
	        {
		        var c = code[i];
		        if (c != null)
		        {
#if UNITY_EDITOR
			        try
			        {
				        var r = await c.Run(obj);

				        if (r != ReturnCode.None)
					        return r;
			        }
			        catch (Exception e)
			        {
						Debug.LogError(c);
						Debug.Log(c.GetType());
						Debug.LogError($"Code Exception: {e}");
			        }
#else
					var r = await c.Run(obj);

			        if (r != ReturnCode.None)
				        return r;
#endif
				}
			}

	        return ReturnCode.None;
        }
    }
}