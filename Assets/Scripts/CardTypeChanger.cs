using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardTypeChanger : MonoBehaviour
{
	[SerializeField] private Image img;
	[SerializeField] private Image spellCardFrame;
	[SerializeField] private Image[] rarityImages;
	[SerializeField] private SpritesVariable[] spritesPerCardType;
	[SerializeField] private IntVariable cardTypeIndex;
	[SerializeField] private IntVariable cardRarityIndex;
	[SerializeField] private int defaultIndex = 1;
	[SerializeField] private IntVariable nullSpellSpeed;
	private int prior_i;

	[Header("Tribes")]
	[SerializeField] private SpritesVariable tribeSprites;
	[SerializeField] private Image currentTribeSprite;

	[Header("Events")]
	[SerializeField] private GameEvent clearKeywordsEvent;
	[SerializeField] private UnityEvent cardChangeEvent;
	[SerializeField] private UnityEvent championBaseCardEvent, championLevel2Event, championLevel3Event;
	[SerializeField] private UnityEvent nonChampionBaseCardEvent;
	[SerializeField] private UnityEvent unitCardEvent, spellCardEvent;
	[SerializeField] private UnityEvent slowSpellEvent;
	[SerializeField] private UnityEvent fastSpellEvent;
	[SerializeField] private UnityEvent burstSpellEvent;
	[SerializeField] private UnityEvent focusSpellEvent;

	[SerializeField] private UnityEvent showRarityEvent;
	[SerializeField] private UnityEvent hideRarityEvent;
	[SerializeField] private UnityEvent championRarityEvent;
	[SerializeField] private UnityEvent skillEvent;
	[SerializeField] private UnityEvent landmarkEvent;
	[SerializeField] private UnityEvent nonLandmarkEvent;
	private bool parsedThisFrame;

	private void Start()
	{
		prior_i = defaultIndex;
		cardTypeIndex.value = defaultIndex;
		ParseType();
	}
	private void LateUpdate()
	{
		parsedThisFrame = false;
	}

	public void ParseType()
	{
		if (parsedThisFrame) return;
		parsedThisFrame = true;

		int i = cardTypeIndex.value;

		// clear keywords if going from unit > spell or vice versa
		if (CardType.IsUnit(prior_i) != CardType.IsUnit(i) || CardType.IsLandmark(prior_i) || CardType.IsLandmark(i))
		{
			clearKeywordsEvent.Raise();
		}

		// Switch depending on card type
		if ((CardTypes)i == CardTypes.Follower)
		{
			nonChampionBaseCardEvent.Invoke();
		}
		else if ((CardTypes)i == CardTypes.Champion)
		{
			championBaseCardEvent.Invoke();
		}
		else if ((CardTypes)i == CardTypes.ChampionLVL2)
		{
			championBaseCardEvent.Invoke();
			championLevel2Event.Invoke();
		}
		else if ((CardTypes)i == CardTypes.ChampionLVL3)
		{
			nonChampionBaseCardEvent.Invoke();
			championLevel3Event.Invoke();
		}

		// landmark
		if (CardType.IsLandmark(i))
		{
			nonChampionBaseCardEvent.Invoke();
			landmarkEvent.Invoke();
		}
		else
		{
			nonLandmarkEvent.Invoke();
		}

		// unit + landmark or spell
		if (CardType.IsUnit(i) || CardType.IsLandmark(i))
		{
			unitCardEvent.Invoke();
		}
		else
		{
			spellCardEvent.Invoke();
		}

		// Show rarity dropdown
		if (CardType.HasRarity(i))
		{
			if (CardType.IsChampion(i))
			{
				championRarityEvent.Invoke();
			}
			else
			{
				showRarityEvent.Invoke();
			}
		}
		else
		{
			hideRarityEvent.Invoke();
		}

		// Set Tribe Sprite Type
		if (CardType.IsUnit(i)) // if non-spell card
		{
			if ((CardTypes)i == CardTypes.Follower)
			{
				currentTribeSprite.sprite = tribeSprites.values[0];
			}
			else
			{
				currentTribeSprite.sprite = tribeSprites.values[1];
			}
		}

		// Reset NullSpellSpeed flag
		if (!CardType.IsSpell(i))
		{
			nullSpellSpeed.value = 0;
		}

		// Add some effect keywords on spells
		if ((CardTypes)i == CardTypes.Slow)
		{
			slowSpellEvent.Invoke();
		}
		else if ((CardTypes)i == CardTypes.Fast)
		{
			fastSpellEvent.Invoke();
		}
		else if ((CardTypes)i == CardTypes.Burst)
		{
			burstSpellEvent.Invoke();
		}
		else if ((CardTypes)i == CardTypes.Skill)
		{
			skillEvent.Invoke();
		}
		else if ((CardTypes)i == CardTypes.Focus)
		{
			focusSpellEvent.Invoke();
		}

		// Change Card Frame Sprite && rarity
		if (cardRarityIndex.value > 3 && cardTypeIndex.value != 1) // reset rarity index if previously was on noncollectable champ's index (4)
		{
			cardRarityIndex.value = 0;
		}
		if (CardType.IsChampion(i) && !CardType.HasRarity(i)) // champion sprites without rarity
		{
			img.sprite = spritesPerCardType[i].values[cardRarityIndex.value];
			foreach (Image ri in rarityImages)
			{
				ri.enabled = false;
			}
		}
		else // rarity cards
		{
			img.sprite = spritesPerCardType[i].values[0];
			spellCardFrame.sprite = spritesPerCardType[i].values[0];

			foreach (Image ri in rarityImages)
			{
				if (CardType.IsChampion(i))
				{
					if (cardRarityIndex.value == 4)
					{
						ri.enabled = false;
					}
					else
					{
						ri.enabled = true;
						ri.sprite = spritesPerCardType[i].values[1];
					}
				}
				else // non champ rarity
				{
					if (cardRarityIndex.value > 0)
					{
						ri.enabled = true;
						ri.sprite = spritesPerCardType[i].values[cardRarityIndex.value];
					}
					else
					{
						ri.enabled = false;
					}
				}
			}
		}

		// Invoke Change card event
		cardChangeEvent.Invoke();
		prior_i = i;
	}
}