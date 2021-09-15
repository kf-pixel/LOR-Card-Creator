using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MouseWheelZoom : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private bool hovering;
	[SerializeField] private float sensitivity = 20f;
	[SerializeField] private Slider slider;

	public void OnPointerEnter(PointerEventData eventData)
	{
		hovering = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hovering = false;
	}

	private void Update()
	{
		if (!hovering)
		{
			return;
		}

		float scrollY = Input.mouseScrollDelta.y;
		if (scrollY != 0f)
		{
			slider.value = Mathf.Clamp(slider.value + (scrollY * sensitivity), slider.minValue, slider.maxValue);
		}
	}
}