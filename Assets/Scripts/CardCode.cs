using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Text.RegularExpressions;

public class CardCode : MonoBehaviour
{
	[SerializeField] private TMP_InputField inputF;
	[SerializeField] private FileUpload artworkUploader;
	[SerializeField] private GameEvent changeCardTypeEvent;
	[SerializeField] private GameEvent clearKeywordsEvent;
	[SerializeField] private GameEvent updateNullSpeedEvent;
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
	[SerializeField] private SpellSpeedToggler nullSpeedToggler;

	[Header("Scriptable Object Data")]
	[SerializeField] private IntVariable cardType;
	[SerializeField] private IntVariable region, region2;
	[SerializeField] private IntVariable rarity;
	[SerializeField] private IntVariable nullSpeed;

	[SerializeField] private StringVariable mana;
	[SerializeField] private StringVariable attack;
	[SerializeField] private StringVariable health;
	[SerializeField] private StringVariable title;
	[SerializeField] private StringVariable group;
	[SerializeField] private StringVariable cardText;
	[SerializeField] private StringVariable levelUp;
	[SerializeField] private StringVariable credit;

	[Header("Events")]
	[SerializeField] private StringEvent keywordValueAddEvent;

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
	private string integerRegexPattern = "[^a-z ]\\ *([.0-9])*\\d";

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
		if (CardType.IsUnit(cardType.value))
		{
			code += $"{attack.value}/{health.value} ";
		}

		// Region
		code += regionNames[region.value] + " ";

		// Rarity
		if (CardType.HasRarity(cardType.value))
		{
			if (CardType.IsChampion(cardType.value))
			{
				if (rarity.value == 4)
				{
					code += "NonCollectable ";
				}
			}
			else if (rarity.value != 0)
			{
				code += rarityNames[rarity.value] + " ";
			}
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
		if (!CardType.IsSpell(cardType.value) && !string.IsNullOrEmpty(levelUp.value))
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
		code += $" &~{artworkShadowScale.value} ";

		// Region 2
		if (region2.value != 13)
		{
			code += $"++{regionNames[region2.value]} ";
		}

		// NullSpellSpeed
		if (nullSpeed.value == 1)
		{
			code += "//NULLSPELLSPEED//";
		}

		// Apply
		inputF.SetTextWithoutNotify(code);

		// Raise Event
		cardReadEvent.Raise();
	}

	public void WriteInput(string originalInputString)
	{
		KeywordClear();
		string working = originalInputString;

		// Clear Existing Keywords
		clearKeywordsEvent.Raise();

		// Get NullSpeedSpeed
		if (working.Contains("//NULLSPELLSPEED//"))
		{
			nullSpeed.value = 1;
			nullSpeedToggler.SetTrue();
			working = working.Replace("//NULLSPELLSPEED//", "");
		}
		else
		{
			nullSpeed.value = 0;
			nullSpeedToggler.SetFalse();
		}

		// Get Region 2
		int region2Index = working.IndexOf(" ++", 0);
		if (region2Index > 1)
		{
			string region2String = working.Substring(region2Index);
			for (int i = 0; i < regionNames.Length; i++)
			{
				if (region2String.Contains(regionNames[i]))
				{
					working = working.Remove(region2Index);
					region2.value = i;
					break;
				}
			}
		}
		else
		{
			region2.value = 13;
		}

		// Get Shadow Scale
		int shadowStartIndex = working.IndexOf("&~", 0);
		if (shadowStartIndex > 1)
		{
			string shadowString = working.Substring(shadowStartIndex);
			int shadowEndIndex = shadowString.IndexOf(" ") + shadowStartIndex;
			if (shadowString.IndexOf(" ") < 0)
			{
				shadowEndIndex = working.Length;
			}

			Match shadowIntegersMatch = Regex.Match(shadowString, integerRegexPattern);
			if (shadowIntegersMatch.Success)
			{
				int.TryParse(shadowIntegersMatch.Value, out int shadowvalueInt);
				artworkShadowScale.value = shadowvalueInt;
				working = working.Remove(shadowStartIndex, shadowEndIndex - shadowStartIndex);
			}
			else
			{
				artworkShadowScale.value = -400;
			}
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

		// Get Card Text
		int levelUpInd = working.IndexOf("Level Up:");
		if (levelUpInd < 0)
		{
			levelUpInd = working.Length;
		}
		int cardTextStartIndex = working.IndexOf("\"", 0, levelUpInd);
		int cardTextEndIndex = working.LastIndexOf("\"", levelUpInd);

		if (cardTextStartIndex > 0 && cardTextEndIndex > 0)
		{
			string cardTextString = working.Substring(cardTextStartIndex, cardTextEndIndex - cardTextStartIndex + 1);

			working = working.Replace(cardTextString, "");

			cardTextString = cardTextString.Substring(1, cardTextString.Length - 2);
			cardText.value = cardTextString;
			cardTextF.SetTextWithoutNotify(cardTextString);
		}
		else
		{
			cardText.value = "";
			cardTextF.SetTextWithoutNotify("");
		}


		// Get Card Type
		bool foundCardType = false;
		for (int i = cardTypeNames.Length - 1; i >= 0; i--)
		{
			int levelUpIndex = working.ToLower().IndexOf("level up:");
			if (levelUpIndex == -1)
			{
				levelUpIndex = working.Length;
			}
			if (working.Substring(0, levelUpIndex).ToLower().Contains(cardTypeNames[i].ToLower()))
			{
				foundCardType = true;
				cardType.value = i;

				int cardTypeIndex = working.IndexOf(cardTypeNames[i]);
				working = working.Remove(cardTypeIndex, cardTypeNames[i].Length);
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

		// Get Level Up Text
		if (!CardType.IsSpell(cardType.value))
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
			else
			{
				levelUp.value = "";
				levelUpTextF.text = "";
			}
		}
		else
		{
			levelUp.value = "";
			levelUpTextF.text = "";
		}

		// Get Subtype
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
		if (CardType.HasRarity(cardType.value))
		{
			if (!CardType.IsChampion(cardType.value))
			{
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
			}
			else // champion rarity
			{
				if (working.ToLower().Contains("noncollectable"))
				{
					rarity.value = 4;
					working = working.Replace("noncollectable", "");
					working = working.Replace("NonCollectable", "");
				}
				else
				{
					rarity.value = 0;
				}
			}
		}
		else
		{
			rarity.value = 0;
		}


	

		// Remove Spaces, get Keywords, Mana, then stats
		working = working.ToLower();
		working = working.Replace(" ", "");

		// Change card type before keywords reset
		changeCardTypeEvent.Raise();

		// Find && add Keywords
		for (int i = 0; i < keywordNames.Count; i++)
		{
			string key = keywordNames[i].ToLower().Replace(" ", "");

			Match match = Regex.Match(working, $"{key}\\d?\\d?");
			if (match.Success)
			{
				int valueDigits = match.Length - key.Length;
				if (valueDigits == 0)
				{
					keywordValueAddEvent.Raise($"{keywordNames[i]},0");
					keywordRecord.Add(($"{keywordNames[i]}0"));
				}
				else
				{
					keywordValueAddEvent.Raise($"{keywordNames[i]},{match.Value.Substring(key.Length)}");
					keywordRecord.Add(($"{keywordNames[i]}{match.Value.Substring(key.Length)}"));
				}

				working = working.Replace(match.Value, "");
			}
		}
		
		// Get Mana
		if (working.ToLower().Contains("mana"))
		{
			int manaIndex = working.ToLower().LastIndexOf("mana");
			string manaString = working.Substring(0, manaIndex);

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
		else
		{
			mana.value = "";
			manaF.text = "";
			manaF.SetTextWithoutNotify("");
		}

		// Get Attack/Health
		if (CardType.IsUnit(cardType.value))
		{
			int slashIndex = working.IndexOf("/");
			if (slashIndex >= 0)
			{
				string attackString = working.Substring(0, slashIndex);
				string healthString = working.Substring(slashIndex + 1);

				// Apply
				attack.value = attackString;
				health.value = healthString;

				attackF.text = attackString;
				healthF.text = healthString;
			}
			else
			{
				attack.value = "";
				health.value = "";
				attackF.SetTextWithoutNotify("");
				healthF.SetTextWithoutNotify("");
			}
		}
		else
		{
			attack.value = "";
			health.value = "";
			attackF.SetTextWithoutNotify("");
			healthF.SetTextWithoutNotify("");
		}

		
		// Invoke Finishing Events
		writeEvent.Invoke();
	}

	public void KeywordValueRecorder(string keyvalue)
	{
		string[] keypair = keyvalue.Split(',');

		for (int i = 0; i < keywordRecord.Count; i++)
		{
			if (keywordRecord[i].StartsWith(keypair[0]))
			{
				if (keypair[1] == "0")
				{
					keywordRecord[i] = keypair[0];
				}
				else
				{
					keywordRecord[i] = keypair[0] + keypair[1];
				}
				ReadInput();
			}
		}
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


	public void RemoveKeyword(int removeIndex)
	{
		string keywordName = keywordNames[removeIndex];

		for (int i = keywordRecord.Count - 1; i >= 0; i--)
		{
			if (keywordRecord[i].StartsWith(keywordName))
			{
				keywordRecord.RemoveAt(i);
			}
		}
		ReadInput();
	}

	public void KeywordClear()
	{
		keywordRecord.Clear();
	}

	public void ClearKeywordRecord()
	{
		keywordRecord.Clear();
		ReadInput();
	}
}