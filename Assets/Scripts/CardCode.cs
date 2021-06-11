﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CardCode : MonoBehaviour
{
	[SerializeField] private TMP_InputField inputF;
	[SerializeField] private FileUpload artworkUploader;
	[SerializeField] private GameEvent changeCardTypeEvent;
	[SerializeField] private GameEvent clearKeywordsEvent;
	[SerializeField] private GameEvent cardReadEvent;
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
	[SerializeField] private TMP_InputField creditF;

	[Header("Scriptable Object Data")]
	[SerializeField] private IntVariable cardType;
	[SerializeField] private IntVariable region, region2;
	[SerializeField] private IntVariable rarity;

	[SerializeField] private StringVariable mana;
	[SerializeField] private StringVariable attack;
	[SerializeField] private StringVariable health;
	[SerializeField] private StringVariable title;
	[SerializeField] private StringVariable group;
	[SerializeField] private StringVariable cardText;
	[SerializeField] private StringVariable levelUp;
	[SerializeField] private StringVariable credit;

	[SerializeField] private IntEvent effectIntAdd;

	[Header("Scriptable Object Data Translation")]
	[SerializeField] private string[] regionNames;
	[SerializeField] private string[] rarityNames;
	[SerializeField] private string[] cardTypeNames;
	[SerializeField] private GameObjectVariableList keywords;
	private List<string> keywordNames = new List<string>();

	[Header("Artwork Data")]
	[SerializeField] private StringVariable artworkPath;
	[SerializeField] private FloatVariable artworkX;
	[SerializeField] private FloatVariable artworkY;
	[SerializeField] private FloatVariable artworkScale;
	[SerializeField] private BoolVariable manualShadowScale;
	[SerializeField] private Slider artworkScaleSlider;
	[SerializeField] private Slider artworkShadowScale;
	[SerializeField] private RectTransform[] artworkRectTransforms;

	// Internal
	private bool readInputThisFrame;

	private void Start()
	{
		// Generate keyword names from GameObject prefabs
		foreach (GameObject keywordPrefab in keywords.value)
		{
			keywordNames.Add(keywordPrefab.name);
		}
	}

	private void LateUpdate()
	{
		if (readInputThisFrame)
		{
			DoReadInput();
			readInputThisFrame = false;
		}
	}

	public void ReadInput()
	{
		readInputThisFrame = true;
	}

	private void DoReadInput()
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
		if ((cardType.value < 3 || cardType.value == 8) && attack.value.Length > 0 && health.value.Length > 0)
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
		if (cardType.value == 1 || cardType.value == 2)
		{
			code += "Level Up: " + levelUp.value;
		}

		// Remove double spaces
		code = code.Replace("  ", " ");

		// Artwork data
		code += "*";

		// Read artwork path data on standalone
		if (artworkPath.value != null)
		{
			if (artworkPath.value.Length > 1)
			{
				code += artworkPath.value.Replace(Application.persistentDataPath, "&local&");
			}
		}

		code += "*" + ((int)artworkX.value).ToString() + "*" + ((int)artworkY.value).ToString() + "*" + artworkScale.value.ToString() + "*";

		// Artist Credit
		if (!string.IsNullOrEmpty(credit.value))
		{
			code += " &@" + (credit.value);
		}

		// Shadow Scale
		code += " &~" + artworkShadowScale.value;

		// Region 2
		if (region2.value != 13)
		{
			code += " ++" + regionNames[region2.value];
		}

		// Apply
		inputF.SetTextWithoutNotify(code);

		// Raise Event
		cardReadEvent.Raise();
	}

	public void WriteInput(string s)
	{
		string working = s;

		// Clear Existing Keywords
		clearKeywordsEvent.Raise();

		// Get Region 2
		int region2Index = working.IndexOf(" ++", 0);
		if (region2Index > 1)
		{
			string region2String = working.Substring(region2Index);
			for (int i = 0; i < regionNames.Length; i++)
			{
				if (region2String.Contains(regionNames[i]))
				{
					working.Remove(region2Index);
					region2.value = i;
					break;
				}
			}
			/*
			region2String = region2String.Replace(" ++", "");
			int region2Int = -1;
			int.TryParse(region2String, out region2Int);
			if (region2Int != -1) region2.value = region2Int;
			else region2.value = 13;
			working.Remove(region2Index);
			*/
		}
		else region2.value = 13;

		// Get Shadow Scale
		int shadowStringIndex = working.IndexOf(" &~", 0);
		if (shadowStringIndex > 1)
		{
			string shadowString = working.Substring(shadowStringIndex);
			shadowString = shadowString.Replace(" &~", "");
			int shadowValue = 0;
			int.TryParse(shadowString, out shadowValue);
			artworkShadowScale.value = shadowValue;
			working = working.Remove(shadowStringIndex);
		}
		else
		{
			artworkShadowScale.value = -400;
		}

		// Get Artist Credit
		int creditIndex = working.IndexOf(" &@");
		if (creditIndex > 0)
		{
			string creditString = working.Substring(creditIndex + 3);
			creditF.text = creditString;

			working = working.Remove(creditIndex);
		}
		else
		{
			creditF.text = "";
		}

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

				// Get Artwork Local Path
				if (asterisk1 - asterisk0 > 2)
				{
					string foundArtwork = working.Substring(asterisk0 + 1, asterisk1 - asterisk0 - 1);
					{
						artworkPath.value = foundArtwork;
						artworkUploader.FileSelected(foundArtwork);
					}
				}
				// Remove artwork data from working string
				working = working.Remove(asterisk0, asterisk4 - asterisk0 + 1);
			}

		}


		// Get Card Text
		int levelUpInd = working.IndexOf("Level Up:");
		levelUpInd = Mathf.Max(working.Length - 1, 0);
		int cardTextStartIndex = working.IndexOf("\"", 0, levelUpInd);
		int cardTextEndIndex = working.LastIndexOf("\"", levelUpInd);

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
			cardText.value = "";
			cardTextF.SetTextWithoutNotify("");
		}

		// Get Title
		if (working.Contains(":"))
		{
			int manaIndex = working.IndexOf("Mana");
			int colonIndex = manaIndex > 0 ? working.Substring(0, manaIndex).LastIndexOf(":") : working.IndexOf(":");
			if (colonIndex < 0)
			{
				colonIndex = working.IndexOf(":");
			}
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


		// Get Card Type
		bool foundCardType = false;
		for (int i = cardTypeNames.Length - 1; i >= 0; i--)
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

		/* Determine whether Base or Leveled Champion
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
		*/


		// Get Level Up Text
		if (cardType.value == 1 || cardType.value == 2)
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
			levelUpTextF.text = "";
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


		// Get Group
		if (working.Contains("#"))
		{
			int groupIndex = working.IndexOf("#");
			string groupSubstring = working.Substring(groupIndex);
			working = working.Replace(groupSubstring, "");

			group.value = groupSubstring.Remove(0, 1);
			groupF.SetTextWithoutNotify(groupSubstring.Remove(0, 1));
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

				manaF.text = manaString;
			}
			else
			{
				mana.value = 0.ToString();
				working = working.Replace("mana", "");

				manaF.text = "0";
			}
		}

		// Get Attack/Health
		if (cardType.value < 3 || cardType.value == 8)
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

				attackF.text = attackString;
				healthF.text = healthString;

				// Remove
				working = working.Replace(attackString + "/" + healthString, "");
			}
		}

		// Find Keywords
		List<int> keywordIndexes = new List<int>();

		changeCardTypeEvent.Raise();

		int ik = 0;
		foreach (string key in keywordNames)
		{
			if (working.ToLower().Contains(key.ToLower().Replace(" ", "")))
			{				
				keywordIndexes.Add(ik);
				working = working.Replace(key.ToLower().Replace(" ", ""), "");
			}
			ik++;
		}

		

		foreach(int ki in keywordIndexes)
		{
			effectIntAdd.Raise(ki);
		}

		// Apply
		writeEvent.Invoke();
	}

	public void KeywordRecorder(int i)
	{

		if (keywordNames[i] == "Fast" || keywordNames[i] == "Burst" || keywordNames[i] == "Slow" || (cardType.value == 6 && keywordNames[i] == "Skill") || keywordNames[i] == "Landmark" || keywordNames[i] == "Focus")
		{
			return;
		}

		keywordRecord.Add(keywordNames[i]);
		ReadInput();
	}

	public void ClearKeywordRecord()
	{
		keywordRecord.Clear();
		ReadInput();

	}

	public void RemoveKeyword(int removeIndex)
	{
		string keywordName = keywordNames[removeIndex];

		for (int i = keywordRecord.Count - 1; i >= 0; i--)
		{
			if (keywordRecord[i] == keywordName)
			{
				keywordRecord.RemoveAt(i);
			}
		}
		ReadInput();

	}
}