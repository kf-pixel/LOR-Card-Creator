using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] [TextArea] private string header;
	[SerializeField] [TextArea] private string content;

	private TooltipManager tooltipManager;
	[SerializeField] private float delay = 0.5f;

	private bool isHovering = false;

	private void Awake()
	{
		tooltipManager = FindObjectOfType<TooltipManager>();
	}

	public void NewTooltipText(string newContentText, string newHeaderText = "")
	{
		content = newContentText;
		header = newHeaderText;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isHovering = true;

#if UNITY_ANDROID
		return;
#endif

		tooltipManager.ChangeActive(true, content, header, delay);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovering = false;

#if UNITY_ANDROID
		return;
#endif
		tooltipManager.ChangeActive(false, content, header);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
#if UNITY_ANDROID
		if (isHovering) tooltipManager.ChangeActive(true, content, header, delay);
#endif
	}

	public void OnPointerUp(PointerEventData eventData)
	{
#if UNITY_ANDROID
		tooltipManager.ChangeActive(false, content, header);
		isHovering = false;
#endif
	}

	private void OnDisable()
	{
		if (isHovering)
		{
			if (tooltipManager != null)
			{
				tooltipManager.ChangeActive(false, content, header);
			}
			isHovering = false;
		}
	}
}