using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class TMP_MultiLineShiftEnter : MonoBehaviour
{
    private TMP_InputField inputField;

	private void Awake()
    {
		inputField = GetComponent<TMP_InputField>();
	}

	public void ShiftEnterField(InputAction.CallbackContext ctx)
	{
		if (EventSystem.current.currentSelectedGameObject == inputField.gameObject && ctx.performed)
		{
			StartCoroutine(ShiftEnterFieldIE());
		}
	}
	private IEnumerator ShiftEnterFieldIE()
	{
		inputField.ActivateInputField();

		yield return null;
		//inputField.text = inputField.text.Substring(0, inputField.text.Length - 1) + "<br>";
		//inputField.text += "\n";
		//inputField.MoveTextEnd(false);
	}
}
