using System.Collections;
using System.Collections.Generic;

namespace LORAPI
{
	[System.Serializable]
	public class LORCard
	{
		public List<string> associatedCards;
		public List<string> associatedCardRefs;
		public List<Asset> assets;
		public string region;
		public string regionRef;
		public List<string> regions;
		public List<string> regionRefs;
		public int attack;
		public int cost;
		public int health;
		public string description;
		public string descriptionRaw;
		public string levelupDescription;
		public string levelupDescriptionRaw;
		public string flavorText;
		public string artistName;
		public string name;
		public string cardCode;
		public List<string> keywords;
		public List<string> keywordRefs;
		public string spellSpeed;
		public string spellSpeedRef;
		public string rarity;
		public string rarityRef;
		public string subtype;
		public List<string> subtypes;
		public string supertype;
		public string type;
		public bool collectible;
		public string set;
	}

	[System.Serializable]
	public class Asset
	{
		public string gameAbsolutePath;
		public string fullAbsolutePath;
	}

	[System.Serializable]
	public class LORCardSet
	{
		public List<LORCard> cards;
	}
}