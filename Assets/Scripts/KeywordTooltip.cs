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
	[SerializeField] private AutoTextReplacer autoTextReplacer;

	[Header("Custom Keyword Data")]
	[SerializeField] private CustomKeywordData[] KWData;
	[SerializeField] private IntVariable KWTabIndex;

	private void OnEnable()
	{
		TextUpdate();
		autoTextReplacer.ReplaceTMPUGUI();
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

		// Keyword Description, add tags
		string descriptionText = kw.description;
		if (descriptionText == null)
		{
			descriptionText = "description";
		}
		else if (descriptionText.Length == 0)
		{
			descriptionText = "description";
		}

		// [] and {} tags
		if (descriptionText.Contains("[") && descriptionText.Contains("]"))
		{
			descriptionText = descriptionText.Replace("[", "<style=Card>");
			descriptionText = descriptionText.Replace("]", "</style>");
		}

		if (descriptionText.Contains("{") && descriptionText.Contains("}"))
		{
			descriptionText = descriptionText.Replace("{", "<style=Keyword>");
			descriptionText = descriptionText.Replace("}", "</style>");
		}

		// New line tag
		descriptionText = descriptionText.Replace("`", "<br>");

		// Skill Sprite
		descriptionText = descriptionText.Replace("@", "<sprite name=skill>");

		// break
		descriptionText = descriptionText.Replace("//", "<b></b>");

		// Update Title
		string newText = titleText;
		if (kw.spriteIndex <= 0)
		{
			newText = $"{titleText}";
		}
		else if (kw.spriteIndex <= 68)
		{
			newText = $"<voffset=14><size=80%><color={kw.hexColor}><sprite name=\"Custom_{kw.spriteIndex}\" tint></voffset></size></color>{titleText}";
		}
		else // user sprites
		{
			newText = $"<voffset=14><size=80%><color={kw.hexColor}><sprite name=\"user{kw.spriteIndex - 68}\" tint></voffset></size></color>{titleText}";
		}

		titleTMP.text = newText;

		// Update Description
		descriptionTMP.text = descriptionText;
	}
}