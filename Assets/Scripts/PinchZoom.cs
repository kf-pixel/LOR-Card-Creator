using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PinchZoom : MonoBehaviour
{
	[SerializeField] private float sensitivity;
	[SerializeField] private Slider slider;
	[SerializeField] private ScrollRect scroll, mainScroll;
	[SerializeField] private RectTransform rectTransform;
	private LORInputActions input;
	private Coroutine zoomCoroutine;
	private bool hovering;

	private void Awake()
    {
		input = new LORInputActions();
		input.Enable();
		input.UI.TouchPosition1Contact.performed += ContextMenu => ZoomStart();
		input.UI.TouchPosition1Contact.canceled += ContextMenu => ZoomEnd();
	}

	private void OnEnable()
	{
		scroll.enabled = true;
		mainScroll.enabled = true;
	}

	private void ZoomStart()
	{
		Vector2 touchPos0 = input.UI.TouchPosition0.ReadValue<Vector2>();
		Vector2 touchPos1 = input.UI.TouchPosition1.ReadValue<Vector2>();
		Vector2 localMousePosition0 = rectTransform.InverseTransformPoint(touchPos0);
		Vector2 localMousePosition1 = rectTransform.InverseTransformPoint(touchPos1);
		if (rectTransform.rect.Contains(localMousePosition1) && rectTransform.rect.Contains(localMousePosition0))
		{
			zoomCoroutine = StartCoroutine(ZoomDetection());
			scroll.enabled = false;
			mainScroll.enabled = false;
		}
	}

	private void ZoomEnd()
	{
		StopCoroutine(zoomCoroutine);
		scroll.enabled = true;
		mainScroll.enabled = true;
	}

	private IEnumerator ZoomDetection()
	{
        float dist = Vector2.Distance(input.UI.TouchPosition0.ReadValue<Vector2>(), input.UI.TouchPosition1.ReadValue<Vector2>());
		float previousDist = dist, delta = 0f;

		while (true)
		{
			dist = Vector2.Distance(input.UI.TouchPosition0.ReadValue<Vector2>(), input.UI.TouchPosition1.ReadValue<Vector2>());
			delta = dist - previousDist;

			if (delta != 0)
			{
				slider.value = Mathf.Clamp(slider.value + (delta * sensitivity), slider.minValue, slider.maxValue);

			}

			previousDist = dist;
			yield return null;
		}
	}
}
