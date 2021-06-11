using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RiotAPI
{
	[System.Serializable]
	public class LOLChampionData : MonoBehaviour
	{

	}

	[System.Serializable]
	public class Image
	{
		public string full { get; set; }
		public string sprite { get; set; }
		public string group { get; set; }
		public int x { get; set; }
		public int y { get; set; }
		public int w { get; set; }
		public int h { get; set; }
	}

	[System.Serializable]
	public class Skin
	{
		public string id { get; set; }
		public int num { get; set; }
		public string name { get; set; }
		public bool chromas { get; set; }
	}

	[System.Serializable]
	public class Info
	{
		public int attack { get; set; }
		public int defense { get; set; }
		public int magic { get; set; }
		public int difficulty { get; set; }
	}

	[System.Serializable]
	public class Stats
	{
		public int hp { get; set; }
		public int hpperlevel { get; set; }
		public int mp { get; set; }
		public int mpperlevel { get; set; }
		public int movespeed { get; set; }
		public int armor { get; set; }
		public double armorperlevel { get; set; }
		public double spellblock { get; set; }
		public double spellblockperlevel { get; set; }
		public int attackrange { get; set; }
		public int hpregen { get; set; }
		public int hpregenperlevel { get; set; }
		public int mpregen { get; set; }
		public int mpregenperlevel { get; set; }
		public int crit { get; set; }
		public int critperlevel { get; set; }
		public int attackdamage { get; set; }
		public int attackdamageperlevel { get; set; }
		public double attackspeedperlevel { get; set; }
		public double attackspeed { get; set; }
	}

	[System.Serializable]
	public class Leveltip
	{
		public List<string> label { get; set; }
		public List<string> effect { get; set; }
	}

	[System.Serializable]
	public class Datavalues
	{
	}

	[System.Serializable]
	public class Spell
	{
		public string id { get; set; }
		public string name { get; set; }
		public string description { get; set; }
		public string tooltip { get; set; }
		public Leveltip leveltip { get; set; }
		public int maxrank { get; set; }
		public List<int> cooldown { get; set; }
		public string cooldownBurn { get; set; }
		public List<int> cost { get; set; }
		public string costBurn { get; set; }
		public Datavalues datavalues { get; set; }
		public List<List<int>> effect { get; set; }
		public List<string> effectBurn { get; set; }
		public List<object> vars { get; set; }
		public string costType { get; set; }
		public string maxammo { get; set; }
		public List<int> range { get; set; }
		public string rangeBurn { get; set; }
		public Image image { get; set; }
		public string resource { get; set; }
	}

	[System.Serializable]
	public class Passive
	{
		public string name { get; set; }
		public string description { get; set; }
		public Image image { get; set; }
	}

	[System.Serializable]
	public class Item
	{
		public string id { get; set; }
		public int count { get; set; }
		public bool hideCount { get; set; }
	}

	[System.Serializable]
	public class Block
	{
		public string type { get; set; }
		public bool recMath { get; set; }
		public bool recSteps { get; set; }
		public int minSummonerLevel { get; set; }
		public int maxSummonerLevel { get; set; }
		public string showIfSummonerSpell { get; set; }
		public string hideIfSummonerSpell { get; set; }
		public string appendAfterSection { get; set; }
		public List<string> visibleWithAllOf { get; set; }
		public List<string> hiddenWithAnyOf { get; set; }
		public List<Item> items { get; set; }
	}

	[System.Serializable]
	public class Recommended
	{
		public string champion { get; set; }
		public string title { get; set; }
		public string map { get; set; }
		public string mode { get; set; }
		public string type { get; set; }
		public string customTag { get; set; }
		public int sortrank { get; set; }
		public bool extensionPage { get; set; }
		public bool useObviousCheckmark { get; set; }
		public object customPanel { get; set; }
		public List<Block> blocks { get; set; }
	}

	[System.Serializable]
	public class Aatrox
	{
		public string id { get; set; }
		public string key { get; set; }
		public string name { get; set; }
		public string title { get; set; }
		public Image image { get; set; }
		public List<Skin> skins { get; set; }
		public string lore { get; set; }
		public string blurb { get; set; }
		public List<string> allytips { get; set; }
		public List<string> enemytips { get; set; }
		public List<string> tags { get; set; }
		public string partype { get; set; }
		public Info info { get; set; }
		public Stats stats { get; set; }
		public List<Spell> spells { get; set; }
		public Passive passive { get; set; }
		public List<Recommended> recommended { get; set; }
	}

	[System.Serializable]
	public class Data
	{
		public Aatrox champion { get; set; }
	}

	[System.Serializable]
	public class Root
	{
		public string type { get; set; }
		public string format { get; set; }
		public string version { get; set; }
		public Data data { get; set; }
	}

	//[System.Serializable]
	//public class Aatrox : ChampionData { }
	public class Ahri : Aatrox { }
	public class Akali : Aatrox { }
	public class Alistar : Aatrox { }
	public class Amumu : Aatrox { }
	public class Anivia : Aatrox { }
	public class Annie : Aatrox { }
	public class Aphelios : Aatrox { }
	public class Ashe : Aatrox { }
	public class AurelionSol : Aatrox { }
	public class Azir : Aatrox { }
	public class Bard : Aatrox { }
	public class Blitzcrank : Aatrox { }
	public class Brand : Aatrox { }
	public class Braum : Aatrox { }
	public class Caitlyn : Aatrox { }
	public class Camille : Aatrox { }
	public class Cassiopeia : Aatrox { }
	public class Chogath : Aatrox { }
	public class Corki : Aatrox { }
	public class Darius : Aatrox { }
	public class Diana : Aatrox { }
	public class Draven : Aatrox { }
	public class DrMundo : Aatrox { }
	public class Ekko : Aatrox { }
	public class Elise : Aatrox { }
	public class Evelynn : Aatrox { }
	public class Ezreal : Aatrox { }
	public class Fiddlesticks : Aatrox { }
	public class Fiora : Aatrox { }
	public class Fizz : Aatrox { }
	public class Galio : Aatrox { }
	public class Gangplank : Aatrox { }
	public class Garen : Aatrox { }
	public class Gnar : Aatrox { }
	public class Gragas : Aatrox { }
	public class Graves : Aatrox { }
	public class Hecarim : Aatrox { }
	public class Heimerdinger : Aatrox { }
	public class Illaoi : Aatrox { }
	public class Irelia : Aatrox { }
	public class Ivern : Aatrox { }
	public class Janna : Aatrox { }
	public class JarvanIV : Aatrox { }
	public class Jax : Aatrox { }
	public class Jayce : Aatrox { }
	public class Jhin : Aatrox { }
	public class Jinx : Aatrox { }
	public class Kaisa : Aatrox { }
	public class Kalista : Aatrox { }
	public class Karma : Aatrox { }
	public class Karthus : Aatrox { }
	public class Kassadin : Aatrox { }
	public class Katarina : Aatrox { }
	public class Kayle : Aatrox { }
	public class Kayn : Aatrox { }
	public class Kennen : Aatrox { }
	public class Kindred : Aatrox { }
	public class Khazix : Aatrox { }
	public class Kled : Aatrox { }
	public class KogMaw : Aatrox { }
	public class Leblanc : Aatrox { }
	public class LeeSin : Aatrox { }
	public class Leona : Aatrox { }
	public class Lillia : Aatrox { }
	public class Lissandra : Aatrox { }
	public class Lucian : Aatrox { }
	public class Lulu : Aatrox { }
	public class Lux : Aatrox { }
	public class Malphite : Aatrox { }
	public class Malzahar : Aatrox { }
	public class Maokai : Aatrox { }
	public class MasterYi : Aatrox { }
	public class MissFortune : Aatrox { }
	public class MonkeyKing : Aatrox { }
	public class Mordekaiser : Aatrox { }
	public class Morgana : Aatrox { }
	public class Nami : Aatrox { }
	public class Nasus : Aatrox { }
	public class Nautilus : Aatrox { }
	public class Neeko : Aatrox { }
	public class Nidalee : Aatrox { }
	public class Nocturne : Aatrox { }
	public class Nunu : Aatrox { }
	public class Olaf : Aatrox { }
	public class Orianna : Aatrox { }
	public class Ornn : Aatrox { }
	public class Pantheon : Aatrox { }
	public class Poppy : Aatrox { }
	public class Pyke : Aatrox { }
	public class Qiyana : Aatrox { }
	public class Quinn : Aatrox { }
	public class Rakan : Aatrox { }
	public class Rammus : Aatrox { }
	public class RekSai : Aatrox { }
	public class Rell : Aatrox { }
	public class Renekton : Aatrox { }
	public class Rengar : Aatrox { }
	public class Riven : Aatrox { }
	public class Rumble : Aatrox { }
	public class Ryze : Aatrox { }
	public class Samira : Aatrox { }
	public class Sejuani : Aatrox { }
	public class Senna : Aatrox { }
	public class Seraphine : Aatrox { }
	public class Sett : Aatrox { }
	public class Shaco : Aatrox { }
	public class Shen : Aatrox { }
	public class Shyvana : Aatrox { }
	public class Singed : Aatrox { }
	public class Sion : Aatrox { }
	public class Sivir : Aatrox { }
	public class Skarner : Aatrox { }
	public class Sona : Aatrox { }
	public class Soraka : Aatrox { }
	public class Swain : Aatrox { }
	public class Sylas : Aatrox { }
	public class Syndra : Aatrox { }
	public class TahmKench : Aatrox { }
	public class Taliyah : Aatrox { }
	public class Talon : Aatrox { }
	public class Taric : Aatrox { }
	public class Teemo : Aatrox { }
	public class Thresh : Aatrox { }
	public class Tristana : Aatrox { }
	public class Trundle : Aatrox { }
	public class Tryndamere : Aatrox { }
	public class TwistedFate : Aatrox { }
	public class Twitch : Aatrox { }
	public class Udyr : Aatrox { }
	public class Urgot : Aatrox { }
	public class Varus : Aatrox { }
	public class Vayne : Aatrox { }
	public class Veigar : Aatrox { }
	public class Velkoz : Aatrox { }
	public class Vi : Aatrox { }
	public class Viktor : Aatrox { }
	public class Vladimir : Aatrox { }
	public class Volibear : Aatrox { }
	public class Warwick : Aatrox { }
	public class Xayah : Aatrox { }
	public class Xerath : Aatrox { }
	public class XinZhao : Aatrox { }
	public class Yasuo : Aatrox { }
	public class Yone : Aatrox { }
	public class Yorick : Aatrox { }
	public class Yuumi : Aatrox { }
	public class Zac : Aatrox { }
	public class Zed : Aatrox { }
	public class Ziggs : Aatrox { }
	public class Ziliean : Aatrox { }
	public class Zoe : Aatrox { }
	public class Zyra : Aatrox { }
}