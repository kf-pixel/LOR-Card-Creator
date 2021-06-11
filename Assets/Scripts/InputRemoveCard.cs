using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputRemoveCard : MonoBehaviour
{
	public UnityEvent onPointerUp;
	private bool isActive;

	public void SetActive(bool active)
	{
		isActive = active;
	}
	public void PointerUp(InputAction.CallbackContext context)
	{
		if (context.performed && isActive) StartCoroutine(PointerUpIE());
	}

	private IEnumerator PointerUpIE()
	{
		yield return new WaitForEndOfFrame();
		onPointerUp.Invoke();
	}
}
