using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class MouseClickInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
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

	public void Increment(InputAction.CallbackContext ctx)
	{
		if (ctx.performed && inside)
		{
			int stringInt = 0;
			if (int.TryParse(inputF.text, out stringInt) == true)
			{
				inputF.text = (stringInt + 1).ToString();
			}
			else
			{
				inputF.text = "1";
				return;
			}
		}
	}

	public void Decrement(InputAction.CallbackContext ctx)
	{
		if (ctx.performed && inside)
		{
			int stringInt = 0;
			if (int.TryParse(inputF.text, out stringInt) == true)
			{
				inputF.text = (stringInt - 1).ToString();
			}
			else
			{
				inputF.text = "1";
				return;
			}
		}
	}
}
