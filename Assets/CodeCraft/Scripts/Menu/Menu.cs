using System.Collections;
using System.Collections.Generic;
using System.IO;
using ObjectEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
	void Awake()
	{
		Saving.Initialize();
	}

    // Start is called before the first frame update
    void Start()
    {
        // Open Tutorial menu when coming back from a tutorial
	    if (SceneData.IsTutorial)
		    OpenMenu(1);

	    if (!PlayerPrefs.HasKey("WentThroughTutorial"))
	    {
		    LoadTutorial();
	    }
    }





	// Tutorial
	public Image[] tutorialImages;
    private int currentTutorialIndex = 0;
    public Button prevTutorialButton;

	void ReloadTutorial()
    {
		prevTutorialButton.gameObject.SetActive(currentTutorialIndex > 0);

	    if (currentTutorialIndex < 0) currentTutorialIndex = 0;
	    if (currentTutorialIndex >= tutorialImages.Length)
	    {
		    PlayerPrefs.SetInt("WentThroughTutorial", 1);
			OpenMenu(0);
	    }

	    for (int i = 0; i < tutorialImages.Length; i++)
	    {
			tutorialImages[i].gameObject.SetActive(i == currentTutorialIndex);
	    }
    }

    public void NextTutorial()
    {
	    currentTutorialIndex++;
		ReloadTutorial();
    }

    public void PreviousTutorial()
    {
	    currentTutorialIndex--;
	    ReloadTutorial();
    }

    public void LoadTutorial()
    {
	    currentTutorialIndex = 0;
		ReloadTutorial();
	    OpenMenu(6);
    }







    public GameObject[] Menus;

    public void OpenMenu(int id)
    {
	    CanvasScale.Instance.UpdateScale(id == 0);

	    for (int i = 0; i < Menus.Length; i++)
	    {
		    Menus[i].SetActive(i == id);
	    }
    }

    public void LoadTutorial(string data)
    {
	    var split = data.IndexOf("/");
	    name = Saving.FindUnusedName(Saving.FixName(data.Substring(5, split - 5)));
	    data = data.Substring(split + 1);

		SceneData.Name = name;
	    SceneData.Data = data;
		SceneData.IsTutorial = true;
		SceneData.AllowAutosave = false;
		SceneManager.LoadScene("Editor");
    }

    public InputField nameField;
    public void CreateNew()
    {
        // Load Default Save
        SceneData.Name = Saving.FindUnusedName(Saving.FixName(nameField.text));
        SceneData.Data = "H4sIAAAAAAAACzN4v3+/c2lSKpAy0NE11DHTMwUzQdAQyDLUMQTT7xfvNQBJ6JmbmpgbmutgMgzxSoON2M/LZQSyLzE3tSgRyDDS0QXJ65joGeG01cxAzwAIwJpBQi6ZRanJJZn5eYk5Cj6Z6RklYI1GeiaWIAByP0g51DEG5oYGZhZgY+E8VOORbYNoArnEEORXM7ClAN5vNkQhAQAA";
        SceneData.IsTutorial = false;
        SceneData.AllowAutosave = true;
        SceneManager.LoadScene("Editor");
    }

    public void FixName()
    {
	    nameField.text = Saving.FindUnusedName(Saving.FixName(nameField.text));
    }

    public InputField loadField;

    public void AddProject()
    {
	    var text = loadField.text;

	    var urlIndex = text.LastIndexOf("\nhttp://");
	    if (urlIndex != -1)
		    text = text.Substring(0, urlIndex);

	    var nameIndex = text.IndexOf("Name:");
	    if (nameIndex != -1)
		    text = text.Substring(nameIndex);
			    
	    SceneData.IsTutorial = false;
	    var name = "";
	    var data = "";

	    if (text.StartsWith("Name:"))
	    {
		    var split = text.IndexOf("/");
		    name = Saving.FindUnusedName(Saving.FixName(text.Substring(5, split - 5)));
		    data = text.Substring(split + 1);
	    }
	    else
	    {
		    name = Saving.FindUnusedName("Unnamed");
		    data = text;
	    }

	    var path = Saving.GetSavePath(name);
	    //File.WriteAllText(path, data);
		Saving.WriteSave(path, data);

		GetSaves.Instance.CreateButtons();

        Debug.Log($"Added project {name} with {data.Length} data");
        StartCoroutine(AddedProject(name));
    }

    IEnumerator AddedProject(string name)
    {
	    var text = $"Added {name}";
	    loadField.text = text;
	    yield return new WaitForSeconds(2);
        // If it didn't change, add it
        if (loadField.text == text)
			loadField.text = "";
    }
}
