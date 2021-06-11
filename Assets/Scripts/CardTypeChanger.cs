using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardTypeChanger : MonoBehaviour
{
	[SerializeField] private Image img;
	[SerializeField] private Image spellCardFrame;
	[SerializeField] private Image focusImage;
	[SerializeField] private Image[] rarityImages;
	[SerializeField] private SpritesVariable[] spritesPerCardType;
	[SerializeField] private IntVariable cardTypeIndex;
	[SerializeField] private IntVariable cardRarityIndex;
	[SerializeField] private int defaultIndex = 1;
	private int previousCardType;

	[Header("Tribes")]
	[SerializeField] private SpritesVariable tribeSprites;
	[SerializeField] private Image currentTribeSprite;

	[Header("Events")]
	[SerializeField] private GameEvent clearKeywordsEvent;
	[SerializeField] private UnityEvent cardChangeEvent;
	[SerializeField] private UnityEvent championBaseCardEvent, championLevel2Event, championLevel3Event;
	[SerializeField] private UnityEvent nonChampionBaseCardEvent;

	[SerializeField] private UnityEvent spellCardEvent;
	[SerializeField] private UnityEvent slowSpellEvent;
	[SerializeField] private UnityEvent fastSpellEvent;
	[SerializeField] private UnityEvent burstSpellEvent;
	[SerializeField] private UnityEvent focusSpellEvent;

	[SerializeField] private UnityEvent showRarityEvent;
	[SerializeField] private UnityEvent hideRarityEvent;

	[SerializeField] private UnityEvent skillEvent;

	[SerializeField] private UnityEvent landmarkEvent;
	[SerializeField] private UnityEvent nonLandmarkEvent;
	private bool parsedThisFrame;

	// List of card type categories
	private List<int> unitsT = new List<int>(4) { 0, 1, 2, 8 };
	private List<int> nonUnitsT = new List<int>(6) { 3, 4, 5, 6, 7, 9 };

	private void Start()
	{
		previousCardType = defaultIndex;
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

		// clear keywords if going from unit > spell or vice versa
		if (unitsT.Contains(previousCardType) != unitsT.Contains(cardTypeIndex.value))
		{
			clearKeywordsEvent.Raise();
		}

		// Switch depending on card type
		if (cardTypeIndex.value == 0) 			// Follower
		{
			nonChampionBaseCardEvent.Invoke();
			nonLandmarkEvent.Invoke();
		}
		else if (cardTypeIndex.value == 1)  	// Champion base
		{
			championBaseCardEvent.Invoke();
			nonLandmarkEvent.Invoke();
		}
		else if (cardTypeIndex.value == 2) 		// Champion 2
		{
			championBaseCardEvent.Invoke();
			nonLandmarkEvent.Invoke();
			championLevel2Event.Invoke();
		}
		else if (cardTypeIndex.value == 8) 		// Champion Level 3
		{
			nonChampionBaseCardEvent.Invoke();
			nonLandmarkEvent.Invoke();
			championLevel3Event.Invoke();
		}
		else if (cardTypeIndex.value == 7) 		// Landmark
		{
			nonChampionBaseCardEvent.Invoke();
			landmarkEvent.Invoke();
		}
		else 									// else is Spell Card
		{
			spellCardEvent.Invoke();
		}

		// Show rarity dropdown
		if (cardTypeIndex.value == 1 || cardTypeIndex.value == 2)
		{
			hideRarityEvent.Invoke();
		}
		else
		{
			showRarityEvent.Invoke();
		}

		// Set Tribe Sprite Type
		if (cardTypeIndex.value <= 2 || cardTypeIndex.value == 8) // if non-spell card
		{
			if (cardTypeIndex.value == 0)
			{
				currentTribeSprite.sprite = tribeSprites.values[0];
			}
			else
			{
				currentTribeSprite.sprite = tribeSprites.values[1];
			}
		}

		// Add some effect keywords on spells
		if (cardTypeIndex.value == 3)
		{
			slowSpellEvent.Invoke();
		}
		else if (cardTypeIndex.value == 4)
		{
			fastSpellEvent.Invoke();
		}
		else if (cardTypeIndex.value == 5)
		{
			burstSpellEvent.Invoke();
		}
		else if (cardTypeIndex.value == 9)
		{
			focusSpellEvent.Invoke();
		}

		// Change Sprite
		if (cardTypeIndex.value == 1 || cardTypeIndex.value == 2 || cardTypeIndex.value == 8) // champion sprite
		{
			img.sprite = spritesPerCardType[cardTypeIndex.value].values[cardRarityIndex.value];
			foreach (Image ri in rarityImages)
			{
				ri.enabled = false;
			}
		}
		else if (cardTypeIndex.value != 6) // spells excl skill
		{
			img.sprite = spritesPerCardType[cardTypeIndex.value].values[0];
			spellCardFrame.sprite = spritesPerCardType[cardTypeIndex.value].values[0];

			foreach (Image ri in rarityImages)
			{
				if (cardRarityIndex.value > 0)
				{
					ri.enabled = true;
					ri.sprite = spritesPerCardType[cardTypeIndex.value].values[cardRarityIndex.value];
				}
				else
				{
					ri.enabled = false;
				}
			}
		}
		else if (cardTypeIndex.value == 6)// skill card type
		{
			spellCardFrame.sprite = spritesPerCardType[4].values[0];
			if (cardRarityIndex.value > 0)
			{
				rarityImages[1].enabled = true;
				rarityImages[1].sprite = spritesPerCardType[4].values[cardRarityIndex.value];
			}
			else
			{
				rarityImages[1].enabled = false;
			}
			skillEvent.Invoke();
		}

		// Focus Image
		focusImage.enabled = cardTypeIndex.value == 9 ? true : false;

		// Invoke Change card event
		cardChangeEvent.Invoke();
		previousCardType = cardTypeIndex.value;
	}
}