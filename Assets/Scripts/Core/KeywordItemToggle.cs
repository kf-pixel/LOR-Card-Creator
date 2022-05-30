using UnityEngine;
using UnityEngine.Events;
using TMPro;


public class KeywordItemToggle : MonoBehaviour
{
	public TextMeshProUGUI tmp;
	[HideInInspector] public string keywordName;
	[HideInInspector] public int keywordIndex;
	[SerializeField] private IntVariable numberOfKeywords;
	[SerializeField] private IntEvent keywordAdd;
	[SerializeField] private IntEvent keywordRemove;
	[SerializeField] private UnityEvent onTrue;
	[SerializeField] private UnityEvent onFalse;
	[SerializeField] private UnityEvent onMaxKeywords;

	public void Toggle(bool toggleOn)
	{
		if (toggleOn)
		{
			Add();
		}
		else
		{
			Remove();
		}
	}

	private void Add()
	{
		if (numberOfKeywords.value >= 5)
		{
			onMaxKeywords.Invoke();
			return;
		}
		keywordAdd.Raise(keywordIndex);
	}

	private void Remove()
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

	public void KeywordValueAddedCheck(string s)
	{
		if (s.StartsWith(keywordName))
		{
			onTrue.Invoke();
		}
	}
}
