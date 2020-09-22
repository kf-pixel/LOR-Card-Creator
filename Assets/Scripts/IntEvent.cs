using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Int Event", menuName = "Events/Int Event")]
public class IntEvent : ScriptableObject
{
	private List<IntEventListener> intEventListeners = new List<IntEventListener>();
	public int testValue;

	public void Raise(int value)
	{
		for (int i = intEventListeners.Count - 1; i >= 0; i--)
			intEventListeners[i].OnEventRaised(value);
	}

	public void RegisterListener(IntEventListener listener)
	{
		if (!intEventListeners.Contains(listener))
			intEventListeners.Add(listener);
	}

	public void UnregisterListener(IntEventListener listener)
	{
		if (intEventListeners.Contains(listener))
			intEventListeners.Remove(listener);
	}
}

