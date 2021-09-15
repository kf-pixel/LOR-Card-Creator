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
		public BoolVariable enterInput;
		private void Update()
		{
			shiftInput.value = Input.GetKey(KeyCode.LeftShift);
			ctrlInput.value = Input.GetKey(KeyCode.LeftControl);
			enterInput.value = Input.GetKeyDown(KeyCode.Return);
		}

		/*
				public void ShiftInput(InputAction.CallbackContext context)
				{
					shiftInput.value = context.ReadValueAsButton();
				}

				public void CtrlInput(InputAction.CallbackContext context)
				{
					ctrlInput.value = context.ReadValueAsButton();
				}

				public void EnterInput(InputAction.CallbackContext context)
				{
					enterInput.value = context.ReadValueAsButton();
				}
		*/
	}
}