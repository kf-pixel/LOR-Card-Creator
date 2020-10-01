using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CustomKeyword : MonoBehaviour
{
	[SerializeField] private RectTransform rect;
	[SerializeField] private TextMeshProUGUI textfield;
	[SerializeField] private CustomKeywordData KWData;
	[SerializeField] private bool isFullLengthKeyword = true;

	private void OnEnable()
	{
		UpdateTextAndSize();
	}

	public void UpdateTextAndSize()
	{
		var rgbColour = new Color(KWData.colorR, KWData.colorG, KWData.colorB);
		var hexColour = ColorUtility.ToHtmlStringRGB(rgbColour);

		// Add the Sprite to the display if the index is higher than 0
		if (KWData.spriteIndex > 0)
		{
			textfield.text = "<color=#" + hexColour + ">" + "<sprite name=\"Custom_" + KWData.spriteIndex + "\" tint></color>";

			if (isFullLengthKeyword) // Adds the keyword name (for single added keywords)
			{
				textfield.text += (string.IsNullOrWhiteSpace(KWData.label)) ? "custom" : KWData.label;
				StartCoroutine(WaitForTextRender());
			}
		}
		else // Rendering for no sprite keywords
		{
			if (isFullLengthKeyword)
			{
				textfield.text = (string.IsNullOrWhiteSpace(KWData.label)) ? "Custom" : KWData.label;
				StartCoroutine(WaitForTextRender());
			}
			else
			{
				textfield.text = " ";
			}
		}

	}

	private IEnumerator WaitForTextRender()
	{
		yield return new WaitForEndOfFrame();
		rect.sizeDelta = new Vector2(Mathf.Clamp(textfield.renderedWidth + 25f, 50f, 900f), rect.sizeDelta.y);
	}
}