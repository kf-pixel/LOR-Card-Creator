using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new Int", menuName = "Int Variable")]
public class IntVariable : ScriptableObject
{
	[System.NonSerialized] public int value;
	public int defaultValue;
	public UnityEvent valueChangedEvent;

	public void NewInt(int i)
	{
		value = i;
		valueChangedEvent.Invoke();
	}

	private void OnEnable()
	{
		if (defaultValue != 0) value = defaultValue;
	}
}

