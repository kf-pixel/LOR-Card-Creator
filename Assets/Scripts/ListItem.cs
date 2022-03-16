using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ListItem : MonoBehaviour
{
	public ListManager listManager;
	public CardDataObject cardData;
	public TextMeshProUGUI textLabel;
	public TextMeshProUGUI subTextLabel;
	public TextMeshProUGUI manaLabel;
	public TextMeshProUGUI indexLabel;
	public GameObject duplicateGameObject;
	public int listOrderIndex;
	[SerializeField] private UnityEvent highlightEvent, dehighlightEvent;

	private void Start()
	{
		UpdateLabel();
	}

	public void UpdateLabel()
	{
		// Determine type of card
		int cardTypeIndex = cardData.GetCardTypeIndex(listManager.cardTypeNames);
		string cardTypeName = cardData.GetCardType(listManager.cardTypeNames);

		string cardDataAppend = $"   {cardTypeName}";
		string colorTypeAppend = "<color=#88CDD4>";
		if (cardTypeIndex == 1)
		{
			colorTypeAppend = "<color=#F1D590>";
		}
		else if (cardTypeIndex == 2)
		{
			colorTypeAppend = "<color=#f4d853>";
		}
		else if (cardTypeIndex == 8)
		{
			colorTypeAppend = "<color=#f9b50d>";
		}
		else if (cardTypeIndex == 0)
		{
			colorTypeAppend = "";
		}
		else if (cardTypeIndex == 7)
		{
			colorTypeAppend = "<color=#CCDBD7>";
		}

		// Update Main Label
		textLabel.text = colorTypeAppend + cardData.cardName;

		// Update Sub Text Labels
		subTextLabel.text = colorTypeAppend + cardDataAppend;
		indexLabel.text = (listOrderIndex + 1).ToString();
		manaLabel.text = cardData.GetMana();
	}

	public void UpdateOrderIndexOnly()
	{
		indexLabel.text = (listOrderIndex + 1).ToString();
	}

	public void SetAsActiveItem()
	{
		if (listManager != null)
		{
			listManager.SetNewActiveItem(this);
		}
	}

	public void SetAsHoveringItem()
	{
		if (listManager != null)
		{
			listManager.SetNewHoveringItem(this);
		}
	}

	public void SetOnDragEnd(Vector3 mousePosition)
	{
		if (listManager != null)
		{
			listManager.RepositionOnDrag(this, mousePosition);
		}
	}

	public void SetAsDragging()
	{
		if (listManager != null)
		{
			listManager.SetActiveDragging(this);
		}
	}

	// for multi select function

	public void Highlight()
	{
		highlightEvent.Invoke();
	}

	public void Dehighlight()
	{
		dehighlightEvent.Invoke();
	}

	public void DisableDuplicateGameObject()
	{
		duplicateGameObject.SetActive(false);
	}

	public void EnableDuplicateGameObject()
	{
		duplicateGameObject.SetActive(true);
	}
}
