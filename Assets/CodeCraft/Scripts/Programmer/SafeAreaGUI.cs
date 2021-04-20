using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SafeAreaGUI : MonoBehaviour
{
	public RectTransform screenGui;
	public int width = 0;
	public int height = 0;
	public float safeArea = 0;

	void Awake()
	{
		Resize();
	}

	void Start()
	{
		Resize();
	}

	void Resize()
	{
		if (Screen.width != width || Screen.height != height || transform.parent.localScale.x != safeArea)
		{
			screenGui.sizeDelta = new Vector2(Screen.safeArea.width, Screen.safeArea.height) /
			                      transform.parent.localScale.x;
		}

		width = Screen.width;
		height = Screen.height;
		safeArea = transform.parent.localScale.x;
	}

    void Update()
    {
	    Resize();
    }
}
