using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;

public class AutoTextReplacer : MonoBehaviour
{
	[SerializeField] private BoolVariable disableAutoReplace;
	[SerializeField] private TextMeshProUGUI tmpUGUIField;
	[SerializeField] private StringPairVariable stringReplacer, regularTextReplacer;

	[Header("Card Name List Grab")]
	[SerializeField] private BoolVariable disableBlueText;
	[SerializeField] private ListManager listManager;
	[SerializeField] private StringListVariable cardNamesList;

	[Header("Custom Keyword Data")]
	[SerializeField] private CustomKeywordData[] KWData;

	public void ReplaceTMPUGUI()
	{
		// Exit if empty of exceeding max length
		if (string.IsNullOrEmpty(tmpUGUIField.text))
		{
			return;
		}
		else if (tmpUGUIField.text.Length > 2000)
		{
			return;
		}

		if (disableAutoReplace.value == true)
		{
			if (disableBlueText.value == false)
			{
				AutoBlueText();
				TitleCaseBlueText();
			}
			return;
		}

		// insert spaces
		MatchCollection spaces = Regex.Matches(tmpUGUIField.text, "(\\.|:|,)(?=\\w)");
		for (int i = spaces.Count - 1; i >= 0; i--)
		{
			tmpUGUIField.text = tmpUGUIField.text.Insert(spaces[i].Index + 1, " ");
		}
		// Capitalize letters
		MatchCollection capitals = Regex.Matches(tmpUGUIField.text, "(?<=(: |\\. *(<br>)? *)|^)\\w");
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
					if (ValidateStyle(m.Index) == false) continue;
					tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, "(?<!(=|=\"))\\b" + m.Value, "<style=Keyword>" + m.Value + "</style>");
				}
			}
			else if (p.matchExactOnly == true)
			{
				MatchCollection matches = Regex.Matches(tmpUGUIField.text, "(?<!(=|=\"))\\b" + p.inputString + "\\b", RegexOptions.IgnoreCase);
				for (int i = matches.Count - 1; i >= 0 ; i--)
				{
					Match m = matches[i];
					if (ValidateStyle(m.Index) == false) continue;
					tmpUGUIField.text = tmpUGUIField.text.Remove(m.Index, m.Length);
					tmpUGUIField.text = tmpUGUIField.text.Insert(m.Index, "<style=Keyword>" + p.replacedString + "</style>");
				}
			}
			else
			{
				MatchCollection matches = Regex.Matches(tmpUGUIField.text, "(?<!(=|=\"))\\b(" + p.inputString + ")\\w*", RegexOptions.IgnoreCase);
				for (int i = matches.Count - 1; i >= 0 ; i--)
				{
					Match m = matches[i];
					if (ValidateStyle(m.Index) == false) continue;
					string suffix = Regex.Replace(m.Value, p.inputString, "", RegexOptions.IgnoreCase);
					tmpUGUIField.text = tmpUGUIField.text.Remove(m.Index, m.Length);
					tmpUGUIField.text = tmpUGUIField.text.Insert(m.Index, "<style=Keyword>" + p.replacedString + suffix + "</style>");
				}
			}
		}

		AutoBlueText();

		// Replace the Custom Keywords
		foreach (CustomKeywordData k in KWData)
		{
			if (string.IsNullOrEmpty(k.label)) continue;
			if (k.label.Length < 2) continue;

			// Run the custom keyword name thru regex
			string regexLabel = k.label;
			regexLabel = Regex.Replace(regexLabel, "[\\[\\]{}]", "");
			regexLabel = StringExtensions.ToTitleCase(regexLabel);

			string ks = "";
			if (k.spriteIndex <= 68 && k.spriteIndex > 0)
			{
				ks = $"<color={k.hexColor}><sprite name=\"Custom_{k.spriteIndex}\" tint></color>";
			}
			else if (k.spriteIndex >= 68) // user sprites
			{
				ks = $"<color={k.hexColor}><sprite name=\"user{k.spriteIndex - 68}\" tint></color>";
			}

			MatchCollection kwMatches = Regex.Matches(tmpUGUIField.text, "(?<!(=|=\"|<|\\/))\\b" + regexLabel + "\\w*", RegexOptions.IgnoreCase);
			for (int kwi = kwMatches.Count - 1; kwi >= 0; kwi--)
			{
				Match m = kwMatches[kwi];
				if (ValidateStyle(m.Index) == false) continue;
				string suffix = Regex.Replace(m.Value, regexLabel, "", RegexOptions.IgnoreCase);
				tmpUGUIField.text = tmpUGUIField.text.Remove(m.Index, m.Length);
				tmpUGUIField.text = tmpUGUIField.text.Insert(m.Index, ks + "<style=Keyword>" + regexLabel + suffix + "</style>");
			}
		}

		// Regular spellcheck auto-correct
		foreach (StringPair r in regularTextReplacer.values)
		{
			tmpUGUIField.text = Regex.Replace(tmpUGUIField.text, "\\b" + r.inputString + "\\b", r.replacedString);
		}

		// Fix colon colour tag
		tmpUGUIField.text = tmpUGUIField.text.Replace(":</style>", "</style>:");

		// Fix slashes into pipes
		MatchCollection statMatches = Regex.Matches(tmpUGUIField.text, "[+-]?\\d[/|\\|][+-]?\\d");
		for (int i = statMatches.Count - 1; i >= 0 ; i--)
		{
			tmpUGUIField.text = tmpUGUIField.text.Remove(statMatches[i].Index, statMatches[i].Length);
			tmpUGUIField.text = tmpUGUIField.text.Insert(statMatches[i].Index, $"<nobr>{statMatches[i].Value.Replace("/", "|")}</nobr>");
		}

		TitleCaseBlueText();
	}

	private bool ValidateStyle(int i)
	{
		List<int> cardOpenTagList = new List<int>();
		List<int> cardClosingTagList = new List<int>();

		MatchCollection openMatches = Regex.Matches(tmpUGUIField.text, "<style=Card>");
		MatchCollection closeMatches = Regex.Matches(tmpUGUIField.text, "</STYLE>");

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
					break;
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

	private void TitleCaseBlueText()
	{
		List<int> cardOpenTagList = new List<int>();
		List<int> cardClosingTagList = new List<int>();

		MatchCollection openMatches = Regex.Matches(tmpUGUIField.text, "<style=Card>");
		MatchCollection closeMatches = Regex.Matches(tmpUGUIField.text, "</STYLE>");

		if (openMatches.Count < 1) return;

		foreach (Match om in openMatches) cardOpenTagList.Add(om.Index + 12);
		foreach (Match cm in closeMatches) cardClosingTagList.Add(cm.Index);

		List<Vector2> cardTagRange = new List<Vector2>();
		foreach (int oi in cardOpenTagList)
		{
			foreach (int ci in cardClosingTagList)
			{
				if (ci < oi) continue;
				else
				{
					cardTagRange.Add(new Vector2(oi, ci));
					break;
				}
			}
		}

		foreach (Vector2 v in cardTagRange)
		{
			string tagged = tmpUGUIField.text.Substring((int)v.x, (int)v.y - (int)v.x);
			tagged = StringExtensions.ToTitleCase(tagged);

			tmpUGUIField.text = tmpUGUIField.text.Remove((int)v.x, (int)v.y - (int)v.x);
			tmpUGUIField.text = tmpUGUIField.text.Insert((int)v.x, tagged);
		}
	}

	private void AutoBlueText()
	{
		// Auto Tag Card Names
		if (listManager.activeItem.Count > 0 && disableBlueText.value == false)
		{
			if (listManager.activeItem[0] != null)
			{
				// Get an index range of cards to check from the List Manager
				List<string> checkedNames = new List<string>();
				int minRange = Mathf.Max(listManager.activeItem[0].listOrderIndex - 35, 0);
				int maxRange = Mathf.Min(listManager.activeItem[0].listOrderIndex + 35, listManager.GetListItemCount());
				string loopingCardName = "";

				// Get all card names
				for (int i = minRange; i < maxRange; i++)
				{
					string sanitizedName = Regex.Match(cardNamesList.values[i], "[\\w\\s',.:-]+").Value;

					if (checkedNames.Contains(sanitizedName)) continue;
					loopingCardName = sanitizedName;

					if (string.IsNullOrEmpty(loopingCardName)) continue;
					if (loopingCardName.Length < 3) continue;
					checkedNames.Add(loopingCardName);
				}
				// Order card names by length, then replace each
				checkedNames = checkedNames.OrderByDescending(x => x.Length).ToList();
				foreach (string s in checkedNames)
				{
					MatchCollection nameMatches = Regex.Matches(tmpUGUIField.text, $"(?<!(=|=\"|<|>|\\/))\\b{s}(s|'s)?\\b", RegexOptions.IgnoreCase);
					for (int ni = nameMatches.Count - 1; ni >= 0; ni--)
					{
						tmpUGUIField.text = tmpUGUIField.text.Remove(nameMatches[ni].Index, nameMatches[ni].Length);
						tmpUGUIField.text = tmpUGUIField.text.Insert(nameMatches[ni].Index, $"<style=Card>{nameMatches[ni].Value}</STYLE>");
					}
				}
			}
		}
	}
}