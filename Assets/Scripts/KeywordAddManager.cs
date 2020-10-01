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
	[SerializeField] private KeywordBarController keywordBarController;

	private void Start()
	{
		// Spawn Prefabs
		for (int i = 0; i < keywordBarController.keywordsFull.Length; i++)
		{
			GameObject item = Instantiate(keywordItemPrefab, content);
			KeywordItemToggler tog = item.GetComponent<KeywordItemToggler>();
			if (tog != null)
			{
				tog.keywordIndex = i;
				tog.tmp.text = keywordBarController.keywordsFull[i].GetComponentInChildren<TextMeshProUGUI>().text;
			}
		}
	}
}
