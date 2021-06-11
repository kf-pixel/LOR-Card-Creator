using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TMP_Colour : MonoBehaviour
{
	[SerializeField] private Color[] colours;
	[SerializeField] private TextMeshProUGUI textMeshProUGUI;

	public void ChangeTMPColour(int i)
	{
		textMeshProUGUI.color = colours[i];
	}
}
