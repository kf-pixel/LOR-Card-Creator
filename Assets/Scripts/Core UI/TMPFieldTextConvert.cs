using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TMPFieldTextConvert : MonoBehaviour
{
	[SerializeField] private TMP_InputField inputField;
	[SerializeField] private Slider slider;

	public void IntToText(int i)
	{
		inputField.SetTextWithoutNotify(i.ToString());
	}

	public void FloatToText(float f)
	{
		inputField.SetTextWithoutNotify(f.ToString());
	}

	public void TextToSliderInt(string s)
	{
		int i = 0;
		int.TryParse(s, out i);

		slider.SetValueWithoutNotify(i);
	}
}