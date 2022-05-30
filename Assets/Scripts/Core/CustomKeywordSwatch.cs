using System.Collections;
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
		bool isEnabled = keywordData.spriteIndex > 0 ? true : false;
		foreach (Button b in swatches)
		{
		    b.interactable = isEnabled;
		}
		foreach (Tooltip t in tooltips)
		{
			t.enabled = isEnabled;
		}
	}

	public void SetDropdownLabelColor()
	{
		var c = new Color();
		ColorUtility.TryParseHtmlString(keywordData.hexColor, out c);
		dropdownLabel.color = c;
		dropdownItemLabel.color = c;
		StartCoroutine(SetDropdownColorIE());
	}

	private IEnumerator SetDropdownColorIE()
	{
		yield return new WaitForEndOfFrame();
		
	}

	private void OnEnable()
    {
		DisableSwatches();
		SetDropdownLabelColor();
	}
}
