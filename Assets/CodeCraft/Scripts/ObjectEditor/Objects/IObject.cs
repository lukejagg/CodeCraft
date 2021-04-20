using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectEditor
{
    public interface IObject
    {
        float lastCloneTime { get; set; }
        int cloneCount { get; set; }

        int Index { get; set; }
        ObjectType Id { get; }
        Transform ConvertToObject();
        string Serialize();
        void Deserialize(string[] properties);
        void UpdateProperties();
    }

    public static class ObjectUtil
    {
	    public static void SetColor(Transform obj, Color c)
	    {
		    if (obj.TryGetComponent<CodeExecutor>(out var code))
		    {
			    code.transparency = 1 - c.a;
                code.color = new Vector3(c.r, c.g, c.b);
                code.RefreshColor();
		    }
	    }
    }
}