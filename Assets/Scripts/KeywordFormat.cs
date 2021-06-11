using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeywordFormat : MonoBehaviour
{
	[SerializeField] private RectTransform panel;
	[SerializeField] private TextMeshProUGUI textField;
	[SerializeField] private Image panelImage;
	[SerializeField] private Sprite fullPanelSprite;
	[SerializeField] private Sprite shortPanelSprite;
	private float fullWidth;
	private string fullText;
	private bool isFullLength = true;
	private void Awake()
    {
		InitializePanel();
	}

	public float GetWidth()
	{
		return fullWidth;
	}

	private void InitializePanel()
	{
		textField.ForceMeshUpdate();
		fullWidth = textField.renderedWidth + 18f;
		panel.sizeDelta = new Vector2(fullWidth, panel.sizeDelta.y);
		fullText = textField.text;
	}

	public void SetToFull()
	{
		panelImage.sprite = fullPanelSprite;
		panel.sizeDelta = new Vector2(fullWidth, panel.sizeDelta.y);
		textField.text = fullText;
		isFullLength = true;
	}

	public void SetToShort()
	{
		panelImage.sprite = shortPanelSprite;
		panel.sizeDelta = new Vector2(50f, panel.sizeDelta.y);

		int spriteIndex = fullText.LastIndexOf(">") + 1;
		textField.text = spriteIndex > 0 ? fullText.Substring(0, spriteIndex) : "?";
		isFullLength = false;
	}

	public void UpdateFullText(string customTxt) // for custom keyword
	{
		fullText = customTxt;
		textField.ForceMeshUpdate();
		fullWidth = textField.renderedWidth + 18f;
		panel.sizeDelta = new Vector2(fullWidth, panel.sizeDelta.y);

		if (isFullLength) SetToFull();
        else SetToShort();
	}
}
