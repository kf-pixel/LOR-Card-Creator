using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ListItem : MonoBehaviour, IPointerDownHandler
{
	public ListManager listManager;
	public CardDataObject cardData;
	public TextMeshProUGUI textLabel;
	public TextMeshProUGUI subTextLabel;
	public TextMeshProUGUI manaLabel;
	public TextMeshProUGUI indexLabel;
	public GameObject duplicateGameObject;
	public int listOrderIndex;
	[SerializeField] private UnityEvent highlightEvent;
	[SerializeField] private UnityEvent dehighlightEvent;

	private void Start()
	{
		UpdateLabel();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		SetAsActiveItem();
	}

	public void UpdateLabel()
	{
		// Determine type of card
		string cardRegion = cardData.GetRegion(listManager.regionNames);
		string cardRarity = cardData.GetRarity(listManager.rarityNames);
		string cardSubtype = cardData.GetSubtype();

		int cardTypeIndex = cardData.GetCardTypeIndex(listManager.cardTypeNames);
		//string cardTypeName = cardData.GetCardType(listManager.cardTypeNames).Replace("LVL", "-");
		string cardTypeName = cardData.GetCardType(listManager.cardTypeNames);

		string colorTypeAppend = "<color=#88CDD4>";
		if (cardTypeIndex == 1)
		{
			colorTypeAppend = "<color=#F1D590>";
		}
		else if (cardTypeIndex == 2)
		{
			colorTypeAppend = "<color=#e3c729>";
		}
		else if (cardTypeIndex == 8)
		{
			colorTypeAppend = "<color=#ebe309>";
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
		textLabel.text = $"<sprite name=\"{cardTypeName}\">{colorTypeAppend}  {cardData.cardName}";

		// Update Sub Text Labels
		//subTextLabel.text = $"<sprite name=\"{cardRegion}\"><sprite name=\"{cardRarity}\">{colorTypeAppend}<alpha=#66> {cardTypeName}";
		subTextLabel.text = $"<sprite name=\"{cardRegion}\"><sprite name=\"{cardRarity}\">{colorTypeAppend}</color><alpha=#66>{cardSubtype}";
		indexLabel.text = (listOrderIndex + 1).ToString();
		manaLabel.text = $"{cardData.GetMana()}";
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
