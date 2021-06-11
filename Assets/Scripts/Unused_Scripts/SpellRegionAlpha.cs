using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SpellRegionAlpha : MonoBehaviour
{
	[SerializeField] Image regionImage;
	[SerializeField] TextMeshProUGUI titleTMP;
	[SerializeField] TextMeshProUGUI spellTextTMP;
	[SerializeField] float alphaValue = 0.5f;
	[SerializeField] float textHeightTrigger = 25f;

    private void OnEnable()
    {
		DetermineRegionAlpha();
	}
	private void OnDisable()
	{
		regionImage.material.SetColor("_Color", new Color(regionImage.color.r, regionImage.color.g, regionImage.color.b, 1));
	}

	public void DetermineRegionAlpha()
	{
		StartCoroutine(DetermineRegionAlphaCoroutine());
	}

	private IEnumerator DetermineRegionAlphaCoroutine()
	{
		yield return new WaitForEndOfFrame();

		var newColor = titleTMP.renderedHeight + spellTextTMP.renderedHeight > textHeightTrigger ?
			new Color(regionImage.color.r, regionImage.color.g, regionImage.color.b, alphaValue) :
			new Color(regionImage.color.r, regionImage.color.g, regionImage.color.b, 1);

		regionImage.material.SetColor("_Color", newColor);
	}
}
