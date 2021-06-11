using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class AutoTextReplacer : MonoBehaviour
{
	[SerializeField] private BoolVariable disableAutoReplace;
	[SerializeField] private TextMeshProUGUI tmpUGUIField;
	[SerializeField] private StringPairVariable stringReplacer, regularTextReplacer;
	[SerializeField] private StringVariable cardTitle;

	[Header("Custom Keyword Data")]
	[SerializeField] private CustomKeywordData[] KWData;

	public void ReplaceTMPUGUI()
	{
		if (disableAutoReplace.value == true) return;

		// insert spaces
		MatchCollection spaces = Regex.Matches(tmpUGUIField.text, "(\\.|:|,)(?=\\w)");
		for (int i = spaces.Count - 1; i >= 0; i--)
		{
			tmpUGUIField.text = tmpUGUIField.text.Insert(spaces[i].Index + 1, " ");
		}
		// Capitalize letters
		MatchCollection capitals = Regex.Matches(tmpUGUIField.text, "(?<=(: |\\. ?)|^|<br>)\\w");
		foreach (Match c in capitals)
		{
			tmpUGUIField.text = tmpUGUIField.text.Remove(c.Index, 1);
			tmpUGUIField.text = tmpUGUIField.text.Insert(c.Index, c.Value.ToUpper());
		}

		// Go through each entry in the StringPair AutoReplace List, replace the input string for each one
		foreach (StringPair p in stringReplacer.values)
		{
			if (p.matchCase == true)
			{
				MatchCollection mcMatches = Regex.Matches(tmpUGUIField.text, "(?<!(=|=\"))\\b" + p.inputString, RegexOptions.IgnoreCase);
				foreach (Match m in mcMatches)
				{
					tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, "(?<!(=|=\"))\\b" + m.Value, "<style=Keyword>" + m.Value + "</style>");
				}
			}
			else
			{
				MatchCollection matches = Regex.Matches(tmpUGUIField.text, "(?<!(=|=\"))\\b(" + p.inputString + ")\\w*", RegexOptions.IgnoreCase);
				foreach (Match m in matches)
				{
					if (ValidateStyle(m.Index) == false) continue;
					string suffix = Regex.Replace(m.Value, p.inputString, "", RegexOptions.IgnoreCase);
					tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, "(?<!(=|=\"))\\b(?<!\"\\>)" + m.Value, "<style=Keyword>" + p.replacedString + suffix + "</style>");
				}
			}
		}

		// Replace the Custom Keywords
		foreach (CustomKeywordData k in KWData)
		{
			if (string.IsNullOrEmpty(k.label)) continue;
			if (k.label.Length < 2) continue;

			// Run the custom keyword name thru regex
			string regexLabel = k.label;
			regexLabel = Regex.Replace(regexLabel, "[\\[\\]{}]", "");

			string ks = k.spriteIndex > 0 ?
				"<color=" + k.hexColor + "><sprite name=\"Custom_" + k.spriteIndex + "\" tint>" + "</color><style=Keyword>" + regexLabel + "</style>" :
				"<style=Keyword>" + regexLabel + "</style>";

			tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, "(?<!(=|=\"))\\b" + regexLabel + "\\b", ks, RegexOptions.IgnoreCase);
		}

		// Regular spellcheck auto-correct
		foreach (StringPair r in regularTextReplacer.values)
		{
			tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, "\\b" + r.inputString + "\\b", r.replacedString);
		}

		// Fix colon colour tag
		tmpUGUIField.text = tmpUGUIField.text.Replace(":</style>", "</style>:");

		// Fix slashes
		tmpUGUIField.text = tmpUGUIField.text.Replace("/+", "|+");
	}

	private bool ValidateStyle(int i)
	{
		List<int> cardOpenTagList = new List<int>();
		List<int> cardClosingTagList = new List<int>();

		MatchCollection openMatches = Regex.Matches(tmpUGUIField.text, "<style=Card>");
		MatchCollection closeMatches = Regex.Matches(tmpUGUIField.text, "</style>");

		if (openMatches.Count < 1) return true;

		foreach(Match om in openMatches) cardOpenTagList.Add(om.Index);
		foreach(Match cm in closeMatches) cardClosingTagList.Add(cm.Index);

		List<Vector2> cardTagRange = new List<Vector2>();
		foreach (int oi in cardOpenTagList)
		{
			foreach (int ci in cardClosingTagList)
			{
				if (ci < oi) continue;
				else
				{
					cardTagRange.Add(new Vector2(oi, ci));
				}
			}
		}

		foreach (Vector2 v in cardTagRange)
		{
			if (i > v.x && i < v.y)
			{
				return false;
			}
		}
		return true;
	}
}