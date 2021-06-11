using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class IntVariableToggler : MonoBehaviour
{
	[SerializeField] private IntVariable currentIntValue;
	[SerializeField] private int thisIntValue;
	[SerializeField] private Toggle toggleUI;
	[SerializeField] private UnityEvent onTrue;
	[SerializeField] private UnityEvent onFalse;

	private void OnEnable()
	{
		Parse();
		Invoke("Parse", 0.1f);
	}

	public void Parse(bool b)
	{
		if (b == true)
		{
			onTrue.Invoke();
		}
		else
		{
			onFalse.Invoke();
		}
	}

	public void Parse()
	{
		if (currentIntValue.value == thisIntValue)
		{
			toggleUI.isOn = true;
		}
		else
		{
			onFalse.Invoke();
		}
	}
}