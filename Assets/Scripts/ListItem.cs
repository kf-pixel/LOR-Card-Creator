using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListItem : MonoBehaviour
{
	public ListManager listManager;
	public CardDataObject cardData;
	public TextMeshProUGUI textLabel;
	public int listOrderIndex;

	private void Start()
	{
		UpdateLabel();
	}

	public void UpdateLabel()
	{
		// Determine type of card
		int cardTypeIndex = cardData.GetCardTypeIndex(listManager.cardTypeNames);
		string cardTypeName = cardData.GetCardType(listManager.cardTypeNames);
		string cardDataAppend = "   <alpha=#66><cspace=1><font=\"Univers 59 Ultra Condensed SDF\">[" + cardTypeName + "]";
		string colorTypeAppend = cardTypeIndex == 1 || cardTypeIndex == 2 ? "<color=#F1D590>" : "<color=#88CDD4>";
		if (cardTypeIndex == 0)
		{
			colorTypeAppend = "";
		}

		textLabel.text = (listOrderIndex + 1) < 10 ?
			"<alpha=#66><mspace=6>" + (listOrderIndex + 1) + "  </mspace></color>" + colorTypeAppend + cardData.cardName + cardDataAppend :
			"<alpha=#66><mspace=6>" + (listOrderIndex + 1) + " </mspace></color>"  + colorTypeAppend + cardData.cardName + cardDataAppend ;
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
}
