using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

public class GetSaves : MonoBehaviour
{
	public static GetSaves Instance;

	public Transform loadParent;
	public Load loadPrefab;
	public DeleteConfirmation deletePrefab;

	private int creationId = 0;

	async void CreateAsync()
	{
		creationId++;
		int id = creationId;

		foreach (Transform t in loadParent)
		{
			Destroy(t.gameObject);
		}

#if UNITY_WEBGL
		var files = Saving.SaveFolders[Saving.SavePath];
		Debug.Log($"Found {files.Count} saves");

		foreach (var file in files)
		{
			if (id != creationId) break;

			await Task.Delay(10);

			var name = Path.GetFileNameWithoutExtension(file);
			var load = Instantiate(loadPrefab, loadParent);
			load.Name = name;
			load.transform.name = name;
			load.Initialize();

			Debug.Log($"Found file {name}");
		}
#else
		var files = Directory.GetFiles(Saving.SavePath);
		Debug.Log($"Found {files.Length} saves");

		foreach (var file in files)
		{
			if (id != creationId) break;

			await Task.Delay(10);

			var name = Path.GetFileNameWithoutExtension(file);
			var load = Instantiate(loadPrefab, loadParent);
			load.Name = name;
			load.transform.name = name;
			load.Initialize();

			Debug.Log($"Found file {name}");
		}
#endif
	}

	public void CreateButtons()
	{
		CreateAsync();
	}

	void Start()
	{
		Instance = this;
		CreateButtons();
	}
}
