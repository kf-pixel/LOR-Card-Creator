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
		// Spawn Prefabs
		for (int i = 0; i < keywords.value.Count; i++)
		{
			GameObject item = Instantiate(keywordItemPrefab, content);
			KeywordItemToggler tog = item.GetComponent<KeywordItemToggler>();
			if (tog != null)
			{
				tog.keywordIndex = i;
				tog.tmp.text = keywords.value[i].GetComponentInChildren<TextMeshProUGUI>().text;
				tog.keywordName = keywords.value[i].name;
			}
		}
	}
}
