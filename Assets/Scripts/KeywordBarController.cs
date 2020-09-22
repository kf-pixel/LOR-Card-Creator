using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeywordBarController : MonoBehaviour
{
	[SerializeField] private List<GameObject> effectKeywordsList = new List<GameObject>();
	[SerializeField] private GameObject[] keywordsFull;
	[SerializeField] private GameObject[] keywordsShort;
	[SerializeField] private int maxNumberOfKeywords = 5;

	[Header("Events")]
	[SerializeField] private UnityEvent onEnableEvent;
	[SerializeField] private UnityEvent onDisableEvent;
	[SerializeField] private UnityEvent noKeywordsEvent;
	[SerializeField] private UnityEvent someKeywordsEvent;

	private void OnEnable()
	{
		onEnableEvent.Invoke();
		CheckNumberOfKeywords();
	}

	private void OnDisable()
	{
		onDisableEvent.Invoke();

		foreach (GameObject go in effectKeywordsList)
		{
			Destroy(go);
		}
		effectKeywordsList.Clear();
	}

	public void AddKeyword(int index)
	{
		SolveNewKeyword(index);
		FormatKeywords();
		CheckNumberOfKeywords();
	}

	public void ClearKeywords()
	{
		foreach (GameObject go in effectKeywordsList)
		{
			Destroy(go);
		}
		effectKeywordsList.Clear();
		CheckNumberOfKeywords();
	}

	private void CheckNumberOfKeywords()
	{
		if (effectKeywordsList.Count > 0)
		{
			someKeywordsEvent.Invoke();
		}
		else
		{
			noKeywordsEvent.Invoke();
		}
	}

	private void FormatKeywords()
	{
		int i = effectKeywordsList.Count;

		// 2 keywords
		if (i == 2)
		{
			effectKeywordsList[0].transform.localPosition = new Vector3(-40, 0);
			effectKeywordsList[1].transform.localPosition = new Vector3(40, 0);
			return;
		}

		// 3 keywords
		if (i == 3)
		{
			effectKeywordsList[0].transform.localPosition = new Vector3(-60, 0);
			effectKeywordsList[1].transform.localPosition = new Vector3(0, 0);
			effectKeywordsList[2].transform.localPosition = new Vector3(60, 0);
			return;
		}

		// 4 keywords
		if (i == 4)
		{
			effectKeywordsList[0].transform.localPosition = new Vector3(-90, 0);
			effectKeywordsList[1].transform.localPosition = new Vector3(-30, 0);
			effectKeywordsList[2].transform.localPosition = new Vector3(30, 0);
			effectKeywordsList[3].transform.localPosition = new Vector3(90, 0);
			return;
		}

		// 5 keywords
		if (i == 5)
		{
			effectKeywordsList[0].transform.localPosition = new Vector3(-110, 0);
			effectKeywordsList[1].transform.localPosition = new Vector3(-55, 0);
			effectKeywordsList[2].transform.localPosition = new Vector3(0, 0);
			effectKeywordsList[3].transform.localPosition = new Vector3(55, 0);
			effectKeywordsList[4].transform.localPosition = new Vector3(110, 0);
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
		if (effectKeywordsList.Count == 0)
		{
			GameObject newKeyword = Instantiate(keywordsFull[index], this.transform);
			newKeyword.name = index.ToString();
			effectKeywordsList.Add(newKeyword);
			return;
		}

		// Max keywords
		if (effectKeywordsList.Count >= maxNumberOfKeywords)
		{
			return;
		}

		// Replace full length keyword
		if (effectKeywordsList.Count == 1)
		{
			// Get index from name
			int ind = 0;
			int.TryParse(effectKeywordsList[0].name, out ind);

			// Remove
			Destroy(effectKeywordsList[0]);
			effectKeywordsList.Clear();

			// Add new
			GameObject newShortKey = Instantiate(keywordsShort[ind], this.transform);
			effectKeywordsList.Add(newShortKey);
		}

		// Otherwise add short keywords
		GameObject newShort = Instantiate(keywordsShort[index], this.transform);
		effectKeywordsList.Add(newShort);
	}
}