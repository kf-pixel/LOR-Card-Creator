using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "New Custom Keyword Data", menuName = "Custom Keyword Data")]
public class CustomKeywordData : ScriptableObject
{
	public string label;
	public string description;
	public string hexColor;
	public int spriteIndex;
	public float colorR;
	public float colorG;
	public float colorB;

	public void SetLabel(string s)
	{
		label = s;
	}

	public void SetDescription(string s)
	{
		description = s;
	}

	public void SetHexColour(string s)
	{
		hexColor = s;
	}

	public void SetSpriteIndex(int i)
	{
		spriteIndex = i;
	}

	// Used to translate the user sprite index from the dropdown option (for sprites >68) 
	public void SetUserSpriteIndex(TMP_Dropdown dropdown)
	{
		if (dropdown.value <= 68) return;

		string activeOptionText = dropdown.options[dropdown.value].text;
		int userIndex = activeOptionText.IndexOf("user");
		if (userIndex > 0)
		{
			int parsedIndex = 1;
			int.TryParse(activeOptionText[userIndex + 4].ToString(), out parsedIndex);
			spriteIndex = 68 + parsedIndex;
		}
	}

	public void SetColorR(float f)
	{
		colorR = f;
	}

	public void SetColorG(float f)
	{
		colorG = f;
	}

	public void SetColorB(float f)
	{
		colorB = f;
	}

	private void OnEnable()
	{
		ResetValues();
	}

	public void ResetValues()
	{
		label = "";
		description = "";
		hexColor = "#FFFFFF";
		spriteIndex = 0;
		colorR = 1f;
		colorG = 1f;
		colorB = 1f;
	}
}
