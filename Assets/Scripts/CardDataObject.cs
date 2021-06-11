using System.Collections;
using System.Collections.Generic;
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
		cardCode = "Untitled: 1 Mana 1/1 " + regionName + " Common Follower *&local&/placeholder.png*155*0*640*";
		folder = "";
	}

	public CardDataObject(CardDataObject card)
	{
		id = "";
		cardName = "";
		cardCode = card.cardCode;
		folder = "";
	}

	public void GenerateID(string suffix = "")
	{
		int randomIndex = Random.Range(100, 999);
		id = System.DateTime.Now.ToString() + suffix + randomIndex.ToString();
	}

	public string GetCardType(string[] cardNamesList)
	{
		string foundCardType = "Follower";
		int foundIndex = 99;
		/*
		foreach (string r in cardNamesList)
		{
			int i = cardCode.IndexOf(r);
			if (i < foundIndex && i > 0)
			{
				foundCardType = r;
				foundIndex = i;
			}
		}
		*/

		for (int i = cardNamesList.Length - 1; i >= 0 ; i--)
		{
			int stringIndex = cardCode.IndexOf(cardNamesList[i]);
			if (stringIndex < foundIndex && stringIndex > 0)
			{
				foundCardType = cardNamesList[i];
				foundIndex = stringIndex;
			}
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
		int foundIndex = 99;
		foreach(string r in regionNamesList)
		{
			int i = cardCode.IndexOf(r);
			if (i < foundIndex && i > 0)
			{
				foundRegion = r;
				foundIndex = i;
			}
		}

		return foundRegion;
	}

	public string GetMana()
	{
		// Get Mana
		string manaString = "";
		int titleColon = cardCode.IndexOf(":");
		int manaIndex = cardCode.ToLower().IndexOf("mana", titleColon);

		// Find Numbers
		if (manaIndex > 2)
		{
			manaString = cardCode.Substring(manaIndex - 3, 3).Replace(" ","");
			return manaString;
		}

		return " ";
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
				cardName = titleSubstring;
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
