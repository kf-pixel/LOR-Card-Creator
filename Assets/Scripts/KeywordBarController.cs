using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KeywordBarController : MonoBehaviour
{
	[System.Serializable]
	private class GameObjectInt
	{
		public GameObject obj;
		public int index;

		public GameObjectInt()
		{
			obj = null;
			index = 0;
		}

		public GameObjectInt(GameObject o, int i)
		{
			obj = o;
			index = i;
		}
	}

	[SerializeField] private bool autoShorten;
	[SerializeField] private List<GameObjectInt> keywordInstances = new List<GameObjectInt>();
	public GameObjectVariableList keywords;
	private int maxNumberOfKeywords = 5;
	[SerializeField] private IntVariable numberOfKeywords;
	[SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;

	[Header("Events")]
	[SerializeField] private UnityEvent noKeywordsEvent;
	[SerializeField] private UnityEvent someKeywordsEvent;
	[SerializeField] private UnityEvent maxKeywordsEvent;
	[SerializeField] private IntEvent keywordIntAddEvent, keywordIntRemoveEvent;

	private void OnEnable()
	{
		CheckNumberOfKeywords();
	}

	private void OnDisable()
	{
		foreach (GameObjectInt go in keywordInstances)
		{
			Destroy(go.obj);
		}
		keywordInstances.Clear();
	}

	public void AddKeyword(int index)
	{
		if (index > keywords.value.Count - 1 || keywordInstances.Count >= maxNumberOfKeywords) return;

		GameObject newKeyword = Instantiate(keywords.value[index], this.transform);
		keywordInstances.Add(new GameObjectInt(newKeyword, index));

		CheckNumberOfKeywords();
		FormatKeywords();
	}

	public void RemoveKeyword(int indexRemove)
	{
		for (int i = keywordInstances.Count - 1; i >= 0; i--)
		{
			if (keywordInstances[i].index == indexRemove)
			{
				Destroy(keywordInstances[i].obj);
				keywordInstances.Remove(keywordInstances[i]);

				if (keywordInstances.Count == 1)
				{
					int ind = keywordInstances[0].index;
					ClearKeywords();
					AddKeyword(ind);
					return;
				}
			}
		}
		CheckNumberOfKeywords();
		FormatKeywords();
	}
	public void AddKeyword(string keywordName)
	{
		int i = 0;
		foreach (GameObject k in keywords.value)
		{
			if (k.name == keywordName)
			{
				keywordIntAddEvent.Raise(i);
				return;
			}
			i++;
		}
	}

	public void RemoveKeyword(string keywordName)
	{
		int i = 0;
		foreach (GameObject k in keywords.value)
		{
			if (k.name == keywordName)
			{
				keywordIntRemoveEvent.Raise(i);
				return;
			}
			i++;
		}
	}

	public void RemoveCardTypeKeywords()
	{
		string[] cardTypeNames = new string[6] { "Burst", "Fast", "Slow", "Skill", "Focus", "Landmark" };

		foreach (string cn in cardTypeNames)
		{
			RemoveKeyword(cn);
		}
	}

	public void ClearKeywords()
	{
		foreach (GameObjectInt go in keywordInstances)
		{
			Destroy(go.obj);
		}
		keywordInstances.Clear();
		CheckNumberOfKeywords();
	}

	private void CheckNumberOfKeywords()
	{
		if (keywordInstances.Count >= 5)
		{
			maxKeywordsEvent.Invoke();
		}
		else if (keywordInstances.Count > 0)
		{
			someKeywordsEvent.Invoke();
		}
		else
		{
			noKeywordsEvent.Invoke();
		}
		numberOfKeywords.value = keywordInstances.Count;

	}

	public void FormatKeywords()
	{
		int i = keywordInstances.Count;

		// Rearrange spell speed & skill icons to the left
		for (int inx = keywordInstances.Count - 1; inx >= 0; inx--)
		{
			if (keywordInstances[inx].obj.name.StartsWith("Burst") || keywordInstances[inx].obj.name.StartsWith("Fast") || keywordInstances[inx].obj.name.StartsWith("Slow") 
			|| keywordInstances[inx].obj.name.StartsWith("Skill") || keywordInstances[inx].obj.name.StartsWith("Focus"))
			{
				GameObjectInt spellObject = keywordInstances[inx];
				keywordInstances.Remove(spellObject);
				keywordInstances.Add(spellObject);
				spellObject.obj.transform.SetAsLastSibling();
			}
		}

		// Check total lengths of full sized keywords
		float totalWidth = 0f;
		foreach (GameObjectInt kwi in keywordInstances)
		{
			totalWidth += kwi.obj.GetComponent<KeywordFormat>().GetWidth() + 20f;
		}

		// Format based on total length
		if (totalWidth > 300f || (autoShorten && keywordInstances.Count > 1))
		{
			foreach (GameObjectInt kwi in keywordInstances)
			{
				kwi.obj.GetComponent<KeywordFormat>().SetToShort();
			}
		}
		else foreach (GameObjectInt kwi in keywordInstances)
		{
			kwi.obj.GetComponent<KeywordFormat>().SetToFull();
		}

		// adjust spacing
		horizontalLayoutGroup.spacing = keywordInstances.Count > 4 ? 5f : 20f;
	}
}