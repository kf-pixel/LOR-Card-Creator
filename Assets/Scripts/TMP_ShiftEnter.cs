using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class TMP_ShiftEnter : MonoBehaviour
{

	private TMP_InputField inputfield;
	[SerializeField] private BoolVariable shiftInput;
	[SerializeField] private BoolVariable enterInput;

	private void Awake()
	{
		inputfield = GetComponent<TMP_InputField>();

	}

	IEnumerator FieldFix()
	{
		//print("Shift + Enter newline.");
		inputfield.ActivateInputField();

		yield return null;

		//inputfield.text += "\n";
		inputfield.MoveTextEnd(false);
	}
	void Update()
	{
		if (EventSystem.current.currentSelectedGameObject == inputfield.gameObject)
		{
			if (enterInput.value)
			{
				inputfield.text = inputfield.text.TrimEnd('\n');
				if (shiftInput.value)
				{
					StartCoroutine(FieldFix());
				}
			}
		}
	}
}
