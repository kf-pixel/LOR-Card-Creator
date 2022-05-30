using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlSubtype : MonoBehaviour
{
	[SerializeField] private Image img;
	[SerializeField] private TextMeshProUGUI tmp;

	private void OnEnable()
	{
		HideOrShow();
	}

	public void HideOrShow()
	{
		if (tmp.text.Length == 0)
		{
			img.enabled = false;
			tmp.enabled = false;
		}
		else if (tmp.text.Length > 0)
		{
			img.enabled = true;
			tmp.enabled = true;
		}
	}
}