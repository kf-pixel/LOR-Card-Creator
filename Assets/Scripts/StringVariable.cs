using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new String", menuName = "String Variable")]
public class StringVariable : ScriptableObject
{
	[System.NonSerialized] public string value;
	public UnityEvent valueChangedEvent;

	public void NewValue(string s)
	{
		value = s;
		valueChangedEvent.Invoke();
	}

	public void FloatToString(float i)
	{
		value = i.ToString();
		valueChangedEvent.Invoke();
	}
}