using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using TMPro;

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
	[SerializeField] private IntVariable cardTypeIndex;
	[SerializeField] private IntVariable nullSpellSpeed;
	[SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
	[SerializeField] private float[] keywordSpacing = new float[] { 20, 20, 15, 10, 5 };

	[Header("Custom Spell Speed Data")]
	[SerializeField] private TextMeshProUGUI spellSpeedText;
	[SerializeField] private CustomKeywordData[] customKeywordData;

	[Header("Events")]
	[SerializeField] private UnityEvent noKeywordsEvent;
	[SerializeField] private UnityEvent someKeywordsEvent;
	[SerializeField] private UnityEvent maxKeywordsEvent;
	[SerializeField] private IntEvent keywordIntAddEvent, keywordIntRemoveEvent;
	private string[] cardTypeNames = new string[] { "Burst", "Fast", "Slow", "Skill", "Focus", "Landmark" };

	private void OnEnable()
	{
		CheckNumberOfKeywords();
	}

	public void AddKeywordValue(string keyvalue)
	{
		string[] keypair = keyvalue.Split(',');

		for (int index = 0; index < keywords.value.Count; index++)
		{
			if (keywords.value[index].name.StartsWith(keypair[0]))
			{
				GameObject newKeyword = Instantiate(keywords.value[index], this.transform);
				keywordInstances.Add(new GameObjectInt(newKeyword, index));

				KeywordFormat kwf = newKeyword.GetComponent<KeywordFormat>();
				kwf.keywordName = keywords.value[index].name;
				kwf.keywordIndex = index;

				int.TryParse(keypair[1], out kwf.keywordValue);

				SetCustomSpellSpeed();
				CheckNumberOfKeywords();
				FormatKeywords();
				break;
			}
		}
	}

	public void AddKeyword(int index)
	{
		if (index > keywords.value.Count - 1 || keywordInstances.Count >= maxNumberOfKeywords) return;

		GameObject newKeyword = Instantiate(keywords.value[index], this.transform);
		keywordInstances.Add(new GameObjectInt(newKeyword, index));

		KeywordFormat kwf = newKeyword.GetComponent<KeywordFormat>();
		kwf.keywordName = keywords.value[index].name;
		kwf.keywordIndex = index;

		SetCustomSpellSpeed();
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
					keywordInstances[0].obj.GetComponent<KeywordFormat>().SetToFull();
					SetCustomSpellSpeed();
					return;
				}
			}
		}

		SetCustomSpellSpeed();
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

	public void AddSpellKeyword(string keywordName)
	{
		if (nullSpellSpeed.value == 1)
		{
			return;
		}

		AddKeyword(keywordName);
	}

	public void NullSpellSpeedCheck()
	{
		if (nullSpellSpeed.value == 1)
		{
			// remove spell keywords
			RemoveKeyword(CardType.Name(cardTypeIndex.value));
		}
		else
		{
			spellSpeedText.enabled = false;

			string cardTypeKeywordName = CardType.Name(cardTypeIndex.value);
			foreach (GameObjectInt keyword in keywordInstances)
			{
				if (keyword.obj.name.StartsWith(cardTypeKeywordName))
				{
					return;
				}
			}
			// add spell keyword if it doesnt have one
			AddKeyword(CardType.Name(cardTypeIndex.value));
		}
	}

	public void SetCustomSpellSpeed()
	{
		if (nullSpellSpeed.value == 0)
		{
			spellSpeedText.enabled = false;
			return;
		}

		int customKWNumber = -1;
		bool success = false;
		foreach (GameObjectInt keyword in keywordInstances)
		{
			if (int.TryParse(keyword.obj.name.Replace("(Clone)", "").Replace("Custom", ""), out customKWNumber))
			{
				success = true;
			}
		}

		if (success)
		{
			if (customKeywordData[customKWNumber].spriteIndex > 0)
			{
				string spellSpeedString = $"<color={customKeywordData[customKWNumber].hexColor}><sprite name=\"Custom_{customKeywordData[customKWNumber].spriteIndex}\" tint>";
				if (customKeywordData[customKWNumber].spriteIndex > 68)
				{
					spellSpeedString = $"<color={customKeywordData[customKWNumber].hexColor}><sprite name=\"user{customKeywordData[customKWNumber].spriteIndex - 68}\" tint>";
				}
	
				spellSpeedText.text = spellSpeedString;
				spellSpeedText.enabled = true;
				return;
			}
		}
		spellSpeedText.enabled = false;
	}

	public void RemoveCardTypeKeywords()
	{
		foreach (string cn in cardTypeNames)
		{
			RemoveKeyword(cn);
		}
	}

	public void ClearKeywords()
	{
		for (int i = keywordInstances.Count - 1; i >= 0 ; i--)
		{
			Destroy(keywordInstances[i].obj);
			keywordInstances.Remove(keywordInstances[i]);
		}

		CheckNumberOfKeywords();
		FormatKeywords();
	}

	public void ClearKeywordsReaddSpell()
	{
		if (nullSpellSpeed.value == 0)
		{
			AddKeyword(CardType.Name(cardTypeIndex.value));
		}
		CheckNumberOfKeywords();
		FormatKeywords();
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
		GameObjectInt cardtypeKeyword = null;

		// Rearrange alphabetical order, then place cardtype keywords at the end
		keywordInstances = keywordInstances.OrderBy(k => k.index).ToList();
		for (int kwi = 0; kwi < keywordInstances.Count; kwi++)
		{
			keywordInstances[kwi].obj.transform.SetSiblingIndex(kwi);
			foreach (string ns in cardTypeNames)
			{
				if (keywordInstances[kwi].obj.name.StartsWith(ns))
				{
					cardtypeKeyword = keywordInstances[kwi];
					break;
				}
			}
		}

		if (cardtypeKeyword != null)
		{
			cardtypeKeyword.obj.transform.SetAsLastSibling();
		}

		// Check total lengths of full sized keywords
		float totalWidth = 0f;
		foreach (GameObjectInt kwi in keywordInstances)
		{
			totalWidth += kwi.obj.GetComponent<KeywordFormat>().GetWidth() + 20f;
		}

		// Format based on total length
		if (totalWidth > 275f || (autoShorten && keywordInstances.Count > 1))
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
		horizontalLayoutGroup.spacing = keywordSpacing[Mathf.Clamp(keywordInstances.Count - 1, 0, keywordSpacing.Length - 1)];
	}
}