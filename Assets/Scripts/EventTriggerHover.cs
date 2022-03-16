using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventTriggerHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private UnityEvent onEnter;
	[SerializeField] private UnityEvent onExit;

	private void OnDisable()
	{
		onExit.Invoke();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
#if UNITY_ANDROID
		return;
#endif
		FrameRateManager.Instance.RequestFullFrameRate();
		onEnter.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
#if UNITY_ANDROID
		return;
#endif
		FrameRateManager.Instance.RequestFullFrameRate();
		onExit.Invoke();
	}
}
