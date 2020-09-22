using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new Bool", menuName = "Bool Variable")]
public class BoolVariable : ScriptableObject
{
	[System.NonSerialized] public bool value;
	public UnityEvent valueChangedEvent;

	public void NewBool(bool b)
	{
		value = b;
		valueChangedEvent.Invoke();
	}
}