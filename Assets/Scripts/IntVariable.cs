using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new Int", menuName = "Int Variable")]
public class IntVariable : ScriptableObject
{
	[System.NonSerialized] public int value;
	public UnityEvent valueChangedEvent;

	public void NewInt(int i)
	{
		value = i;
		valueChangedEvent.Invoke();
	}
}

