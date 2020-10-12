using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardTypeChanger : MonoBehaviour
{
	[SerializeField] private Image img;
	[SerializeField] private Image spellCardFrame;
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
	[SerializeField] private UnityEvent championBaseCardEvent;
	[SerializeField] private UnityEvent nonChampionBaseCardEvent;

	[SerializeField] private UnityEvent spellCardEvent;
	[SerializeField] private UnityEvent slowSpellEvent;
	[SerializeField] private UnityEvent fastSpellEvent;
	[SerializeField] private UnityEvent burstSpellEvent;

	[SerializeField] private UnityEvent showRarityEvent;
	[SerializeField] private UnityEvent hideRarityEvent;

	[SerializeField] private UnityEvent skillEvent;

	[SerializeField] private UnityEvent landmarkEvent;
	[SerializeField] private UnityEvent nonLandmarkEvent;


	private void Start()
	{
		previousCardType = defaultIndex;
		cardTypeIndex.value = defaultIndex;
		ParseType();
	}

	public void ParseType()
	{
		// clear keywords if going from unit > spell or vice versa
		if (previousCardType <= 2 && cardTypeIndex.value > 2 || previousCardType > 2 && cardTypeIndex.value <= 2 || cardTypeIndex.value == 7)
		{
			clearKeywordsEvent.Raise();
		}

		if (cardTypeIndex.value == 1)  // if Champion base card
		{
			championBaseCardEvent.Invoke();
			nonLandmarkEvent.Invoke();
		}
		else if (cardTypeIndex.value == 0 || cardTypeIndex.value == 2) // if Follow or Champion Leveled card
		{
			nonChampionBaseCardEvent.Invoke();
			nonLandmarkEvent.Invoke();
		}
		else if (cardTypeIndex.value == 7)
		{
			nonChampionBaseCardEvent.Invoke();
			landmarkEvent.Invoke();
		}
		else // else is Spell Card
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
		if (cardTypeIndex.value <= 2) // if non-spell card
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

		// Change Sprite
		if (cardTypeIndex.value != 6)
		{
			img.sprite = spritesPerCardType[cardTypeIndex.value].values[cardRarityIndex.value];
			spellCardFrame.sprite = spritesPerCardType[cardTypeIndex.value].values[cardRarityIndex.value];
		}
		else // skill card type
		{
			img.sprite = spritesPerCardType[3].values[cardRarityIndex.value];
			spellCardFrame.sprite = spritesPerCardType[3].values[cardRarityIndex.value];
			skillEvent.Invoke();
		}

		// Invoke Change card event
		cardChangeEvent.Invoke();
		previousCardType = cardTypeIndex.value;
	}
}