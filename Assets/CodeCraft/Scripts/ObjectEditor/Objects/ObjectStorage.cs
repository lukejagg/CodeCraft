using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectEditor
{
    public class ObjectStorage : MonoBehaviour
    {
        public static ObjectStorage Instance { get; private set; }

        public Saving saving;
        public ObjectEdit objectEdit;

        public Mesh[] objectMeshes;
        public Transform[] loadMeshObjects;

        public Transform[] lightObjects;

        void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
	        Instance = this;
            foreach (var comp in GetComponentsInChildren<IObject>())
            {
                comp.UpdateProperties();
            }
        }

        void SetPosition(Transform t, IObject obj)
        {
	        var cam = objectEdit.cam.transform;
	        var pos = objectEdit.RoundVector3(cam.position + cam.forward * 5);
            t.position = pos;

            objectEdit.AddUndo(new ParentAction(t, objectEdit.deleteStorage));

            obj.Index = 0;
            saving.AddObjectIndex(obj);
        }

        public void AddMeshObject(int meshId)
        {
	        var obj = Instantiate(saving.ObjectIdPrefabs[0], saving.objects);
	        var mesh = obj.GetComponent<MeshObject>();
	        mesh.meshShape = (MeshObject.MeshShape)meshId;
            mesh.UpdateProperties();
            SetPosition(obj, mesh);
        }

        public void AddLightObject(int lightId)
        {
	        var obj = Instantiate(saving.ObjectIdPrefabs[1], saving.objects);
	        var mesh = obj.GetComponent<LightObject>();
	        mesh.lightType = (LightType)lightId;
	        mesh.UpdateProperties();
	        SetPosition(obj, mesh);
        }

        public void AddCameraObject()
        {
	        var obj = Instantiate(saving.ObjectIdPrefabs[2], saving.objects);
	        var mesh = obj.GetComponent<CameraObject>();
	        mesh.UpdateProperties();
	        SetPosition(obj, mesh);
        }
    }
}