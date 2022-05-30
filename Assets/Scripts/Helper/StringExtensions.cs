using System.Globalization;
using System.Text.RegularExpressions;
public static class StringExtensions
{
	private static string richTextPattern = "<.*?=";
	private static string titleCaseWordPatterns = "(?<= )(The|A|An|As|For|Or|To|Of)(?= )";
	private static string latinWordPattern = "'\\w(?=\\w)";

	public static string MatchReplace(this Match match, string source, string replacement)
	{
		return source.Substring(0, match.Index) + replacement + source.Substring(match.Index + match.Length);
	}

	/// <summary>
	/// Use the current thread's culture info for conversion
	/// </summary>
	public static string ToTitleCase(this string str)
	{
		var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
		str = cultureInfo.TextInfo.ToTitleCase(str);

		// Lowercase TMP RichText tags
		MatchCollection richTextMatches = Regex.Matches(str, richTextPattern);
		foreach (Match rtm in richTextMatches)
		{
			str = MatchReplace(rtm, str, rtm.Value.ToLower());
		}

		// TitleCase Words
		MatchCollection titleCaseWords = Regex.Matches(str, titleCaseWordPatterns);
		foreach (Match tcw in titleCaseWords)
		{
			str = MatchReplace(tcw, str, tcw.Value.ToLower());
		}

		// Latin Words
		MatchCollection latinWords = Regex.Matches(str, latinWordPattern);
		foreach (Match lw in latinWords)
		{
			str = MatchReplace(lw, str, lw.Value.ToUpper());
		}

		return str;
	}

	/// <summary>
	/// Overload which uses the culture info with the specified name
	/// </summary>
	public static string ToTitleCase(this string str, string cultureInfoName)
	{
		var cultureInfo = new CultureInfo(cultureInfoName);
		return cultureInfo.TextInfo.ToTitleCase(str);
	}

	/// <summary>
	/// Overload which uses the specified culture info
	/// </summary>
	public static string ToTitleCase(this string str, CultureInfo cultureInfo)
	{
		return cultureInfo.TextInfo.ToTitleCase(str);
	}
}
