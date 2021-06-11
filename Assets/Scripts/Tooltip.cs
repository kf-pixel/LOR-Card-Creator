using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
		if (tooltipManager == null) return;

		// Set tooltip values
		tooltipManager.ChangeActive(true, content, header, delay);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovering = false;
		if (tooltipManager == null) return;

		tooltipManager.ChangeActive(false, content, header);
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