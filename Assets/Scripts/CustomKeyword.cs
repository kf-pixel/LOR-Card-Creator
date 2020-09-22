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
	[SerializeField] private StringVariable referenceString;
	[SerializeField] private IntVariable referenceSpriteIndex;
	[SerializeField] private FloatVariable referenceColourR;
	[SerializeField] private FloatVariable referenceColourG;
	[SerializeField] private FloatVariable referenceColourB;
	[SerializeField] private bool isFullLengthKeyword = true;

	private void OnEnable()
	{
		UpdateTextAndSize();
	}

	public void UpdateTextAndSize()
	{
		var rgbColour = new Color(referenceColourR.value, referenceColourG.value, referenceColourB.value);
		var hexColour = ColorUtility.ToHtmlStringRGB(rgbColour);

		if (isFullLengthKeyword) // add sprite and string
		{

			textfield.text = "<color=#" + hexColour + ">" + "<sprite name=\"Custom_" + referenceSpriteIndex.value + "\" tint></color>" + referenceString.value;
			StartCoroutine(WaitForTextRender());
		}
		else // only add the sprite
		{
			textfield.text = "<color=#" + hexColour + ">" + "<sprite name=\"Custom_" + referenceSpriteIndex.value + "\" tint></color>";
		}
	}

	private IEnumerator WaitForTextRender()
	{
		yield return new WaitForSeconds(0.05f);

		if (textfield.text != null)
		{
			rect.sizeDelta = new Vector2(textfield.renderedWidth + 40f, rect.sizeDelta.y);
		}
		else
		{
			textfield.text = "custom";
			rect.sizeDelta = new Vector2(120f, rect.sizeDelta.y);
		}
	}
}