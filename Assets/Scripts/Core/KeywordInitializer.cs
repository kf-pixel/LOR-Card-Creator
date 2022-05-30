using System.Collections;
using UnityEngine;
using TMPro;

public class KeywordInitializer : MonoBehaviour
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
			KeywordItemToggle tog = item.GetComponent<KeywordItemToggle>();
			if (tog != null)
			{
				tog.keywordIndex = i;
				tog.tmp.text = keywords.value[i].GetComponentInChildren<TextMeshProUGUI>().text;
				tog.keywordName = keywords.value[i].name;
			}

			waitCount++;
			if (waitCount > 3)
			{
				yield return new WaitForEndOfFrame();
				FrameRateManager.Instance.RequestFullFrameRate();
				waitCount = 0;
			}
		}
	}
}
