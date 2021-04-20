using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectEditor
{
    public class MeshObject : MonoBehaviour, IObject
    {
	    public float lastCloneTime { get; set; }
        public int cloneCount { get; set; }

        public int Index { get; set; }
        public ObjectType Id => ObjectType.Mesh;

        public enum MeshShape : int
        {
            Block = 0,
            Wedge = 1,
            Sphere = 2,
            Cylinder = 3,
            Square = 4,
        }

        public Transform builtPreset;

        public MeshFilter objectFilter;
        public MeshRenderer objectRenderer;
        public Collider objectCollider;

        public MeshShape meshShape;
        public Color color = new Color(0.8f, 0.8f, 0.8f, 1);
        public Color emission = Color.black;

        public Transform ConvertToObject()
        {
            builtPreset = ObjectStorage.Instance.loadMeshObjects[(int)meshShape];
            var clone = Instantiate(builtPreset);
            //clone.GetComponent<MeshRenderer>().material.color = color;
            ObjectUtil.SetColor(clone, color);
            return clone;
        }

        public string Serialize()
        {
            string str = $"{(int)meshShape}" + Saving.PropertySplit;
            str += $"{Saving.SaveFloat(color.r)},{Saving.SaveFloat(color.g)},{Saving.SaveFloat(color.b)},{Saving.SaveFloat(color.a)}" + Saving.PropertySplit;
            str += $"{Saving.SaveFloat(emission.r)},{Saving.SaveFloat(emission.g)},{Saving.SaveFloat(emission.b)}" + Saving.PropertySplit;
            return str;
        }

        public void Deserialize(string[] properties)
        {
            meshShape = (MeshShape)int.Parse(properties[0]);
            var color = properties[1].Split(',');
            this.color = new Color(float.Parse(color[0]), float.Parse(color[1]), float.Parse(color[2]));
            // Todo: add transparency loading, transparency shaders?
            var emission = properties[1].Split(',');
            this.emission = new Color(float.Parse(emission[0]), float.Parse(emission[1]), float.Parse(emission[2]));
        }

        void AddCollider()
        {
	        switch (meshShape)
	        {
		        case MeshShape.Block:
			        gameObject.AddComponent<BoxCollider>();
			        break;
		        case MeshShape.Cylinder:
		        case MeshShape.Square:
		        case MeshShape.Wedge:
			        gameObject.AddComponent<MeshCollider>().convex = true;
			        break;
		        case MeshShape.Sphere:
			        gameObject.AddComponent<SphereCollider>();
			        break;
	        }
        }

        void UpdateCollider<T>()
        {
	        objectCollider = GetComponent<Collider>();

	        if (objectCollider == null)
	        {
                AddCollider();
	        }
            else if (objectCollider.GetType() != typeof(T))
            {
                Destroy(objectCollider);
                AddCollider();
            }
        }

        public void UpdateProperties()
        {
            objectFilter.mesh = ObjectStorage.Instance.objectMeshes[(int)meshShape];
            objectRenderer.material.color = color;
            switch(meshShape)
            {
                case MeshShape.Block:
                    UpdateCollider<BoxCollider>();
                    break;
                case MeshShape.Cylinder:
                case MeshShape.Square:
                case MeshShape.Wedge:
                    UpdateCollider<MeshCollider>();
                    break;
                case MeshShape.Sphere:
                    UpdateCollider<SphereCollider>();
                    break;
            }
        }
    }
}