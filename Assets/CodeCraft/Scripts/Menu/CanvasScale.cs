using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScale : MonoBehaviour
{
	public static CanvasScale Instance;

	private float scale = 0;
	private CanvasScaler canvas;
	public GameObject checkObject, unscaleObject;
	public Vector2 vec = new Vector2(1920, 1080);

	void Awake()
	{
		Instance = this;

		canvas = GetComponent<CanvasScaler>();
	}

	public void UpdateScale(bool useOriginal)
	{
		if (useOriginal)
		{
			canvas.referenceResolution = vec;
		}
		else
		{
			canvas.referenceResolution = vec / Settings.UIScale;
			scale = Settings.UIScale;
		}
	}

	// Update is called once per frame
    void Update()
    {
	    if (scale != Settings.UIScale)
	    {
		    if (checkObject == null)
		    {
			    canvas.referenceResolution = vec / Settings.UIScale;
			    scale = Settings.UIScale;
		    }
		    else
		    {
			    if (!checkObject.activeInHierarchy && !unscaleObject.activeInHierarchy)
			    {
				    canvas.referenceResolution = vec / Settings.UIScale;
				    scale = Settings.UIScale;
			    }
		    }
	    }
    }
}
