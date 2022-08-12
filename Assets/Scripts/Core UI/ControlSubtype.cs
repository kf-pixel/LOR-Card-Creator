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
		if (string.IsNullOrEmpty(tmp.text))
		{
			img.enabled = false;
			tmp.enabled = false;
		}
		else
		{
			img.enabled = true;
			tmp.enabled = true;
		}
	}
}