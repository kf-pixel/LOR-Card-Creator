using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColourHexInputter : MonoBehaviour
{
	[SerializeField] private TMP_InputField tmpi;
	[SerializeField] private StringVariable hex;
	[SerializeField] private FloatVariable referenceColourR;
	[SerializeField] private FloatVariable referenceColourG;
	[SerializeField] private FloatVariable referenceColourB;

	[Header("Resetting custom properties on restart")]
	[SerializeField] private Slider sliderR;
	[SerializeField] private Slider sliderG;
	[SerializeField] private Slider sliderB;
	[SerializeField] private StringVariable referenceLabel;
	[SerializeField] private IntVariable referenceIndex;
	[SerializeField] private TMP_Dropdown tmp_index_dd;
	[SerializeField] private TMP_InputField tmp_label_i;

	private void OnEnable()
	{
		if (referenceColourR.value == 0f && referenceColourG.value == 0f && referenceColourB.value == 0f)
		{
			referenceColourR.value = 1f;
			referenceColourG.value = 1f;
			referenceColourB.value = 1f;
		}

		sliderR.value = referenceColourR.value;
		sliderG.value = referenceColourG.value;
		sliderB.value = referenceColourB.value;

		ChangeValue();

		tmp_index_dd.SetValueWithoutNotify(referenceIndex.value);
		tmp_label_i.SetTextWithoutNotify(referenceLabel.value);
	}


	public void ChangeValue()
	{
		var rgbColour = new Color(referenceColourR.value, referenceColourG.value, referenceColourB.value);
		hex.value = "#" + ColorUtility.ToHtmlStringRGB(rgbColour);

		tmpi.SetTextWithoutNotify(hex.value);
	}

	public void HexToRGB()
	{
		var c = new Color();
		ColorUtility.TryParseHtmlString(tmpi.text, out c);

		referenceColourR.value = c.r;
		referenceColourG.value = c.g;
		referenceColourB.value = c.b;

		sliderR.value = c.r;
		sliderG.value = c.g;
		sliderB.value = c.b;

	}
}