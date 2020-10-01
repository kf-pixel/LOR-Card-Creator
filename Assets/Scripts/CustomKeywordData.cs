using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		label = "";
		description = "";
		hexColor = "";
		spriteIndex = 0;
		colorR = 0f;
		colorG = 0f;
		colorB = 0f;
	}
}
