using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class MouseClickInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private Slider inputSlider;
	[SerializeField] private TMP_InputField inputF;
	private bool inside;

	public void OnPointerEnter(PointerEventData eventData)
	{
		inside = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		inside = false;
	}

	private void OnDisable()
	{
		inside = false;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && inside)
		{
			//inputSlider.value++;
			int stringInt = 0;
			if (int.TryParse(inputF.text, out stringInt) == true)
			{
				inputF.text = (stringInt+1).ToString();
			}
			else
			{
				return;
			}
		}
		if (Input.GetMouseButtonDown(1) && inside)
		{
			//inputSlider.value--;
			int stringInt = 0;
			if (int.TryParse(inputF.text, out stringInt) == true)
			{
				inputF.text = (stringInt-1).ToString();
			}
			else
			{
				return;
			}
		}
	}
}
