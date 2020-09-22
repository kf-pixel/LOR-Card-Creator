using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CardCode : MonoBehaviour
{
	[SerializeField] private TMP_InputField inputF;
	[SerializeField] private GameEvent changeCardTypeEvent;
	[SerializeField] private UnityEvent writeEvent;
	[SerializeField] private List<string> keywordRecord = new List<string>();

	[Header("Input Fields")]
	[SerializeField] private TMP_InputField manaF;
	[SerializeField] private TMP_InputField attackF;
	[SerializeField] private TMP_InputField healthF;
	[SerializeField] private TMP_InputField titleF;
	[SerializeField] private TMP_InputField groupF;
	[SerializeField] private TMP_InputField cardTextF;
	[SerializeField] private TMP_InputField levelUpTextF;

	[Header("Scriptable Object Data")]
	[SerializeField] private IntVariable region;
	[SerializeField] private IntVariable cardType;
	[SerializeField] private IntVariable rarity;

	[SerializeField] private StringVariable mana;
	[SerializeField] private StringVariable attack;
	[SerializeField] private StringVariable health;
	[SerializeField] private StringVariable title;
	[SerializeField] private StringVariable group;
	[SerializeField] private StringVariable cardText;
	[SerializeField] private StringVariable levelUp;

	[SerializeField] private IntEvent effectIntAdd;

	[Header("Scriptable Object Data Translation")]
	[SerializeField] private string[] regionNames;
	[SerializeField] private string[] rarityNames;
	[SerializeField] private string[] cardTypeNames;
	[SerializeField] private string[] cardEffectsNames;

	[Header("Artwork Data")]
	[SerializeField] private StringVariable artworkPath;
	[SerializeField] private FloatVariable artworkX;
	[SerializeField] private FloatVariable artworkY;
	[SerializeField] private FloatVariable artworkScale;
	[SerializeField] private Slider artworkScaleSlider;
	[SerializeField] private RectTransform[] artworkRectTransforms;

	public void ReadInput()
	{
		string code = "";

		// Title
		if (title.value.Length > 0)
		{
			code += title.value + ": ";
		}

		// Mana
		if (mana.value.Length > 0)
		{
			code += mana.value + " Mana ";
		}

		// Attack & Health
		if (cardType.value < 3 && attack.value.Length > 0 && health.value.Length > 0)
		{
			code += attack.value + "/" + health.value + " ";
		}

		// Region
		code += regionNames[region.value] + " ";

		// Rarity
		if (cardType.value != 1 && rarity.value != 0)
		{
			code += rarityNames[rarity.value] + " ";
		}

		// Card Type
		code += cardTypeNames[cardType.value] + " ";

		// Keywords
		foreach(string keyw in keywordRecord)
		{
			code += keyw + " ";
		}

		// Group
		if (group.value.Length > 0)
		{
			code += "#" + group.value + " ";
		}

		// Card Text
		if (cardText.value.Length > 0)
		{
			code += "\"" + cardText.value + "\" ";
		}

		// Level Up Text
		if (cardType.value == 1 && levelUp.value.Length > 0)
		{
			code += "Level Up: " + levelUp.value;
		}

		// Remove double spaces
		code = code.Replace("  ", " ");

		// Artwork data
		code += "*";
		/*
		if (artworkPath.value != null)
		{
			if (artworkPath.value.Length > 1)
			{
				code += artworkPath.value;
			}
		}
		*/

		code += "*" + ((int)artworkX.value).ToString() + "*" + ((int)artworkY.value).ToString() + "*" + artworkScale.value.ToString() + "*";

		// Apply
		inputF.SetTextWithoutNotify(code);
	}

	public void WriteInput(string s)
	{
		string working = s;
		//working = working.ToLower();

		// Get Artwork Data
		float scale = 300f;
		float x_pos = 0;
		float y_pos = 0;

		int asterisk4 = working.LastIndexOf("*", working.Length);
		if (asterisk4 > 0)
		{
			int asterisk3 = working.LastIndexOf("*", asterisk4 - 1);
			int asterisk2 = working.LastIndexOf("*", asterisk3 - 1);
			int asterisk1 = working.LastIndexOf("*", asterisk2 - 1);
			int asterisk0 = working.LastIndexOf("*", asterisk1 - 1);

			if (asterisk0 > 0)
			{
				float.TryParse(working.Substring(asterisk3 + 1, asterisk4 - asterisk3 - 1), out scale);
				float.TryParse(working.Substring(asterisk2 + 1, asterisk3 - asterisk2 - 1), out y_pos);
				float.TryParse(working.Substring(asterisk1 + 1, asterisk2 - asterisk1 - 1), out x_pos);

				// Apply Artwork Data
				artworkScaleSlider.value = scale;
				foreach (RectTransform rt in artworkRectTransforms)
				{
					rt.localPosition = new Vector3(x_pos, y_pos);
				}

				working = working.Remove(asterisk0, asterisk4 - asterisk0 + 1);
			}
		}


		// Get Card Text
		int cardTextStartIndex = working.IndexOf("\"", 0);
		int cardTextEndIndex = working.LastIndexOf("\"", working.Length - 1);

		if (cardTextStartIndex > 0 && cardTextEndIndex > 0)
		{
			string cardTextString = s.Substring(cardTextStartIndex, cardTextEndIndex - cardTextStartIndex + 1);

			working = working.Replace(cardTextString, "");

			cardTextString = cardTextString.Substring(1, cardTextString.Length - 2);
			cardText.value = cardTextString;
			cardTextF.SetTextWithoutNotify(cardTextString);
		}
		else
		{
			cardText.value = "";
		}

		// Get Region
		for (int i = 0; i < regionNames.Length; i++)
		{
			if (working.ToLower().Contains(regionNames[i].ToLower()))
			{
				region.value = i;
				working = working.Replace(regionNames[i].ToLower(), "");
				working = working.Replace(regionNames[i], "");
				break;
			}
		}

		// Get Rarity
		bool foundRarity = false;
		for (int i = 1; i < rarityNames.Length; i++)
		{
			if (working.ToLower().Contains(rarityNames[i].ToLower()))
			{
				rarity.value = i;
				working = working.Replace(rarityNames[i].ToLower(), "");
				working = working.Replace(rarityNames[i], "");
				foundRarity = true;
				break;
			}
		}
		if (!working.ToLower().Contains("champion") && !foundRarity)
		{
			rarity.value = 0;
		}

		// Get Card Type
		bool foundCardType = false;
		for (int i = 0; i < cardTypeNames.Length; i++)
		{
			if (working.ToLower().Contains(cardTypeNames[i].ToLower()))
			{
				foundCardType = true;
				cardType.value = i;
				working = working.Replace(cardTypeNames[i].ToLower(), "");
				working = working.Replace(cardTypeNames[i], "");
				break;
			}
		}

		// Default to either follower or slow spell if no Card Type found
		if (!foundCardType)
		{
			if (working.Contains("/"))
			{
				cardType.value = 0;
			}
			else
			{
				cardType.value = 3;
			}
		}

		// Determine whether Base or Leveled Champion
		if (cardType.value == 1)
		{
			if (!working.ToLower().Contains("level up:"))
			{
				cardType.value = 2;
			}
		}

		if (cardType.value == 0 && working.ToLower().Contains("level up:"))
		{
			cardType.value = 1;
		}

		// Get Level Up Text
		if (cardType.value == 1)
		{
			int levelUpIndex = working.ToLower().IndexOf("level up:");
			if (levelUpIndex > 0)
			{
				string levelUpSubstring = working.Substring(levelUpIndex);
				int levelUpColonIndex = levelUpSubstring.IndexOf(":");

				working = working.Replace(levelUpSubstring, "");
				levelUpSubstring = levelUpSubstring.Substring(levelUpColonIndex + 1);
				if (levelUpSubstring.StartsWith(" "))
				{
					levelUpSubstring = levelUpSubstring.Remove(0, 1);
				}

				levelUp.value = levelUpSubstring;
				levelUpTextF.SetTextWithoutNotify(levelUpSubstring);
			}
		}
		else
		{
			levelUp.value = "";
		}

		// Get Title
		if (working.Contains(":"))
		{
			int colonIndex = working.IndexOf(":");
			string titleSubstring = working.Substring(0, colonIndex);

			working = working.Remove(0, colonIndex + 1);
			title.value = titleSubstring;
			titleF.SetTextWithoutNotify(titleSubstring);
		}
		else
		{
			title.value = "Untitled";
			titleF.SetTextWithoutNotify("Untitled");
		}

		// Get Group
		if (working.Contains("#"))
		{
			int groupIndex = working.IndexOf("#");
			string groupSubstring = working.Substring(groupIndex);
			working = working.Replace(groupSubstring, "");

			group.value = groupSubstring.Remove(0, 1);
			groupF.SetTextWithoutNotify(groupSubstring);
		}
		else
		{
			group.value = "";
			groupF.SetTextWithoutNotify("");
		}

		// Remove Spaces
		working = working.ToLower();
		working = working.Replace(" ", "");

		// Get Mana
		if (working.ToLower().Contains("mana"))
		{
			string manaString = "";
			int manaIndex = working.ToLower().IndexOf("mana");

			// Find Numbers
			if (manaIndex > 0)
			{
				manaString += working[manaIndex - 1];
			}
			if (manaIndex > 1)
			{
				manaString = working[manaIndex - 2] + manaString;
			}

			// Apply
			if (manaString.Length > 0)
			{
				mana.value = manaString;
				working = working.Replace(manaString + "mana", "");

				manaF.SetTextWithoutNotify(manaString);
			}
			else
			{
				mana.value = 0.ToString();
				working = working.Replace("mana", "");

				manaF.SetTextWithoutNotify(0.ToString());
			}
		}

		// Get Attack/Health
		if (cardType.value < 3)
		{
			int slashIndex = working.IndexOf("/");
			if (slashIndex > 0)
			{
				string attackString = "";
				string healthString = "";

				// Get Attack
				if (slashIndex > 0)
				{
					attackString = working[slashIndex - 1].ToString();
				}
				if (slashIndex > 1)
				{
					attackString = working[slashIndex - 2].ToString() + attackString;
				}

				// Get Health
				int healthIndex1 = -1;
				int healthIndex2 = -1;
				bool success1 = false;
				bool success2 = false;

				if (slashIndex + 1 < working.Length)
				{
					success1 = int.TryParse(working[slashIndex + 1].ToString(), out healthIndex1);
				}
				if (slashIndex + 2 < working.Length)
				{
					success2 = int.TryParse(working[slashIndex + 2].ToString(), out healthIndex2);
				}

				if (healthIndex1 > -1 && success1)
				{
					healthString += healthIndex1;
				}
				if (healthIndex2 > -1 && success2)
				{
					healthString += healthIndex2;
				}

				// Apply
				attack.value = attackString;
				health.value = healthString;

				attackF.SetTextWithoutNotify(attackString);
				healthF.SetTextWithoutNotify(healthString);

				// Remove
				working = working.Replace(attackString + "/" + healthString, "");
			}
		}

		// Find Keywords
		List<int> keywordIndexes = new List<int>();

		changeCardTypeEvent.Raise();

		int ik = 0;
		foreach (string key in cardEffectsNames)
		{
			if (working.ToLower().Contains(key.ToLower().Replace(" ", "")))
			{				
				keywordIndexes.Add(ik);
				working = working.Replace(key.ToLower().Replace(" ", ""), "");
			}
			ik++;
		}

		// Apply
		writeEvent.Invoke();

		foreach(int ki in keywordIndexes)
		{
			effectIntAdd.Raise(ki);
		}
		
	}

	public void KeywordRecorder(int i)
	{
		if (cardEffectsNames[i].ToLower() == "fast" || cardEffectsNames[i].ToLower() == "burst" || cardEffectsNames[i].ToLower() == "slow" || cardType.value == 6 && cardEffectsNames[i].ToLower() == "skill")
		{
			return;
		}

		keywordRecord.Add(cardEffectsNames[i]);
	}

	public void ClearKeywordRecord()
	{
		keywordRecord.Clear();
	}
}