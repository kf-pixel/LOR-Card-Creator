using UnityEngine;
using TMPro;

public class KeywordColourHexInput : MonoBehaviour
{
	[Header("Slider & UI References")]
	[SerializeField] private TMP_InputField tmpi;
	[SerializeField] private TMP_InputField tmp_label_i;
	[SerializeField] private TMP_Dropdown tmp_index_dd;

	[Header("Custom Keyword Reference Data")]
	[SerializeField] private CustomKeywordData KWData;

	private void OnEnable()
	{
		if (KWData.colorR == 0f && KWData.colorG == 0f && KWData.colorB == 0f)
		{
			KWData.colorR = 1f;
			KWData.colorG = 1f;
			KWData.colorB = 1f;
		}

		ChangeValue();

		tmp_index_dd.SetValueWithoutNotify(KWData.spriteIndex);
		tmp_label_i.SetTextWithoutNotify(KWData.label);
	}


	public void ChangeValue()
	{
		var rgbColour = new Color(KWData.colorR, KWData.colorG, KWData.colorB);
		KWData.hexColor = "#" + ColorUtility.ToHtmlStringRGB(rgbColour);

		tmpi.SetTextWithoutNotify(KWData.hexColor);
	}

	public void HexToRGB()
	{
		var c = new Color();
		ColorUtility.TryParseHtmlString(tmpi.text, out c);

		KWData.colorR = c.r;
		KWData.colorG = c.g;
		KWData.colorB = c.b;
	}

	public void HexToRGB(string hexs)
	{
		var c = new Color();
		ColorUtility.TryParseHtmlString(hexs, out c);

		KWData.colorR = c.r;
		KWData.colorG = c.g;
		KWData.colorB = c.b;
	}

	// used in saving
	public void HexDataToRGB()
	{
		var c = new Color();
		ColorUtility.TryParseHtmlString(KWData.hexColor, out c);
	}
}