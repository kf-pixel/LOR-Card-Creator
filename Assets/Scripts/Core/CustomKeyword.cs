using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CustomKeyword : MonoBehaviour
{
	[System.Serializable]	private class UnityStringEvent : UnityEvent<string> { }
	[SerializeField] private RectTransform rect;
	[SerializeField] private TextMeshProUGUI textfield;
	[SerializeField] private CustomKeywordData KWData;
	[SerializeField] private UnityStringEvent textUpdate;

	private void OnEnable()
	{
		UpdateTextAndSize();
	}

	public void UpdateTextAndSize()
	{
		var rgbColour = new Color(KWData.colorR, KWData.colorG, KWData.colorB);
		var hexColour = ColorUtility.ToHtmlStringRGB(rgbColour);

		// Add the Sprite to the display if the index is higher than 0
		if (KWData.spriteIndex <= 68)
		{
			textfield.text = KWData.spriteIndex > 0 ? "<color=#" + hexColour + ">" + "<sprite name=\"Custom_" + KWData.spriteIndex + "\" tint></color>" : "";
		}
		else // custom user sprites
		{
			textfield.text = $"<color=#{hexColour}><sprite name=\"user{KWData.spriteIndex - 68}\" tint></color>";
		}
		textfield.text += (string.IsNullOrWhiteSpace(KWData.label)) ? "custom" : KWData.label;

		// Update keyword format full text 
		textUpdate.Invoke(textfield.text);
	}
}