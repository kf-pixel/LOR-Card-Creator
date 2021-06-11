using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoolVariableToggler : MonoBehaviour
{
	[SerializeField] private BoolVariable boolVariable;
	[SerializeField] private Toggle toggleUI;
	[SerializeField] private string playerPrefKey;
	[SerializeField] private UnityEvent onTrue;
	[SerializeField] private UnityEvent onFalse;

	private void Start()
	{
		LoadPlayerPrefsKey();
		if (boolVariable.value == true)
		{
			toggleUI.isOn = true;
		}
		else
		{
			toggleUI.isOn = false;
		}
		Solve();
	}

	public void Solve()
	{
		if (boolVariable.value == true)
		{
			onTrue.Invoke();
			if (!string.IsNullOrEmpty(playerPrefKey)) PlayerPrefs.SetInt(playerPrefKey, 1);
		}
		else
		{
			onFalse.Invoke();
			if (!string.IsNullOrEmpty(playerPrefKey)) PlayerPrefs.SetInt(playerPrefKey, 0);
		}
	}

	private void LoadPlayerPrefsKey()
	{
		if (string.IsNullOrEmpty(playerPrefKey)) return;
		boolVariable.value = PlayerPrefs.GetInt(playerPrefKey, 0) == 1 ? true : false;
	}
}