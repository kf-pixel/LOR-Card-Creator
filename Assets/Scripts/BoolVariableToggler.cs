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
			PlayerPrefs.SetInt(playerPrefKey, 1);
		}
		else
		{
			onFalse.Invoke();
			PlayerPrefs.SetInt(playerPrefKey, 0);
		}
	}

	private void LoadPlayerPrefsKey()
	{
		boolVariable.value = PlayerPrefs.GetInt(playerPrefKey, 0) == 1 ? true : false;
	}
}