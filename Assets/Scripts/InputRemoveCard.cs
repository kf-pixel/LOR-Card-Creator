using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputRemoveCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	public UnityEvent onPointerUp;
	private bool isActive;

	public void OnPointerDown(PointerEventData eventData)
	{
		onPointerUp.Invoke();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isActive = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isActive = false;
	}

	public void OnPointerUp()
	{
		if (isActive)
		{
			onPointerUp.Invoke();
		}
	}

	
}
