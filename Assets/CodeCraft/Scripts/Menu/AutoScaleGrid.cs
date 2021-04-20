using UnityEngine;
using UnityEngine.UI;

public class AutoScaleGrid : MonoBehaviour
{
	private int childCount = 0;

	void Update()
	{
		if (transform.childCount != childCount)
		{
			float size = 0;
			foreach (RectTransform child in transform)
			{
				var s = child.rect.height - child.anchoredPosition.y;

				if (s > size)
				{
					size = s;
				}
			}

			var rect = (RectTransform) transform;
			rect.sizeDelta = new Vector2(rect.sizeDelta.x, size + 10);
		}

		childCount = transform.childCount;
	}
}
