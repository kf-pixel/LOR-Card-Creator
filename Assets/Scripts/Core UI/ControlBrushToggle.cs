using UnityEngine;
using TMPro;

public class ControlBrushToggle : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI brush;

	public void ToggleImage(string s)
	{
		brush.enabled = string.IsNullOrEmpty(s) ? false : true;
	}
}