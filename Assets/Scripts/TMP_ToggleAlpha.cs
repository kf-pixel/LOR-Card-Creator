using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class TMP_ToggleAlpha : MonoBehaviour
{
	public float alphaOn = 1, alphaOff = 0.35f;
	private TextMeshProUGUI textMeshProUGUI;
	private Toggle toggle;
	private UnityAction<bool> alphaToggle;

	private void Awake()
    {
		toggle = GetComponent<Toggle>();
		textMeshProUGUI = GetComponent<TextMeshProUGUI>();
		if (textMeshProUGUI == null)
		{
			textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
		}

		alphaToggle += ToggleAlpha;

		ToggleAlpha(toggle.isOn);
	}

    private void OnEnable()
    {
		toggle.onValueChanged.AddListener(alphaToggle);
	}

     private void OnDisable()
     {
		toggle.onValueChanged.RemoveAllListeners();
     }

	public void ToggleAlpha(bool b)
	{
		textMeshProUGUI.alpha = toggle.isOn ? alphaOn : alphaOff;
	}
}
