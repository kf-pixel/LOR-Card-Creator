using TMPro;
using UnityEngine;

public class DropdownUpdate : MonoBehaviour
{
	[SerializeField] private IntVariable intVar;
	[SerializeField] private TMP_Dropdown tmpdd;

	public void SetValue()
	{
		tmpdd.value = intVar.value;
	}
}