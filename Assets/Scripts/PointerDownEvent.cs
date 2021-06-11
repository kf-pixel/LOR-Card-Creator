using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PointerDownEvent : MonoBehaviour, IPointerDownHandler
{
	public UnityEvent onPointerDown;

	public void OnPointerDown(PointerEventData eventData)
	{
		onPointerDown.Invoke();
	}
}