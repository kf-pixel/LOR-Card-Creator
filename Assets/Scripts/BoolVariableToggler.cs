using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BoolVariableToggler : MonoBehaviour
{
	[SerializeField] private BoolVariable boolVariable;
	[SerializeField] private Toggle toggleUI;
	[SerializeField] private UnityEvent onTrue;
	[SerializeField] private UnityEvent onFalse;

	private void Start()
	{
		if (boolVariable.value == true)
		{
			toggleUI.isOn = true;
		}
	}

	public void Solve()
	{
		if (boolVariable.value == true)
		{
			onTrue.Invoke();

		}
		else
		{
			onFalse.Invoke();

		}
	}
}