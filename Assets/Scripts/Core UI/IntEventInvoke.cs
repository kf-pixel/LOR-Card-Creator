using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntEventInvoke : MonoBehaviour
{
	public UnityEvent[] events;

	public void Invoke(int i)
	{
		events[i].Invoke();
	}
}
