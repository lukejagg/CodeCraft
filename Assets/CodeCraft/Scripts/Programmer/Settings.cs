using UnityEngine;
using UnityEngine.UI;

public enum JoystickType
{
    Joystick = 0,
    Move = 1, // Use delta for change in look
}

[System.Flags]
public enum CodeDropType : byte
{
	None = 0b00,
	LeftOnly = 0b01,
	RightOnly = 0b10,
	Both = 0b11,
}

public class Settings : MonoBehaviour
{
	public static JoystickType LookType = JoystickType.Move;
	public static float Sensitivity = 3;
	public static float UIScale = 1;
	public static bool FPS60 = false;
	public static CodeDropType CodeDropType = CodeDropType.Both;

	static int LoadInt(string key, int defaultValue) => PlayerPrefs.GetInt(key, defaultValue);
	static float LoadFloat(string key, float defaultValue) => PlayerPrefs.GetFloat(key, defaultValue);

	void Start()
	{
		LookType = (JoystickType)LoadInt("LookType", (int)JoystickType.Move);
		Sensitivity = LoadFloat("Sensitivity", 3);
		UIScale = LoadFloat("UIScale", 1);
		FPS60 = LoadInt("60FPS", 0) != 0;
		CodeDropType = (CodeDropType)LoadInt("CodeDrop", 3);

		UpdateGUI();
	}

	void UpdateGUI()
	{
		lookTypeToggle.isOn = LookType == JoystickType.Joystick;
		sensitivitySlider.value = Sensitivity;
		uiScaleSlider.value = UIScale;
		fpsToggle.isOn = FPS60;
		codeDropTypeDropdown.value = (int)CodeDropType;

#if UNITY_IOS
		Application.targetFrameRate = FPS60 ? 60 : 30;
#endif
	}

	public Toggle lookTypeToggle;
	public Slider sensitivitySlider;
	public Slider uiScaleSlider;
	public Toggle fpsToggle;
	public Dropdown codeDropTypeDropdown;

	public void UpdateLookType()
	{
		LookType = lookTypeToggle.isOn ? JoystickType.Joystick : JoystickType.Move;
		PlayerPrefs.SetInt("LookType", (int)LookType);
		UpdateGUI();
	}

	public void UpdateSensitivity()
	{
		Sensitivity = sensitivitySlider.value;
		PlayerPrefs.SetFloat("Sensitivity", Sensitivity);
		UpdateGUI();
	}

	public void UpdateUIScale()
	{
		UIScale = uiScaleSlider.value;
		PlayerPrefs.SetFloat("UIScale", UIScale);
		UpdateGUI();
	}

	public void UpdateFPS()
	{
		FPS60 = fpsToggle.isOn;
		PlayerPrefs.SetInt("60FPS", FPS60 ? 1 : 0);
		UpdateGUI();
	}

	public void UpdateCodeDropType()
	{
		CodeDropType = (CodeDropType)codeDropTypeDropdown.value;
		PlayerPrefs.SetInt("CodeDrop", (int)CodeDropType);
		UpdateGUI();
	}
}
