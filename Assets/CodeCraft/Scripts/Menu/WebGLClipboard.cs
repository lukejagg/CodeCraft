using UnityEngine;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

public class WebGLClipboardAPI
{

#if UNITY_WEBGL

    [DllImport("__Internal")]
    private static extern void initWebGLCopyAndPaste(string objectName, string cutCopyCallbackFuncName, string pasteCallbackFuncName);
    [DllImport("__Internal")]
    private static extern void passCopyToBrowser(string str);

#endif

    static public void Init(string objectName, string cutCopyCallbackFuncName, string pasteCallbackFuncName)
    {
#if UNITY_WEBGL

        initWebGLCopyAndPaste(objectName, cutCopyCallbackFuncName, pasteCallbackFuncName);

#endif
    }

    static public void PassCopyToBrowser(string str)
    {
#if UNITY_WEBGL

        passCopyToBrowser(str);

#endif
    }
}

public class WebGLClipboard : MonoBehaviour
{
	public EventSystem eventSystem;

#if UNITY_WEBGL
    void Start()
    {
        if (!Application.isEditor)
        {
            WebGLClipboardAPI.Init(this.name, "GetClipboard", "ReceivePaste");
        }
    }

    private void SendKey(string baseKey)
    {
        string appleKey = "%" + baseKey;
        string naturalKey = "^" + baseKey;

        var currentObj = eventSystem.currentSelectedGameObject;
        if (currentObj == null)
        {
            return;
        }
        {
            var input = currentObj.GetComponent<UnityEngine.UI.InputField>();
            if (input != null)
            {
                input.ProcessEvent(Event.KeyboardEvent(naturalKey));
                input.ProcessEvent(Event.KeyboardEvent(appleKey));
            }
        }
    }

    public void GetClipboard(string key)
    {
        SendKey(key);
        WebGLClipboardAPI.PassCopyToBrowser(GUIUtility.systemCopyBuffer);
    }

    public void ReceivePaste(string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }
#endif
}