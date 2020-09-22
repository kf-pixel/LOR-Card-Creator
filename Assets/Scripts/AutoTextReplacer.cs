using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class AutoTextReplacer : MonoBehaviour
{
	[SerializeField] private BoolVariable disableAutoReplace;
	[SerializeField] private TextMeshProUGUI tmpUGUIField;
	[SerializeField] private StringPairVariable stringReplacer;
	[SerializeField] private StringVariable cardTitle;

	[Header("Custom Keyword Data")]
	[SerializeField] private StringVariable title;
	[SerializeField] private StringVariable colour;
	[SerializeField] private IntVariable spriteIndex;

	[SerializeField] private StringVariable title1;
	[SerializeField] private StringVariable colour1;
	[SerializeField] private IntVariable spriteIndex1;

	[SerializeField] private StringVariable title2;
	[SerializeField] private StringVariable colour2;
	[SerializeField] private IntVariable spriteIndex2;

	public void ReplaceTMPUGUI()
	{
		if (disableAutoReplace.value == true) return;

		// Go through each entry in the StringPair AutoReplace List, replace the input string for each one
		foreach (StringPair p in stringReplacer.values)
		{
			// Check if the input string matches
			if (tmpUGUIField.text.ToLower().Contains(p.inputString.ToLower()))
			{
				tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, p.inputString, p.replacedString, RegexOptions.IgnoreCase);
				tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, "\"" + p.replacedString, "\"" + p.inputString.ToLower(), RegexOptions.IgnoreCase);

				// After replacing the inputString, Find the next suitable string index to insert a </color> tag
				int loops = 0;
				int replacedIndex = tmpUGUIField.text.IndexOf(p.replacedString);
				while (tmpUGUIField.text.Substring(replacedIndex).Contains(p.replacedString) && loops < 8) // Loop for each instance of the replaced string
				{
					char[] nextCharArray = " :;?!.,".ToCharArray(); // these characters are considered as suitable for positions to add the </color tag>
					int nextSpace = Mathf.Clamp(tmpUGUIField.text.IndexOfAny(nextCharArray, replacedIndex + p.replacedString.Length - 1), -1, tmpUGUIField.text.Length - 1);

					if (nextSpace > 0)
					{
						tmpUGUIField.text = tmpUGUIField.text.Insert(nextSpace, "</color>"); // add the color tag and repeat for the next instance
						replacedIndex = nextSpace;
					}
					else
					{
						break;
					}
					loops++;
				}
			}
		}

		// Replace the Custom Keywords
		if (title.value != null)
		{
			if (title.value.Length > 0)
			{
				string customKeyword = "<color=" + colour.value + "><sprite name=\"Custom_" + spriteIndex.value + "\" tint>" + "<color=#F0CC70>" + title.value + "</color></color>";
				tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, title.value, customKeyword, RegexOptions.IgnoreCase);
			}
		}

		if (title1.value != null)
		{
			if (title1.value.Length > 0)
			{
				string customKeyword = "<color=" + colour1.value + "><sprite name=\"Custom_" + spriteIndex1.value + "\" tint>" + "<color=#F0CC70>" + title1.value + "</color></color>";
				tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, title1.value, customKeyword, RegexOptions.IgnoreCase);
			}
		}

		if (title2.value != null)
		{
			if (title2.value.Length > 0)
			{
				string customKeyword = "<color=" + colour2.value + "><sprite name=\"Custom_" + spriteIndex2.value + "\" tint>" + "<color=#F0CC70>" + title2.value + "</color></color>";
				tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, title2.value, customKeyword, RegexOptions.IgnoreCase);
			}
		}

	}
}