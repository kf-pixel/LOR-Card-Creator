using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using System.Text.RegularExpressions;

public class KeywordFormat : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private RectTransform panel;
	[SerializeField] private TextMeshProUGUI textField;
	[SerializeField] private Image panelImage;
	[SerializeField] private Sprite fullPanelSprite;
	[SerializeField] private Sprite shortPanelSprite;
	[SerializeField] private StringEvent keywordValueEvent;
	private float fullWidth;
	private string fullText;
	private bool isFullLength = true;
	private bool pointerInside;
	public string keywordName;
	public int keywordIndex;
	public int keywordValue;
	public bool toggleable = true;

	private void Awake()
    {
		InitializePanel();
	}

	public void IncreaseKeywordValue(InputAction.CallbackContext ctx)
	{
		if (pointerInside && ctx.performed)
		{
			keywordValue++;
			keywordValue = Mathf.Clamp(keywordValue, 0, 99);
			UpdateKeywordValue();
		}
	}

	public void DecreaseKeywordValue(InputAction.CallbackContext ctx)
	{
		if (pointerInside && ctx.performed)
		{
			keywordValue--;
			keywordValue = Mathf.Clamp(keywordValue, 0, 99);
			UpdateKeywordValue();
		}
	}

	private void UpdateKeywordValue()
	{
		if (isFullLength)
		{
			SetToFull();
		}
		else
		{
			SetToShort();
		}
		keywordValueEvent.Raise($"{keywordName},{keywordValue}");
	}

	public float GetWidth()
	{
		return fullWidth;
	}

	private void InitializePanel()
	{
		textField.ForceMeshUpdate();

		fullWidth = textField.renderedWidth + 20f;
		panel.sizeDelta = new Vector2(fullWidth, panel.sizeDelta.y);
		fullText = textField.text;
	}

	public void SetToFull()
	{
		isFullLength = true;
		panelImage.sprite = fullPanelSprite;
		textField.margin = new Vector4(-1, 0, 3, 0);

		float keywordValueWidth = 0;
		if (keywordValue > 9)
		{
			keywordValueWidth = 26;
		}
		else if (keywordValue > 0)
		{
			keywordValueWidth = 13;
		}

		panel.sizeDelta = new Vector2(fullWidth + keywordValueWidth, panel.sizeDelta.y);
		if (keywordValue < 1)
		{
			textField.text = fullText;
		}
		else
		{
			textField.text = $"{fullText} {keywordValue}";
		}
	}

	public void SetToShort()
	{
		isFullLength = false;
		panelImage.sprite = shortPanelSprite;
		textField.margin = Vector4.zero;

		float keywordValueWidth = 0;
		if (keywordValue > 9)
		{
			keywordValueWidth = 16;
		}
		else if (keywordValue > 0)
		{
			keywordValueWidth = 8;
		}

		MatchCollection regM = Regex.Matches(fullText, "<sprite name");
		panel.sizeDelta = new Vector2(35 + (regM.Count * 15) + keywordValueWidth, panel.sizeDelta.y);
		int spriteIndex = fullText.LastIndexOf(">") + 1;

		if (keywordValue < 1)
		{
			textField.text = spriteIndex > 0 ? fullText.Substring(0, spriteIndex) : "?";
		}
		else
		{
			textField.text = spriteIndex > 0 ? $"{fullText.Substring(0, spriteIndex)}{keywordValue}" : $"? {keywordValue}";
		}
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

	public void OnPointerEnter(PointerEventData eventData)
	{
		pointerInside = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		pointerInside = false;
	}
}
