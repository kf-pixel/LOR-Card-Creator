using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OneOfs
{
	public class InputCtrlShift : MonoBehaviour
	{
		public BoolVariable shiftInput;
		public BoolVariable ctrlInput;
		public void ShiftInput(InputAction.CallbackContext context)
		{
			shiftInput.value = context.ReadValueAsButton();
		}

		public void CtrlInput(InputAction.CallbackContext context)
		{
			ctrlInput.value = context.ReadValueAsButton();
		}
	}
}