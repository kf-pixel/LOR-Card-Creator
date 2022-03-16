using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Event", menuName = "Events/Event")]
public class GameEvent : ScriptableObject
{
	private List<GameEventListener> eventListeners = new List<GameEventListener>();

	[ContextMenu("Test Raise")]
	public void Raise()
	{
		for (int i = eventListeners.Count - 1; i >= 0; i--)
			eventListeners[i].OnEventRaised();
	}

	public void RegisterListener(GameEventListener listener)
	{
		if (!eventListeners.Contains(listener))
			eventListeners.Add(listener);
	}

	public void UnregisterListener(GameEventListener listener)
	{
		if (eventListeners.Contains(listener))
			eventListeners.Remove(listener);
	}

	[ContextMenu("Print Listeners")]
	public void PrintListeners()
	{
		foreach(GameEventListener listener in eventListeners)
		{
			Debug.Log(listener.ToString() + " is listening.");
		}
	}
}
