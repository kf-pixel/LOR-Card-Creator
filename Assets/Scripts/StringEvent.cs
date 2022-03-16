using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New String Event", menuName = "Events/String Event")]
public class StringEvent : ScriptableObject
{
	private List<StringEventListener> stringEventListeners = new List<StringEventListener>();
	public string testValue;

	public void Raise(string value)
	{
		for (int i = stringEventListeners.Count - 1; i >= 0; i--)
			stringEventListeners[i].OnEventRaised(value);
	}

	public void RegisterListener(StringEventListener listener)
	{
		if (!stringEventListeners.Contains(listener))
			stringEventListeners.Add(listener);
	}

	public void UnregisterListener(StringEventListener listener)
	{
		if (stringEventListeners.Contains(listener))
			stringEventListeners.Remove(listener);
	}
}

