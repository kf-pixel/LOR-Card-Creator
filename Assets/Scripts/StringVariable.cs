using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new String", menuName = "String Variable")]
public class StringVariable : ScriptableObject
{
	public string value;
	public bool setEmptyValue = false;
	public string emptyValue = "";
	public UnityEvent valueChangedEvent;

	public void NewValue(string s)
	{
		value = s;
		if (string.IsNullOrEmpty(value) && setEmptyValue)
		{
			value = emptyValue;
		}
		valueChangedEvent.Invoke();
	}

	private void OnEnable()
	{
		value = "";
	}

	public void FloatToString(float i)
	{
		value = i.ToString();
		if (i == -1)
		{
			value = " ";
		}
		valueChangedEvent.Invoke();
	}
}