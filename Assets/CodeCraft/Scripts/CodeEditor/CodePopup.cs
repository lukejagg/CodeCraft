using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CodeEditor
{
	public class CodePopup : MonoBehaviour
	{
		public static CodePopup Instance;

		public RectTransform popupPrefab;
		public RectTransform currentPopup;

		public float popupTime;

		void Start()
		{
			Instance = this;
		}

		public void MakePopup(string desc, Vector2 pos)
		{
			if (currentPopup != null) Destroy(currentPopup.gameObject);

			popupTime = Time.time;

			var popup = Instantiate(popupPrefab, transform);

			var text = popup.GetComponentInChildren<Text>();
			text.text = desc.Replace(@"\n", "\n");
			popup.sizeDelta = new Vector2(520, text.preferredHeight + 20);
			popup.position = pos;
			popup.localScale = Vector3.zero;

			currentPopup = popup;
			
		}

		void Update()
		{
			if (currentPopup != null)
			{
				var scale = Mathf.Clamp01((Time.time - popupTime) * 8);
				currentPopup.localScale = Vector3.one * scale;

				if (Time.time - popupTime > 0.1f)
				{
					if (Input.GetMouseButtonDown(0))
					{
						Destroy(currentPopup.gameObject);
					}
				}
			}
		}
	}
}