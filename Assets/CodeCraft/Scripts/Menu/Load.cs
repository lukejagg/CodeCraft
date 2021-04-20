using System;
using System.IO;
using ObjectEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Load : MonoBehaviour
{
	public string Name;
	public Button LoadButton;
	public Button LoadManualButton;
	public Button ShareButton;
	public Button CloneButton;
	public Button DeleteButton;
	public InputField NameText;

	public void LoadData(string data, bool allowAutosave)
	{
		SceneData.Name = Name;
		SceneData.IsTutorial = false;
		SceneData.Data = data;
		SceneData.AllowAutosave = allowAutosave;
		SceneManager.LoadScene("Editor");
	}

	public void Initialize()
	{
		NameText.text = Name;

		LoadManualButton.onClick.AddListener(delegate
		{
			Debug.Log($"Loading save for {Name}");

			var path = Saving.GetSavePath(Name);
			var data = Saving.ReadSave(path); //File.ReadAllText(path));
			LoadData(data, false);
		});

		LoadButton.onClick.AddListener(delegate
		{
			Debug.Log($"Loading autosave for {Name}");
			
			var path = Saving.GetAutoSavePath(Name);
			if (!Saving.SaveExists(path)) path = Saving.GetSavePath(Name);
			var data = Saving.ReadSave(path); //File.ReadAllText(path));
			LoadData(data, true);
		});

		ShareButton.onClick.AddListener(delegate
		{
			Debug.Log($"Exporting save {Name}");
			var export = Saving.Export(Name);

#if UNITY_IOS
			new NativeShare().SetText(export).Share();
#else
			GUIUtility.systemCopyBuffer = export;
#endif
		});

		CloneButton.onClick.AddListener(delegate
		{
			Debug.Log($"Duplicating {Name}");

			var path = Saving.GetSavePath(Name);
			var newPath = Saving.GetSavePath(Saving.FindUnusedName(Name));
			//File.WriteAllText(newPath, File.ReadAllText(path));
			Saving.WriteSave(newPath, Saving.ReadSave(path));

			GetSaves.Instance.CreateButtons();
		});

		NameText.onEndEdit.AddListener(delegate
		{
			try
			{
				Debug.Log($"Attempting to rename to {NameText.text}");

				var newName = Saving.FixName(NameText.text);
				var newPath = Saving.GetSavePath(newName);
				if (!Saving.SaveExists(newPath))
				{
#if UNITY_WEBGL
					PlayerPrefs.SetString(newPath, Saving.ReadSave(Saving.GetSavePath(Name)));
					PlayerPrefs.DeleteKey(Saving.GetSavePath(Name));

					if (Saving.SaveExists(Saving.GetAutoSavePath(Name)))
					{
						PlayerPrefs.SetString(Saving.GetAutoSavePath(newName), Saving.ReadSave(Saving.GetAutoSavePath(Name)));
						PlayerPrefs.DeleteKey(Saving.GetAutoSavePath(Name));
					}

					Name = newName;
#else
					// New name doesn't exist, can rename
					File.Move(Saving.GetSavePath(Name), newPath);
					if (Saving.SaveExists(Saving.GetAutoSavePath(Name))) // If an autosave exists, move it
						File.Move(Saving.GetAutoSavePath(Name), Saving.GetAutoSavePath(newName));

					Name = newName;
#endif
				}
				else
				{
					// Path exists, can't rename
					NameText.text = Name;
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to save: {e}");
			}
		});

		DeleteButton.onClick.AddListener(delegate
		{
			var del = Instantiate(GetSaves.Instance.deletePrefab, transform);
			del.Name = Name;
			del.Initialize();
		});
	}
}