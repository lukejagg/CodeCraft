using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectEditor
{
    public class CameraObject : MonoBehaviour, IObject
    {
        public float lastCloneTime { get; set; }
        public int cloneCount { get; set; }

        public int Index { get; set; }
        public ObjectType Id => ObjectType.Camera;

        public Transform builtPreset;

        public float fov = 60;

        public Transform ConvertToObject()
        {
            var clone = Instantiate(builtPreset);

            var cam = clone.GetComponent<Camera>();
            cam.fieldOfView = fov;

            //ObjectUtil.SetColor(clone, );

            return clone;
        }

        public string Serialize()
        {
            return $"{fov.ToString("0.0000")}";
        }

        public void Deserialize(string[] properties)
        {

        }

        public void UpdateProperties()
        {
            
        }
    }
}