using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoolParse : MonoBehaviour
{
	[SerializeField] private UnityEvent onTrue;
	[SerializeField] private UnityEvent onFalse;


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
}
