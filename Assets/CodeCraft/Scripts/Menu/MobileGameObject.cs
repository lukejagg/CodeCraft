using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

public class MobileGameObject : MonoBehaviour
{
	public GameObject[] mobileGameObjects;

    void Awake()
    {
#if UNITY_IOS
		foreach (GameObject obj in mobileGameObjects)
	    {
			obj.SetActive(true);
	    }
#elif UNITY_WEBGL
	    if (ObjectEdit.IsMobile())
	    {
		    foreach (GameObject obj in mobileGameObjects)
		    {
			    obj.SetActive(true);
			}
		}
#else
		foreach (GameObject obj in mobileGameObjects)
	    {
			obj.SetActive(false);
	    }
#endif
	}
}