using UnityEngine;

public class InputCtrlShift : MonoBehaviour
{
	public BoolVariable shiftInput;
	public BoolVariable ctrlInput;
	public BoolVariable enterInput;

	private void Update()
	{
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			shiftInput.value = true;
		}
		else
		{
			shiftInput.value = false;
		}

		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
		{
			ctrlInput.value = true;
		}
		else
		{
			ctrlInput.value = false;
		}

		enterInput.value = Input.GetKeyDown(KeyCode.Return);
	}
}