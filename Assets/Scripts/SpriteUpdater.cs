using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class SpriteUpdater : MonoBehaviour
{
	[SerializeField] private Image img;
	[SerializeField] private IntVariable intSelectionIndex;
	[SerializeField] private SpritesVariable spritesList;

	// Rarity Updater
	[SerializeField] private IntVariable cardIndex;

	public void UpdateSprite()
	{
		img.sprite = spritesList.values[intSelectionIndex.value];
	}

	public void UpdateSpriteIndex(int i)
	{
		img.sprite = spritesList.values[i];
	}

	public void UpdateRarity()
	{
		if (cardIndex.value == 2 || cardIndex.value == 1)
		{
			img.sprite = spritesList.values[4];
		}
		else
		{
			UpdateSprite();
		}
	}
}