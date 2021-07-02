using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomKeywordSwatch : MonoBehaviour
{
	[SerializeField] private CustomKeywordData keywordData;
	[SerializeField] private TextMeshProUGUI dropdownLabel, dropdownItemLabel;
	[SerializeField] private Button[] swatches;
	[SerializeField] private Tooltip[] tooltips;

	public void DisableSwatches()
	{
		foreach (Button b in swatches)
		{
		    b.interactable = (keywordData.spriteIndex > 0) ? true : false;
		}
		foreach (Tooltip t in tooltips)
		{
			t.enabled = (keywordData.spriteIndex > 0) ? true : false;
		}
	}

	public void SetDropdownLabelColor()
	{
		var c = new Color();
		ColorUtility.TryParseHtmlString(keywordData.hexColor, out c);
		dropdownLabel.color = c;
		dropdownItemLabel.color = c;
	}

	private void OnEnable()
    {
		DisableSwatches();
		SetDropdownLabelColor();
	}
}
