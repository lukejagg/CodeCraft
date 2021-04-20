using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectEditor
{
    public class LightObject : MonoBehaviour, IObject
    {
	    public float lastCloneTime { get; set; }
	    public int cloneCount { get; set; }

	    public int Index { get; set; }
        public ObjectType Id => ObjectType.Light;

        public Transform builtPreset;

        public Light lightComponent;

        public LightType lightType;
        public Color color = Color.white;
        public float brightness = 1;
        public float shadowStrength = 0.5f;

        public float range = 10;
        public float fov = 60;

        public Transform ConvertToObject()
        {
            var clone = Instantiate(builtPreset);

            var light = clone.GetComponent<Light>();
            light.type = lightType;
            light.color = color;
            light.intensity = brightness;
            light.shadowStrength = shadowStrength;

            light.range = range;
            light.spotAngle = fov;

            ObjectUtil.SetColor(clone, color);

            return clone;
        }

        public string Serialize()
        {
            string str = ((int)lightType).ToString() + Saving.PropertySplit;
            str += $"{Saving.SaveFloat(color.r)},{Saving.SaveFloat(color.g)},{Saving.SaveFloat(color.b)}" + Saving.PropertySplit;
            str += Saving.SaveFloat(brightness) + Saving.PropertySplit;
            str += Saving.SaveFloat(shadowStrength) + Saving.PropertySplit;
            str += Saving.SaveFloat(range) + Saving.PropertySplit;
            str += Saving.SaveFloat(fov);
            return str;
        }

        public void UpdateProperties()
        {
            lightComponent.type = lightType;
            lightComponent.color = color;
            lightComponent.intensity = brightness;
            lightComponent.shadowStrength = shadowStrength;
            lightComponent.range = range;
            lightComponent.spotAngle = fov;

            if (transform.childCount > 1)
            {
                foreach (Transform t in transform)
                {
                    Destroy(t.gameObject);
                }
            }
            else if (transform.childCount > 0)
            {
                var t = transform.GetChild(0);
                string newName = lightType.ToString();
                if (t.name != newName)
                {
                    Destroy(t.gameObject);

                    var view = Instantiate(ObjectStorage.Instance.lightObjects[(int)lightType], transform);
                    view.localPosition = Vector3.zero;
                    view.localRotation = Quaternion.identity;
                    view.name = newName;
                }
            }
            else
            {
                string newName = lightType.ToString();
                var view = Instantiate(ObjectStorage.Instance.lightObjects[(int)lightType], transform);
                view.localPosition = Vector3.zero;
                view.localRotation = Quaternion.identity;
                view.name = newName;
            }
        }

        public void Deserialize(string[] properties)
        {
            lightType = (LightType)int.Parse(properties[0]);
            var color = properties[1].Split(',');
            this.color = new Color(float.Parse(color[0]), float.Parse(color[1]), float.Parse(color[2]));
            brightness = float.Parse(properties[2]);
            shadowStrength = float.Parse(properties[3]);
            range = float.Parse(properties[4]);
            fov = float.Parse(properties[5]);

            UpdateProperties();
        }
    }
}