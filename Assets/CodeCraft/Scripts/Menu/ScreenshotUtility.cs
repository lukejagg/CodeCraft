using UnityEngine;
using System.Collections;
using Screen = UnityEngine.Screen;

public class ScreenshotUtility : MonoBehaviour
{
#if UNITY_EDITOR
	public bool takeScreenshot = false;
	public int id = 0;

	void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

    void Update()
    {
	    if (takeScreenshot)
        {
	        var name = $"z_{SystemInfo.deviceModel}_{id++}.png".Replace(' ', '-');
	        ScreenCapture.CaptureScreenshot(name);
	        takeScreenshot = false;
        }
    }
#endif
}