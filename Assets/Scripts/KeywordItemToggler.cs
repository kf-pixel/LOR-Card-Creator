using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


public class KeywordItemToggler : MonoBehaviour
{
	public TextMeshProUGUI tmp;
	public IntVariable numberOfKeywords;
	public string keywordName;
	public int keywordIndex;
	public IntEvent keywordAdd;
	public IntEvent keywordRemove;
	public UnityEvent onTrue;
	public UnityEvent onFalse;
	public UnityEvent onMaxKeywords;

	public void Add()
	{
		if (numberOfKeywords.value >= 5)
		{
			onMaxKeywords.Invoke();
			return;
		}
		keywordAdd.Raise(keywordIndex);
	}

	public void Remove()
	{
		keywordRemove.Raise(keywordIndex);
	}

	public void KeywordAddedCheck(int i)
	{
		if (i == keywordIndex)
		{
			onTrue.Invoke();
		}
	}

	public void KeywordRemovedCheck(int i)
	{
		if (i == keywordIndex)
		{
			onFalse.Invoke();
		}
	}
}
