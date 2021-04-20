using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ObjectEditor;
using UnityEngine;
using UnityEngine.UI;

public class DeleteConfirmation : MonoBehaviour
{
	public string Name;
	public Button yes, no;
	public Text nameText;

	public void Initialize()
	{
		nameText.text = $"Delete {Name}?";

		no.onClick.AddListener(delegate
		{
			Destroy(gameObject);
		});

		yes.onClick.AddListener(delegate
		{
#if UNITY_WEBGL
			var path = Saving.GetSavePath(Name);
			Saving.RemoveSaveName(Saving.SavePath, Name);
			PlayerPrefs.DeleteKey(path);
			Destroy(transform.parent.gameObject);
#else
			var path = Saving.GetSavePath(Name);
			var newPath = $"{Saving.DeletePath}{Saving.GetDeletedName(Name)}.ccs";

			File.Move(path, newPath);
			Destroy(transform.parent.gameObject);
#endif
		});
	}
}
