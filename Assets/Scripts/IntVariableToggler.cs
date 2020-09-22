using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IntVariableToggler : MonoBehaviour
{
	[SerializeField] private IntVariable currentIntValue;
	[SerializeField] private int thisIntValue;
	[SerializeField] private Toggle toggleUI;
	[SerializeField] private UnityEvent onTrue;
	[SerializeField] private UnityEvent onFalse;

	private void Start()
	{
		if (currentIntValue.value == thisIntValue)
		{
			toggleUI.isOn = true;
		}
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