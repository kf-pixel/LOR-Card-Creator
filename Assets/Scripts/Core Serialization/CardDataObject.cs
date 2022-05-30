using UnityEngine;

[System.Serializable]
public class CardDataObject
{
	public string id;
	public string cardName;
	public string cardCode = "Untitled: 1 Mana 1/1 Runeterra Follower *&local&/placeholder.png*155*0*640*";
	public string folder;

	public CardDataObject()
	{
		id = "";
		cardName = "";
		cardCode = "Untitled: 1 Mana 1/1 Runeterra Follower *&local&/placeholder.png*155*0*640*";
		folder = "";
	}

	public CardDataObject(string regionName = "Runeterra")
	{
		id = "";
		cardName = "";
		cardCode = $"Untitled: 1 Mana 1/1 {regionName} Common Follower *&local&/placeholder.png*155*0*640*";
		folder = "";
	}

	public CardDataObject(CardDataObject card)
	{
		id = "";
		cardName = "";
		cardCode = card.cardCode;
		folder = "";
	}

	public void GenerateID()
	{
		id = System.DateTime.Now.ToString();
	}

	public string GetCardType(string[] cardNamesList)
	{
		string foundCardType = "Follower";
		int foundIndex = 99;

		for (int i = cardNamesList.Length - 1; i >= 0 ; i--)
		{
			int stringIndex = cardCode.IndexOf(cardNamesList[i]);
			if (stringIndex < foundIndex && stringIndex > 0)
			{
				foundCardType = cardNamesList[i];
				foundIndex = stringIndex;
			}
		}

		if (foundCardType == "Champion")
		{
			foundCardType = "ChampionLVL1";
		}
		return foundCardType;
	}

	public int GetCardTypeIndex(string[] cardNamesList)
	{
		int foundIndex = 99;
		int cardIndex = 0;
		for (int i = cardNamesList.Length - 1; i >= 0; i--)
		{
			int ind = cardCode.IndexOf(cardNamesList[i]);
			if (ind < foundIndex && ind > 0)
			{
				foundIndex = ind;
				cardIndex = i;
			}
		}
		return cardIndex;
	}

	public string GetRegion(string[] regionNamesList)
	{
		string foundRegion = "Runeterra";
		int regionIndex = 0;
		int foundstringIndex = 99;
		for (int i = 0; i < regionNamesList.Length; i++)
		{
			int stringIndex = cardCode.IndexOf(regionNamesList[i]);
			if (stringIndex < foundstringIndex && stringIndex > 0)
			{
				foundRegion = regionNamesList[i];
				foundstringIndex = stringIndex;
				regionIndex = i;
			}
		}
		if (regionIndex >= 13)
		{
			foundRegion = "None";
		}
		return foundRegion;
	}

	public string GetRarity(string[] rarityNamesList)
	{
		string foundRarity = "None";
		int foundstringIndex = 99;
		for (int i = 0; i < rarityNamesList.Length; i++)
		{
			int stringIndex = cardCode.IndexOf(rarityNamesList[i]);
			if (stringIndex < foundstringIndex && stringIndex > 0)
			{
				foundRarity = rarityNamesList[i];
				foundstringIndex = stringIndex;
			}
		}

		return foundRarity;
	}

	public string GetSubtype()
	{
		string foundSubtype = "";
		int titleColon = cardCode.IndexOf(":");
		int hashIndex = cardCode.IndexOf("#", titleColon) + 1;
		int speechIndex = cardCode.IndexOf("\"", titleColon);
		int levelUpIndex = cardCode.IndexOf("Level Up:", titleColon);
		int asteriskIndex = cardCode.IndexOf("*", titleColon);

		if (hashIndex > 0)
		{
			if (speechIndex > 0)
			{
				foundSubtype = cardCode.Substring(hashIndex, Mathf.Clamp(speechIndex - hashIndex, 0, cardCode.Length - hashIndex)).Trim(' ');
			}
			else if (levelUpIndex > 0)
			{
				foundSubtype = cardCode.Substring(hashIndex, Mathf.Clamp(levelUpIndex - hashIndex, 0, cardCode.Length - hashIndex)).Trim(' ');
			}
			else if (asteriskIndex > 0)
			{
				foundSubtype = cardCode.Substring(hashIndex, Mathf.Clamp(asteriskIndex - hashIndex, 0, cardCode.Length - hashIndex)).Trim(' ');
			}
		}

		return foundSubtype;
	}

	public string GetMana()
	{
		// Get Mana
		int titleColon = cardCode.IndexOf(":");
		int manaIndex = cardCode.ToLower().IndexOf("mana", titleColon);

		// Find Numbers
		if (manaIndex > 2)
		{
			string manaString = cardCode.Substring(manaIndex - 3, 3).Replace(" ","");
			if (int.TryParse(manaString, out int parsed))
			{
				return manaString;
			}
		}

		return "-";
	}

	public void SetName()
	{
		if (!string.IsNullOrEmpty(cardCode))
		{
			int manaIndex = cardCode.IndexOf("Mana");
			int colonIndex = manaIndex > 0 ? cardCode.Substring(0, manaIndex).LastIndexOf(":") : cardCode.IndexOf(":");
			if (colonIndex < 0)
			{
				colonIndex = cardCode.IndexOf(":");
			}
			string titleSubstring = cardCode.Substring(0, colonIndex);

			if (colonIndex > 0)
			{
				cardName = StringExtensions.ToTitleCase(titleSubstring);
			}
			else
			{
				cardName = "Untitled";
			}
		}
		else
		{
			cardName = "Untitled";
		}
	}
}
