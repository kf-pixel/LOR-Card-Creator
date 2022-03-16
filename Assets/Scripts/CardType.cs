using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardType
{
	private static List<CardTypes> units = new List<CardTypes> { CardTypes.Follower,  CardTypes.Champion, CardTypes.ChampionLVL2, CardTypes.ChampionLVL3 };
	private static List<CardTypes> spells = new List<CardTypes> { CardTypes.Slow, CardTypes.Fast, CardTypes.Burst, CardTypes.Focus, CardTypes.Skill };
	private static List<CardTypes> champions = new List<CardTypes> { CardTypes.Champion, CardTypes.ChampionLVL2, CardTypes.ChampionLVL3 };
	private static List<CardTypes> rarity = new List<CardTypes> { CardTypes.Follower, CardTypes.Slow, CardTypes.Fast, CardTypes.Burst, CardTypes.Focus, CardTypes.Landmark, CardTypes.Champion };



	public static bool IsUnit(int i)
	{
		if (units.Contains((CardTypes)i)) { return true; }
		return false;
	}

	public static bool IsSpell(int i)
	{
		if (spells.Contains((CardTypes)i)) { return true; }
		return false;
	}

	public static bool IsChampion(int i)
	{
		if (champions.Contains((CardTypes)i)) { return true; }
		return false;
	}

	public static bool IsLandmark(int i)
	{
		if (i == 7) { return true; }
		return false;
	}

	public static bool HasRarity(int i)
	{
		if (rarity.Contains((CardTypes)i)) { return true; }
		return false;
	}

	public static string Name(int i)
	{
		return System.Enum.GetName(typeof(CardTypes), i);
	}
}

public enum CardTypes
{
	Follower,
	Champion,
	ChampionLVL2,
	Slow,
	Fast,
	Burst,
	Skill,
	Landmark,
	ChampionLVL3,
    Focus

}


