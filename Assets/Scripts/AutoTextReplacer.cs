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
	[SerializeField] private CustomKeywordData[] KWData;

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
				tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, "=" + "\"" + p.replacedString, "=" + "\"" + p.inputString.ToLower(), RegexOptions.IgnoreCase);

				// After replacing the inputString, Find the next suitable string index to insert a </color> tag
				int loops = 0;
				int replacedIndex = tmpUGUIField.text.IndexOf(p.replacedString);
				if (replacedIndex < 0) break;
				while (tmpUGUIField.text.Substring(replacedIndex).Contains(p.replacedString) && loops < 8) // Loop for each instance of the replaced string
				{
					char[] nextCharArray = " :;?!.,\"'".ToCharArray(); // these characters are considered as suitable for positions to add the </color tag>
					int nextSpace = Mathf.Clamp(tmpUGUIField.text.IndexOfAny(nextCharArray, replacedIndex + p.replacedString.Length - 3), 0, tmpUGUIField.text.Length - 1);
					if (nextSpace > 0)
					{
						tmpUGUIField.text = tmpUGUIField.text.Insert(nextSpace, "</color>"); // add the color tag and repeat for the next instance
						//replacedIndex = nextSpace;
						replacedIndex = Mathf.Clamp(tmpUGUIField.text.IndexOf(p.replacedString, nextSpace), -1, tmpUGUIField.text.Length - 1);

						if (replacedIndex < 0) break;
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
		foreach(CustomKeywordData k in KWData)
		{
			if (string.IsNullOrEmpty(k.label)) continue;
			if (k.label.Length < 2) continue;

			string ks = k.spriteIndex > 0 ? 
				"<color=" + k.hexColor + "><sprite name=\"Custom_" + k.spriteIndex + "\" tint>" + "<color=#F0CC70>" + k.label + "</color></color>" :
				"<color=#F0CC70>" + k.label + "</color>";

			tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, k.label, ks, RegexOptions.IgnoreCase);
		}
	}
}