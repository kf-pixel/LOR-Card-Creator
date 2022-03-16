using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LORAPI;
using System.Linq;

namespace LORAPI
{
	public class LORApiDataRetriever : MonoBehaviour
	{
		private LORApiHandler lor;
		public List<CardCondition> searchConditions;
		public CardDataType resultData;
		public CardResultType resultType;

		public bool PassesAll(LORCard card, List<CardCondition> conditions)
		{
			foreach (CardCondition cond in conditions)
			{
				if (!cond.Pass(card)) return false;
			}
			return true;
		}

		[ContextMenu("Get Results")]
		public void GetResults()
		{
			int sample = 0;
			float mean = 0f;

			if (resultType == CardResultType.PrintOut)
			{
				foreach (LORCard card in lor.allCards.cards)
				{
					if (PassesAll(card, searchConditions))
					{
						sample++;
						switch (resultData)
						{
							case CardDataType.Name:
								Debug.Log(card.name);
								break;
							case CardDataType.Region:
								foreach (string r in card.regions) Debug.Log(r);
								break;
							case CardDataType.Mana:
								Debug.Log(card.cost);
								break;
							case CardDataType.Power:
								Debug.Log(card.attack);
								break;
							case CardDataType.Health:
								Debug.Log(card.health);
								break;
							case CardDataType.Text:
								Debug.Log(card.descriptionRaw);
								break;
							case CardDataType.LevelUpText:
								Debug.Log(card.levelupDescriptionRaw);
								break;
							case CardDataType.FlavorText:
								Debug.Log(card.flavorText);
								break;
							case CardDataType.Rarity:
								Debug.Log(card.rarity);
								break;
							case CardDataType.Keyword:
								foreach (string k in card.keywords) Debug.Log(k);
								break;
							case CardDataType.Subtype:
								Debug.Log(card.subtype);
								break;
							case CardDataType.Type:
								Debug.Log(card.type);
								break;
							case CardDataType.Supertype:
								Debug.Log(card.supertype);
								break;
							case CardDataType.Collectible:
								Debug.Log(card.collectible);
								break;
							case CardDataType.Set:
								Debug.Log(card.set);
								break;
							default: break;
						}
					}
				}
				Debug.Log($"Total Sample Size: {sample}");
			}
			else if (resultType == CardResultType.AverageMean)
			{
				foreach (LORCard card in lor.allCards.cards)
				{
					if (PassesAll(card, searchConditions))
					{
						sample++;
						switch (resultData)
						{
							case CardDataType.Name:
								//Debug.Log(card.name);
								break;
							case CardDataType.Region:
								//foreach (string r in card.regions) Debug.Log(r);
								break;
							case CardDataType.Mana:
								mean += card.cost;
								break;
							case CardDataType.Power:
								if (card.type == "Unit")
								{
									mean += card.attack;
								}
								else sample--;
								break;
							case CardDataType.Health:
								if (card.type == "Unit")
								{
									mean += card.health;
								}
								else sample--;
								sample--;
								break;
							case CardDataType.Text:
								//Debug.Log(card.descriptionRaw);
								break;
							case CardDataType.LevelUpText:
								//Debug.Log(card.levelupDescriptionRaw);
								break;
							case CardDataType.FlavorText:
								//Debug.Log(card.flavorText);
								break;
							case CardDataType.Rarity:
								//Debug.Log(card.rarity);
								break;
							case CardDataType.Keyword:
								//foreach (string k in card.keywords) Debug.Log(k);
								break;
							case CardDataType.Subtype:
								//Debug.Log(card.subtype);
								break;
							case CardDataType.Type:
								//Debug.Log(card.type);
								break;
							case CardDataType.Supertype:
								//Debug.Log(card.supertype);
								break;
							case CardDataType.Collectible:
								//Debug.Log(card.collectible);
								break;
							case CardDataType.Set:
								//Debug.Log(card.set);
								break;
							default: break;
						}
					}
				}

				Debug.Log(mean/sample);
				Debug.Log($"Total Sample Size: {sample}");
			}
		}

		private void Awake()
		{
			lor = FindObjectOfType<LORApiHandler>();
		}

		[ContextMenu("Get Data")]
		public void GetData()
		{
			/* AVERAGE MEAN
			float mana = 0;
			float health = 0;
			float power = 0;
			int count = 0;

			foreach (LORCard card in lor.allCards.cards)
			{
				//if (card.supertype == "Champion" && card.collectible == true) // level 1 champs
				if (card.supertype == "Champion" && card.collectible == false && card.type == "Unit") // level >1 champs
				{
					count++;
					mana += card.cost;
					power += card.attack;
					health += card.health;
				}
			}

			Debug.Log($"Average Mana: {mana / count}");
			Debug.Log($"Average Power: {power / count} ");
			Debug.Log($"Average Health: {health / count} ");
			Debug.Log($"Sample: {count}");
			*/

			/* AVERAGE MEDIAN
			List<int> mana = new List<int>();
			List<int> power = new List<int>();
			List<int> health = new List<int>();
			int count = 0;

			foreach (LORCard card in lor.allCards.cards)
			{
				//if (card.supertype == "Champion" && card.collectible == false && card.type == "Unit") // level >1 champs
				if (card.supertype == "Champion" && card.collectible == true) // level 1 champs
				{
					count++;
					mana.Add(card.cost);
					power.Add(card.attack);
					health.Add(card.health);
					Debug.Log(card.attack);
				}
			}

			int median = Mathf.RoundToInt(count / 2);

			mana.Sort();
			power.Sort();
			health.Sort();

			Debug.Log($"Median Mana: {mana[median]}");
			Debug.Log($"Median Power: {power[median]} ");
			Debug.Log($"Median Health: {health[median]} ");
			Debug.Log($"Sample: {count}");

			*/


			/*
			int set1count = 0;
			int set2count = 0;
			int set3count = 0;
			int set4count = 0;
			int set5count = 0;

			float set1 = 0;
			float set2 = 0;
			float set3 = 0;
			float set4 = 0;
			float set5 = 0;

			foreach (LORCard card in lor.allCards.cards)
			{
				//if (card.supertype == "Champion" && card.collectible == false && card.type == "Unit") // level >1 champs
				if (card.type == "Unit" && card.collectible == true) // level 1 champs
				{
					if (card.cost == 0) continue;
					if (card.set == "Set1")
					{
						set1count++;
						set1 += GetStatEfficiency(card.cost, card.attack, card.health);
					}
					else if (card.set == "Set2")
					{
						set2count++;
						set2 += GetStatEfficiency(card.cost, card.attack, card.health);
					}
					else if (card.set == "Set3")
					{
						set3count++;
						set3 += GetStatEfficiency(card.cost, card.attack, card.health);
					}
					else if (card.set == "Set4")
					{
						set4count++;
						set4 += GetStatEfficiency(card.cost, card.attack, card.health);
					}
					else if (card.set == "Set5")
					{
						set5count++;
						set5 += GetStatEfficiency(card.cost, card.attack, card.health);
					}
				}
			}

			Debug.Log($"Mean Set1 StatEfficiency: {set1 / set1count} points per mana. Sample: {set1count}");
			Debug.Log($"Mean Set2 StatEfficiency: {set2 / set2count} points per mana. Sample: {set2count}");
			Debug.Log($"Mean Set3 StatEfficiency: {set3 / set3count} points per mana. Sample: {set3count}");
			Debug.Log($"Mean Set4 StatEfficiency: {set4 / set4count} points per mana. Sample: {set4count}");
			Debug.Log($"Mean Set5 StatEfficiency: {set5 / set5count} points per mana. Sample: {set5count}");
			*/
		}

		public float GetStatEfficiency(int mana, int power, int health)
		{
			if (mana == 0) return 1;
			return ((power + health) / mana);
		}
	}

	[System.Serializable]
	public class CardCondition
	{
		public CardDataType data;
		public CardConditionType condition;
		public string input;

		public bool Pass (LORCard card)
		{
			string inputUpper = input.ToUpper();
			int inputInt = -1;
			bool inputBool = inputUpper == "TRUE" ? true : false;
			int.TryParse(input, out inputInt);

			switch (data)
			{
				case CardDataType.Name:
					if (condition == CardConditionType.Contains)
					{
						if (card.name.Contains(input)) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.name.Contains(input)) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.name == input) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						if (card.name.Length > inputInt) return true;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						if (card.name.Length < inputInt) return true;
					}
					break;
				case CardDataType.Region:
					if (condition == CardConditionType.Contains)
					{
						if (card.regions.Contains(input)) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.regions.Contains(input)) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.regions.Contains(input)) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						return false;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						return false;
					}
					break;
				case CardDataType.Mana:
					if (condition == CardConditionType.Contains)
					{
						if (card.cost == inputInt) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (card.cost != inputInt) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.cost == inputInt) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						if (card.cost > inputInt) return true;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						if (card.cost < inputInt) return true;
					}
					break;
				case CardDataType.Power:
					if (condition == CardConditionType.Contains)
					{
						if (card.attack == inputInt) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (card.attack != inputInt) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.attack == inputInt) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						if (card.attack > inputInt) return true;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						if (card.attack < inputInt) return true;
					}
					break;
				case CardDataType.Health:
					if (condition == CardConditionType.Contains)
					{
						if (card.health == inputInt) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (card.health != inputInt) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.health == inputInt) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						if (card.health > inputInt) return true;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						if (card.health < inputInt) return true;
					}
					break;
				case CardDataType.Text:
					if (condition == CardConditionType.Contains)
					{
						if (card.descriptionRaw.Contains(input)) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.descriptionRaw.Contains(input)) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.descriptionRaw == input) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						if (card.descriptionRaw.Length > inputInt) return true;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						if (card.descriptionRaw.Length < inputInt) return true;
					}
					break;
				case CardDataType.LevelUpText:
					if (condition == CardConditionType.Contains)
					{
						if (card.levelupDescriptionRaw.Contains(input)) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.levelupDescriptionRaw.Contains(input)) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.levelupDescriptionRaw == input) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						if (card.levelupDescriptionRaw.Length > inputInt) return true;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						if (card.levelupDescriptionRaw.Length < inputInt) return true;
					}
					break;
				case CardDataType.FlavorText:
					if (condition == CardConditionType.Contains)
					{
						if (card.flavorText.Contains(input)) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.flavorText.Contains(input)) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.flavorText == input) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						if (card.flavorText.Length > inputInt) return true;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						if (card.levelupDescriptionRaw.Length < inputInt) return true;
					}
					break;
				case CardDataType.Keyword:
					if (condition == CardConditionType.Contains)
					{
						if (card.keywords.Contains(input)) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.keywords.Contains(input)) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.keywords.Contains(input) && card.keywords.Count == 1) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						if (card.keywords.Count > inputInt) return true;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						if (card.keywords.Count < inputInt) return true;
					}
					break;
				case CardDataType.Rarity:
					if (condition == CardConditionType.Contains)
					{
						if (card.rarity.Contains(inputUpper)) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.rarity.Contains(inputUpper)) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.rarity.Contains(inputUpper)) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						if (inputUpper.Contains("NONE"))
						{
							if (card.rarity.Contains("COMMON") || card.rarity.Contains("RARE") || card.rarity.Contains("EPIC") || card.rarity.Contains("Champion")) return true;
						}
						else if (inputUpper.Contains("COMMON"))
						{
							if (card.rarity.Contains("RARE") || card.rarity.Contains("EPIC") || card.rarity.Contains("Champion")) return true;
						}
						else if (inputUpper.Contains("RARE"))
						{
							if (card.rarity.Contains("EPIC") || card.rarity.Contains("Champion")) return true;
						}
						else if (inputUpper.Contains("EPIC"))
						{
							if (card.rarity.Contains("Champion")) return true;
						}
					}
					else if (condition == CardConditionType.LesserThan)
					{
						if (inputUpper.Contains("CHAMPION"))
						{
							if (card.rarity.Contains("COMMON") || card.rarity.Contains("RARE") || card.rarity.Contains("EPIC") || card.rarity.Contains("None")) return true;
						}
						else if (inputUpper.Contains("EPIC"))
						{
							if (card.rarity.Contains("RARE") || card.rarity.Contains("COMMON") || card.rarity.Contains("None")) return true;
						}
						else if (inputUpper.Contains("RARE"))
						{
							if (card.rarity.Contains("COMMON") || card.rarity.Contains("None")) return true;
						}
						else if (inputUpper.Contains("COMMON"))
						{
							if (card.rarity.Contains("None")) return true;
						}
					}
					break;
				case CardDataType.Subtype:
					if (condition == CardConditionType.Contains)
					{
						if (card.subtype.Contains(inputUpper)) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.subtype.Contains(inputUpper)) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.subtype.Contains(inputUpper)) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						return false;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						return false;
					}
					break;
				case CardDataType.Type:
					if (condition == CardConditionType.Contains)
					{
						if (card.type.Contains(input)) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.type.Contains(input)) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.type.Contains(input)) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						return false;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						return false;
					}
					break;
				case CardDataType.Supertype:
					if (condition == CardConditionType.Contains)
					{
						if (card.supertype.Contains(input)) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.supertype.Contains(input)) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.supertype.Contains(input)) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						return false;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						return false;
					}
					break;
				case CardDataType.Collectible:
					if (condition == CardConditionType.Contains)
					{
						if (card.collectible == inputBool) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.collectible == inputBool) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.collectible == inputBool) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						return false;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						return false;
					}
					break;
				case CardDataType.Set:
					if (condition == CardConditionType.Contains)
					{
						if (card.set.EndsWith(input)) return true;
					}
					else if (condition == CardConditionType.Excludes)
					{
						if (!card.set.EndsWith(input)) return true;
					}
					else if (condition == CardConditionType.EqualTo)
					{
						if (card.set.EndsWith(input)) return true;
					}
					else if (condition == CardConditionType.GreaterThan)
					{
						return false;
					}
					else if (condition == CardConditionType.LesserThan)
					{
						return false;
					}
					break;
				default:
					return false;
			}
			return false;
		}
}

	public enum CardDataType
	{
		Name,
		Region,
		Mana,
		Power,
		Health,
		Text,
		LevelUpText,
		FlavorText,
		Keyword,
		Rarity,
		Subtype,
		Type,
		Supertype,
		Collectible,
		Set

	}

	public enum CardConditionType
	{
		Contains,
		Excludes,
		EqualTo,
		GreaterThan,
		LesserThan
	}

	public enum CardResultType
	{
		PrintOut,
		AverageMean,
		AverageMedian
	}
}