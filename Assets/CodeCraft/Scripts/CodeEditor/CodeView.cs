using System.Collections;
using System.Collections.Generic;
using CodeEditor;
using Interpreter;
using UnityEngine;
using UnityEngine.UI;

public class CodeView : MonoBehaviour
{
	public static CodeView Instance { get; set; }
	public RectTransform rectTransform;

	public Editor editor;

	public Image[] buttons;
	public GameObject elementObjects, move, physics, conditionals, variables, values, lists, objects, apperances, general;

	// Variable Objects
	public Transform customVariableStorage;
	public Transform storedObjects;
	public HierarchyObject variableObject;

	public RectTransform hierarchyScrollContent;

	void EnableCodeHierarchy(GameObject t)
	{
		t.SetActive(true);

		var max = 0f;
		foreach (RectTransform rect in t.GetComponentsInChildren<RectTransform>())
		{
			if (rect.anchoredPosition.y < max)
			{
				max = rect.anchoredPosition.y;
			}
		}

		hierarchyScrollContent.sizeDelta = new Vector2(hierarchyScrollContent.sizeDelta.x, 1000 + Mathf.Abs(max));
	}

	void SetButtonTheme(int id)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			var b = buttons[i];
			var t = b.GetComponentInChildren<Text>();

			if (i == id)
			{
				// 228, 68, 87
				b.color = new Color(71f / 255, 100f / 255, 222f / 255, 1);
				t.color = Color.white;
			}
			else
			{
				b.color = Color.white;
				t.color = Color.black;
			}
		}

		(move.transform.parent as RectTransform).anchoredPosition = Vector3.zero;


		foreach (Transform t in variables.transform.parent)
		{
			t.gameObject.SetActive(false); ;
		}
	}



	public void OpenEvents()
	{
		SetButtonTheme(0);

		EnableCodeHierarchy(elementObjects);
	}

	public void OpenMove()
	{
		SetButtonTheme(1);

		move.SetActive(true);
	}

	public void OpenPhysics()
	{
		SetButtonTheme(2);

		EnableCodeHierarchy(physics);
	}

	public void OpenConditionals()
	{
		SetButtonTheme(3);

		EnableCodeHierarchy(conditionals);
	}

	public void OpenValues()
	{
		SetButtonTheme(5);

		EnableCodeHierarchy(values);
	}

	public void OpenLists()
	{
		SetButtonTheme(6);

		EnableCodeHierarchy(lists);
	}

	public void OpenObjects()
	{
		SetButtonTheme(7);

		EnableCodeHierarchy(objects);
	}

	public void OpenAppearance()
	{
		SetButtonTheme(8);

		EnableCodeHierarchy(apperances);
	}

	public void OpenGeneral()
	{
		SetButtonTheme(9);

		EnableCodeHierarchy(general);
	}

	public void OpenVariables()
	{
		SetButtonTheme(4);

		variables.SetActive(true);

		foreach (Transform t in storedObjects)
		{
			Destroy(t.gameObject);
		}

		foreach (Transform t in customVariableStorage)
		{
			Destroy(t.gameObject);
		}

		var y = 0;
		ICompilerEvent nextEvent = editor.beginEvent;
		while (nextEvent != null)
		{
			if (nextEvent is B_VariableEvent var)
			{
				var block = Instantiate(variableObject, customVariableStorage);
				block.Description = "";
				block.GetComponent<RectTransform>().anchoredPosition += Vector2.up * y;
				y -= 88;

				var b = (RectTransform) block.transform.Find("Block");
				var field = b.Find("InputField").GetComponent<InputField>();
				field.text = var.variableName.text;
				field.name = var.variableName.text;

				var blockBlock = Instantiate(block.codePrefab, storedObjects);
				var bb = (RectTransform) blockBlock.Find("Block");
				var inputField = bb.Find("InputField").GetComponent<InputField>();
				inputField.text = field.name;
				block.codePrefab = blockBlock;

				// Set Size
				var prefWidth = Mathf.Max(inputField.preferredWidth + 28, 60);

				var rt = (RectTransform)inputField.transform;
				rt.sizeDelta = new Vector2(prefWidth, 60);
				bb.sizeDelta = new Vector2(prefWidth + 16, 76);

				rt = (RectTransform)field.transform;
				rt.sizeDelta = new Vector2(prefWidth, 60);
				b.sizeDelta = new Vector2(prefWidth + 16, 76);

				field.onEndEdit.AddListener(delegate
				{
					foreach (var c in GetComponentsInChildren<B_VariableEvent>())
					{
						if (c.variableName.text == field.name)
						{
							c.variableName.text = field.text;
						}
					}

					foreach (var c in GetComponentsInChildren<B_Variable>())
					{
						if (c.variableName.text == field.name)
						{
							c.variableName.text = field.text;
						}
					}

					field.name = field.text;
					editor.ReconnectAll();

					OpenVariables();
				});
			}
			nextEvent = nextEvent.NextConnection;
		}

		// Scale Scroll View
		EnableCodeHierarchy(variables);
	}

	public Scrollbar scaleBar;
	public void Rescale()
	{
		rectTransform.localScale = Vector3.one * (Mathf.Round(scaleBar.value * 10) / 10 + 0.5f);
	}
}
