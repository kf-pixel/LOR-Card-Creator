using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomKeywordSwatch : MonoBehaviour
{
	[SerializeField] private CustomKeywordData keywordData;
	[SerializeField] private Button[] swatches;
	[SerializeField] private Tooltip[] tooltips;

	public void DisableSwatches()
	{
		foreach (Button b in swatches)
		{
		    b.interactable = (keywordData.spriteIndex > 0) ? true : false;
		}
		foreach (Tooltip t in tooltips)
		{
			t.enabled = (keywordData.spriteIndex > 0) ? true : false;
		}
	}

    private void OnEnable()
    {
		DisableSwatches();
	}
}
