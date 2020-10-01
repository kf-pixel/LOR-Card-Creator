using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

	[SerializeField] private List<GameObjectInt> keywordObject = new List<GameObjectInt>();
	public GameObject[] keywordsFull;
	public GameObject[] keywordsShort;
	private int maxNumberOfKeywords = 5;
	[SerializeField] private IntVariable numberOfKeywords;

	[Header("Events")]
	[SerializeField] private UnityEvent onEnableEvent;
	[SerializeField] private UnityEvent onDisableEvent;
	[SerializeField] private UnityEvent noKeywordsEvent;
	[SerializeField] private UnityEvent someKeywordsEvent;
	[SerializeField] private UnityEvent maxKeywordsEvent;

	private void OnEnable()
	{
		onEnableEvent.Invoke();
		CheckNumberOfKeywords();
	}

	private void OnDisable()
	{
		onDisableEvent.Invoke();

		foreach (GameObjectInt go in keywordObject)
		{
			Destroy(go.obj);
		}
		keywordObject.Clear();
	}

	public void AddKeyword(int index)
	{
		SolveNewKeyword(index);
		CheckNumberOfKeywords();
		FormatKeywords();
	}

	public void RemoveKeyword(int indexRemove)
	{
		for (int i = keywordObject.Count - 1; i >= 0; i--)
		{
			if (keywordObject[i].index == indexRemove)
			{
				Destroy(keywordObject[i].obj);
				keywordObject.Remove(keywordObject[i]);

				if (keywordObject.Count == 1)
				{
					int ind = keywordObject[0].index;
					ClearKeywords();
					AddKeyword(ind);
					return;
				}
			}
		}
		CheckNumberOfKeywords();
		FormatKeywords();
	}

	public void ClearKeywords()
	{
		foreach (GameObjectInt go in keywordObject)
		{
			Destroy(go.obj);
		}
		keywordObject.Clear();
		CheckNumberOfKeywords();
	}

	private void CheckNumberOfKeywords()
	{
		if (keywordObject.Count >= 5)
		{
			maxKeywordsEvent.Invoke();
		}
		else if (keywordObject.Count > 0)
		{
			someKeywordsEvent.Invoke();
		}
		else
		{
			noKeywordsEvent.Invoke();
		}
		numberOfKeywords.value = keywordObject.Count;

	}

	private void FormatKeywords()
	{
		int i = keywordObject.Count;

		// Rearrange spell speed & skill icons to the left
		for (int inx = keywordObject.Count - 1; inx >= 0; inx--)
		{
			if (keywordObject[inx].index == 2 || keywordObject[inx].index == 10 || keywordObject[inx].index == 20 || keywordObject[inx].index == 21)
			{
				GameObjectInt spellObject = keywordObject[inx];
				keywordObject.Remove(spellObject);
				keywordObject.Insert(0, spellObject);
			}
		}

		// 2 keywords
		if (i == 2)
		{
			keywordObject[0].obj.transform.localPosition = new Vector3(-40, 0);
			keywordObject[1].obj.transform.localPosition = new Vector3(40, 0);
			return;
		}

		// 3 keywords
		if (i == 3)
		{
			keywordObject[0].obj.transform.localPosition = new Vector3(-60, 0);
			keywordObject[1].obj.transform.localPosition = new Vector3(0, 0);
			keywordObject[2].obj.transform.localPosition = new Vector3(60, 0);
			return;
		}

		// 4 keywords
		if (i == 4)
		{
			keywordObject[0].obj.transform.localPosition = new Vector3(-90, 0);
			keywordObject[1].obj.transform.localPosition = new Vector3(-30, 0);
			keywordObject[2].obj.transform.localPosition = new Vector3(30, 0);
			keywordObject[3].obj.transform.localPosition = new Vector3(90, 0);
			return;
		}

		// 5 keywords
		if (i == 5)
		{
			keywordObject[0].obj.transform.localPosition = new Vector3(-110, 0);
			keywordObject[1].obj.transform.localPosition = new Vector3(-55, 0);
			keywordObject[2].obj.transform.localPosition = new Vector3(0, 0);
			keywordObject[3].obj.transform.localPosition = new Vector3(55, 0);
			keywordObject[4].obj.transform.localPosition = new Vector3(110, 0);
			return;
		}
	}

	private void SolveNewKeyword(int index)
	{
		if (index > keywordsFull.Length - 1)
		{
			return;
		}

		// If this is the first keyword
		if (keywordObject.Count == 0)
		{
			GameObject newKeyword = Instantiate(keywordsFull[index], this.transform);
			keywordObject.Add(new GameObjectInt(newKeyword, index));
			return;
		}

		// Max keywords
		if (keywordObject.Count >= maxNumberOfKeywords)
		{
			return;
		}

		// Replace full length keyword on the second addition
		if (keywordObject.Count == 1)
		{
			// Get index from name
			int ind = keywordObject[0].index;

			// Remove
			Destroy(keywordObject[0].obj);
			keywordObject.Clear();

			// Add new
			GameObject newShortKey = Instantiate(keywordsShort[ind], this.transform);
			keywordObject.Add(new GameObjectInt(newShortKey, ind));
		}

		// Otherwise add short keywords
		GameObject newShort = Instantiate(keywordsShort[index], this.transform);
		keywordObject.Add(new GameObjectInt(newShort, index));
	}
}