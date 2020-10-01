using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class KeywordTooltip : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI titleTMP;
	[SerializeField] private TextMeshProUGUI descriptionTMP;

	[Header("Custom Keyword Data")]
	[SerializeField] private CustomKeywordData[] KWData;
	[SerializeField] private IntVariable KWTabIndex;

	private void OnEnable()
	{
		TextUpdate();
	}

	private void OnDisable()
	{
		TextUpdate();
	}

	public void TextUpdate()
	{
		CustomKeywordData kw = KWData[KWTabIndex.value];

		string titleText = kw.label;
		if (titleText == null)
		{
			titleText = "keyword";
		}
		else if (titleText.Length == 0)
		{
			titleText = "keyword";
		}

		string descriptionText = kw.description;
		if (descriptionText == null)
		{
			descriptionText = "description";
		}
		else if (descriptionText.Length == 0)
		{
			descriptionText = "description";
		}

		// Update Title
		string newText = kw.spriteIndex > 0 ?
			"<voffset=16><size=80%><color=" + kw.hexColor + "><sprite name=\"Custom_" + kw.spriteIndex + "\" tint></voffset></size></color>" + titleText : titleText;

		titleTMP.text = newText;

		// Update Description
		descriptionTMP.text = descriptionText;
	}
}