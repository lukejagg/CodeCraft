using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Programmer
{
    public class PlayOutput : MonoBehaviour
    {
	    public static PlayOutput Instance;

	    public bool hasChildren = false;
	    public bool open = false;
	    private float height = 0, total = 0;
	    public RectTransform outPrefab;
	    public RectTransform content, view;
	    public Image scroll;

	    void Awake()
	    {
		    Instance = this;

			// Stupid error, VectorValue.cs will error if not done
			try
			{
				var x = (int) new BigInteger(0);
				var y = (float) new BigInteger(0);
				var z = ((float) new BigInteger(0)) + y;
				var w = (float) new BigInteger(0) * 3f;
				var w1 = (float) new BigInteger(0) / 4f;
				var w2 = new BigInteger(23) / 4;

				for (int i = 0; i < (float)w + (float)w1 + (float)w2 + (float)z + (float)x; i++)
				{
					Debug.Log("Fixing numerical operations...");
					break;
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e);
			}
	    }

	    void Start()
	    {
		    Instance = this;
		}

	    public void Output(string str)
	    {
		    try
		    {
			    if (str.Length > 16000)
				    str = str.Substring(0, 16000);

			    if (!hasChildren) scroll.color = new Color(0f, 0f, 0f, 0.2f);
			    hasChildren = true;

			    // Create Text
			    var t = Instantiate(outPrefab);
			    var text = t.GetComponent<Text>();
			    text.text = str;

			    height = text.preferredHeight; // - Mathf.Floor(1.4f * str.Count(s => s == '\n'));
			    total = height;

			    var deleteHeight = false;

			    foreach (RectTransform i in content)
			    {
				    if (total + i.sizeDelta.y > 10000)
					    deleteHeight = true;

				    if (!deleteHeight)
				    {
					    total += i.sizeDelta.y;
					    i.anchoredPosition += Vector2.up * height;
				    }
				    else
				    {
					    DestroyImmediate(i.gameObject);
				    }
			    }

			    // Parent text and set size
			    t.SetParent(content, false);
			    t.localScale = Vector3.one;
			    t.sizeDelta = new Vector2(content.sizeDelta.x, height);
			    t.anchoredPosition = Vector2.zero;
			    t.SetSiblingIndex(0);

			    content.sizeDelta = new Vector2(content.sizeDelta.x, total);
			    if (!open)
				    content.anchoredPosition = new Vector2(0, total - height);
		    }
		    catch (Exception e)
		    {
				Debug.LogError($"ERROR PRINTING IN PLAYOUTPUT\n{e}");
		    }
	    }

	    public void ToggleOpen()
	    {
		    open = !open;

		    if (open && hasChildren)
		    {
				scroll.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);
				view.sizeDelta = new Vector2(view.sizeDelta.x, 500);
		    }
		    else
		    {
			    scroll.color = hasChildren ? new Color(0f, 0f, 0f, 0.2f) : new Color(0,0,0,0);
			    view.sizeDelta = new Vector2(view.sizeDelta.x, 8);
				content.anchoredPosition = new Vector2(0, total - height);
			}
	    }

	    public void Reset()
	    {
		    height = 0;
		    total = 0;
		    open = true;
		    hasChildren = false;
		    ToggleOpen();
		    foreach (Transform obj in content)
		    {
			    Destroy(obj.gameObject);
		    }
		}
	}
}