using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;

public class TooltipManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private RectTransform tooltipRect;
	[SerializeField] private Image panelImage;
	[SerializeField] private TextMeshProUGUI tmpHeader;
	[SerializeField] private TextMeshProUGUI tmpContent;
	[SerializeField] private LayoutElement layoutElement;

	[Header("Tooltip Properties")]
	[SerializeField] private float maxWidth = 120f;
	public bool tooltipActive = false;
	private IEnumerator activeIEnumerator;
	private float lastActiveTime = -1;

	public void MovePosition(InputAction.CallbackContext ctx)
	{
		//SetAnchor(ctx.ReadValue<Vector2>());
		tooltipRect.transform.position = ctx.ReadValue<Vector2>();
	}

	public void SetAnchor(Vector2 mousePosition)
	{
		float pivotXW = (mousePosition.x - Screen.width / 2) >= 0 ? 1 : 0;
		float pivotYH = (mousePosition.y - Screen.height / 2) >= 0 ? 1 : 0;

		tooltipRect.pivot = new Vector2(pivotXW, pivotYH);
	}

	public void ChangeActive(bool b, string content, string header = "", float delay = 0.1f)
	{
		tooltipActive = b;

		if (activeIEnumerator != null)
		{
			StopCoroutine(activeIEnumerator);
		}
		activeIEnumerator = ChangeActiveCoroutine(b, content, header, delay);
		StartCoroutine(activeIEnumerator);
	}

	public IEnumerator ChangeActiveCoroutine(bool b, string content, string header = "", float delay = 0.1f)
	{
		// End here if set to inactive, or there is no content/header data
		if (b == false || string.IsNullOrEmpty(content) && string.IsNullOrEmpty(header))
		{
			tooltipRect.gameObject.SetActive(false);
			activeIEnumerator = null;
			lastActiveTime = Time.time;
			yield break;
		}

		// Turn off visibility of UI elements, before sizing is set
		panelImage.enabled = false;
		tmpHeader.alpha = 0;
		tmpContent.alpha = 0;

		// Add delay if turning on tooltip
		if (Time.time - lastActiveTime > 0.5f)
		{
#if UNITY_ANDROID
			delay = 0f;
#endif
			yield return new WaitForSeconds(delay);
		}

		// Apply text
		tooltipRect.gameObject.SetActive(true);

		tmpHeader.text = header;
		tmpContent.text = content;

		tmpHeader.ForceMeshUpdate();
		tmpContent.ForceMeshUpdate();

		// Disable Header tmp if no text
		bool headerActive = string.IsNullOrEmpty(header) ? false : true;
		tmpHeader.gameObject.SetActive(headerActive);

		yield return new WaitForEndOfFrame();

		// Check size limit
		//layoutElement.enabled = tmpContent.rectTransform.sizeDelta.x > maxWidth ? true : false;

		// Turn back on visibility of UI elements
		panelImage.enabled = true;
		tmpHeader.alpha = 1;
		tmpContent.alpha = 1;

		// Clear the active coroutine
		activeIEnumerator = null;
		lastActiveTime = Time.time;
	}
}
