using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RegionToggle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public int regionID;
	[SerializeField] private IntVariable region1, region2;
	[SerializeField] private GameEvent refreshEvent;
	[SerializeField] private GameObject label, checkmark;
	[SerializeField] private Image backgroundImg, checkmarkImg;
	[SerializeField] private UnityEvent onActive, onInactive;
	private bool entered;

	private void OnEnable()
    {
		Parse();
	}

	public void OnPointerEnter(PointerEventData pointerEventData)
	{
		entered = true;
	}

	public void OnPointerExit(PointerEventData pointerEventData)
	{
		entered = false;
	}

    private void Update()
    {
		if (entered)
		{
			if (Input.GetMouseButtonDown(0))
			{
				Activate(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
			}
			else if (Input.GetMouseButtonDown(1))
			{
				Activate(true);
			}
		}
	}

	public void Parse()
	{
		if (regionID == region1.value)
		{
			label.SetActive(true);
			checkmark.SetActive(true);
			onActive?.Invoke();
		}
        else if (regionID == region2.value && regionID != 13)
		{
			label.SetActive(true);
			checkmark.SetActive(true);
			onActive?.Invoke();
		}
		else
		{
			Deactivate();
			onInactive?.Invoke();
		}
	}

	public void Activate(bool asDual = false)
	{
		if (asDual == false)
		{
			region1.NewInt(regionID);
			region2.NewInt(13);
		}
		else // dual region input
		{
            if (regionID == region1.value) return;
			region2.NewInt(regionID);
		}
		refreshEvent.Raise();

		label.SetActive(true);
		checkmark.SetActive(true);
        
        if (regionID == 13 && asDual) Deactivate();
	}

	public void Deactivate()
	{
		label.SetActive(false);
		checkmark.SetActive(false);
		entered = false;
	}

	public void UpdateSprites(Sprite newSprite)
	{
		backgroundImg.sprite = newSprite;
		checkmarkImg.sprite = newSprite;
	}
}
