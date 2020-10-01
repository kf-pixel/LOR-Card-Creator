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
		cardCode = "Untitled: 1 Mana 1/1 " + regionName + " Follower *&local&/placeholder.png*155*0*640*";
		folder = "";
	}

	public void GenerateID(string suffix = "")
	{
		int randomIndex = Random.Range(100, 999);
		id = System.DateTime.Now.ToString() + suffix + randomIndex.ToString();
	}

	public string GetMana()
	{
		return null;
	}

	public string GetCardType(string[] cardNamesList)
	{
		string foundCardType = "Follower";
		int foundIndex = 99;
		foreach (string r in cardNamesList)
		{
			int i = cardCode.IndexOf(r);
			if (i < foundIndex && i > 0)
			{
				foundCardType = r;
				foundIndex = i;
			}
		}
		if (foundCardType == "Champion" && !cardCode.Contains("Level Up:"))
		{
			foundCardType = "Champion LVLUP";
		}
		return foundCardType;
	}

	public int GetCardTypeIndex(string[] cardNamesList)
	{
		int foundIndex = 99;
		int cardIndex = 0;
		for (int i = 0; i < cardNamesList.Length; i++)
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

	public void SetName()
	{
		if (!string.IsNullOrEmpty(cardCode))
		{
			int colonIndex = cardCode.IndexOf(":");
			if (colonIndex > 0)
			{
				cardName = cardCode.Substring(0, colonIndex);
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
