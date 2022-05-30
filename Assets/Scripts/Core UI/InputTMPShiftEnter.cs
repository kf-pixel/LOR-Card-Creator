using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InputTMPShiftEnter : MonoBehaviour
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
