using UnityEngine;
using TMPro;

public class InputStatsClick : BasePointerClick
{
	[SerializeField] private TMP_InputField inputF;

	protected override void OnClick()
	{
		if (shift || ctrl) return;
		if (int.TryParse(inputF.text, out int stringInt) == true)
		{
			inputF.text = (stringInt + 1).ToString();
		}
		else
		{
			inputF.text = "1";
			return;
		}
	}

	protected override void OnAltClick()
	{
		if (int.TryParse(inputF.text, out int stringInt) == true)
		{
			inputF.text = (stringInt - 1).ToString();
		}
		else
		{
			inputF.text = "1";
			return;
		}
	}
}
