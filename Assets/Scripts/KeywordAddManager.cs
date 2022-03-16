using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class KeywordAddManager : MonoBehaviour
{
	[SerializeField] private Transform content;
	[SerializeField] private GameObject keywordItemPrefab;
	[SerializeField] private GameObjectVariableList keywords;

	private void Start()
	{
		StartCoroutine(LoadKeywordPrefabs());
	}

	private IEnumerator LoadKeywordPrefabs()
	{
		yield return new WaitForSeconds(0.1f);
		// Spawn Prefabs
		FrameRateManager.Instance.RequestFullFrameRate();

		int waitCount = 0;
		for (int i = 0; i < keywords.value.Count; i++)
		{
			KeywordFormat keywordData = keywords.value[i].GetComponent<KeywordFormat>();
			if (keywordData.toggleable == false)
			{
				continue;
			}

			GameObject item = Instantiate(keywordItemPrefab, content);
			KeywordItemToggler tog = item.GetComponent<KeywordItemToggler>();
			if (tog != null)
			{
				tog.keywordIndex = i;
				tog.tmp.text = keywords.value[i].GetComponentInChildren<TextMeshProUGUI>().text;
				tog.keywordName = keywords.value[i].name;
			}

			waitCount++;
			if (waitCount > 5)
			{
				yield return new WaitForEndOfFrame();
				FrameRateManager.Instance.RequestFullFrameRate();
				waitCount = 0;
			}
		}
	}
}
