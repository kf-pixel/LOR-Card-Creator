using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class KeywordTooltip : MonoBehaviour
{
	[SerializeField] private BoolVariable disableSpriteTooltip;
	[SerializeField] private TextMeshProUGUI titleTMP;
	[SerializeField] private TextMeshProUGUI descriptionTMP;

	[Header("Custom Keyword Data")]
	[SerializeField] private StringVariable title;
	[SerializeField] private StringVariable description;
	[SerializeField] private StringVariable colour;
	[SerializeField] private IntVariable spriteIndex;

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
		string titleText = title.value;
		if (titleText == null)
		{
			titleText = "keyword";
		}
		else if (titleText.Length == 0)
		{
			titleText = "keyword";
		}

		string descriptionText = description.value;
		if (descriptionText == null)
		{
			descriptionText = "description";
		}
		else if (descriptionText.Length == 0)
		{
			descriptionText = "description";
		}

		// Update Title
		//string newText = titleText + "|<voffset=20><size=75%><color=" + colour.value + "><sprite=" + spriteIndex.value + " tint>";
		string newText = "<voffset=16><size=80%><color=" + colour.value + "><sprite name=\"Custom_" + spriteIndex.value + "\" tint></voffset></size></color>" + titleText;

		// Disable custom sprite if checked
		if (disableSpriteTooltip.value == true)
		{
			newText = titleText;
		}

		titleTMP.text = newText;

		// Update Description
		descriptionTMP.text = descriptionText;
	}
}