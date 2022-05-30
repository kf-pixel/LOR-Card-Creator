using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using System.Text.RegularExpressions;

public class KeywordFormat : BasePointerClick
{
	[SerializeField] private RectTransform panel;
	[SerializeField] private TextMeshProUGUI textField;
	[SerializeField] private Image panelImage;
	[SerializeField] private Sprite fullPanelSprite;
	[SerializeField] private Sprite shortPanelSprite;
	[SerializeField] private StringEvent keywordValueEvent;
	private float fullWidthValue;
	private string fullTextValue;
	private bool isFullLength = true;
	[HideInInspector] public int keywordIndex;
	[HideInInspector] public string keywordName;
	[HideInInspector] public int keywordValue;
	public bool toggleable = true; // determines if keyword is displayed

	protected override void Awake()
    {
		base.Awake();
		InitializePanel();
	}

	protected override void OnClick()
	{
		if (shift || ctrl) return;
		keywordValue = Mathf.Clamp(keywordValue + 1, 0, 99);
		UpdateKeywordValue();
	}

	protected override void OnAltClick()
	{
		keywordValue = Mathf.Clamp(keywordValue - 1, 0, 99);
		UpdateKeywordValue();
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
		return fullWidthValue;
	}

	private void InitializePanel()
	{
		textField.ForceMeshUpdate();

		fullWidthValue = textField.renderedWidth + 20f;
		panel.sizeDelta = new Vector2(fullWidthValue, panel.sizeDelta.y);
		fullTextValue = textField.text;
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

		panel.sizeDelta = new Vector2(fullWidthValue + keywordValueWidth, panel.sizeDelta.y);
		if (keywordValue < 1)
		{
			textField.text = fullTextValue;
		}
		else
		{
			textField.text = $"{fullTextValue} {keywordValue}";
		}
	}

	public void SetToShort()
	{
		// Exit & display full name if the keyword has no sprite
		if (!textField.text.StartsWith("<"))
		{
			SetToFull();
			return;
		}

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

		MatchCollection regM = Regex.Matches(fullTextValue, "<sprite name");
		panel.sizeDelta = new Vector2(30 + (regM.Count * 15) + keywordValueWidth, panel.sizeDelta.y);
		int spriteIndex = fullTextValue.LastIndexOf(">") + 1;

		if (keywordValue < 1)
		{
			textField.text = spriteIndex > 0 ? fullTextValue.Substring(0, spriteIndex) : "?";
		}
		else
		{
			textField.text = spriteIndex > 0 ? $"{fullTextValue.Substring(0, spriteIndex)}{keywordValue}" : $"? {keywordValue}";
		}
	}

	public void UpdateFullText(string customTxt) // for custom keyword
	{
		fullTextValue = customTxt;
		textField.ForceMeshUpdate();
		fullWidthValue = textField.renderedWidth + 18f;
		panel.sizeDelta = new Vector2(fullWidthValue, panel.sizeDelta.y);

		if (isFullLength) SetToFull();
        else SetToShort();
	}
}
