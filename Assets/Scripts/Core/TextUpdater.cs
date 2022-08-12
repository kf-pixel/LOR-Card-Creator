using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextUpdater : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI tmp;
	[SerializeField] private TMP_InputField inputTMP;
	[SerializeField] private StringVariable reference;
	[SerializeField] private bool updateOnAwake = true;

	[Header("Text Formatting")]
	[SerializeField] private string prefix;
	[SerializeField] private float textHeight;
	[SerializeField] private UnityFloatEvent newTextHeightEvent;

	[Header("Special Cases")]
	[SerializeField] private string countdownPattern;

	private void Awake()
	{
		if (updateOnAwake)
		{
			reference.value = tmp.text;
		}
	}

	private void OnEnable()
	{
		UpdateText();
	}

	public void UpdateText()
	{
		tmp.text = !string.IsNullOrEmpty(reference.value) ? prefix + reference.value : "";
		AddTags();

		if (inputTMP != null)
		{
			inputTMP.text = !string.IsNullOrEmpty(reference.value) ? prefix + reference.value : "";
			AddInputTMPTags();
		}
		
		StartCoroutine(TextUpdateIE());
	}
	private IEnumerator TextUpdateIE()
	{
		yield return new WaitForEndOfFrame();

		textHeight = Mathf.Max(tmp.renderedHeight, 0f);
		newTextHeightEvent.Invoke(textHeight);
	}

	private void AddInputTMPTags()
	{
		// Exit if empty of exceeding max length
		if (string.IsNullOrEmpty(inputTMP.text))
		{
			return;
		}
		else if (inputTMP.text.Length > 2000)
		{
			return;
		}

		inputTMP.text = inputTMP.text.Replace("`", "<br>");

		// [] and {} tags
		// alt [] text w/o formatting
		if (inputTMP.text.Contains("[[") && inputTMP.text.Contains("]]"))
		{
			inputTMP.text = inputTMP.text.Replace("[[", "<color=#49a0f8>");
			inputTMP.text = inputTMP.text.Replace("]]", "</color>");
		}

		if (inputTMP.text.Contains("[") && inputTMP.text.Contains("]"))
		{
			//inputTMP.text = inputTMP.text.Replace("[", "<style=Card>");
			//inputTMP.text = inputTMP.text.Replace("]", "</STYLE>");
		}

		if (inputTMP.text.Contains("{") && inputTMP.text.Contains("}"))
		{
			inputTMP.text = inputTMP.text.Replace("{", "<style=Keyword>");
			inputTMP.text = inputTMP.text.Replace("}", "</style>");
		}




		// double slash break
		inputTMP.text = inputTMP.text.Replace("//", "<b></b>");

		// Skill Sprite
		inputTMP.text = inputTMP.text.Replace("@", "<sprite name=skill>");

		// Countdown
		if (string.IsNullOrEmpty(countdownPattern)) return;

		Regex rgx = new Regex(countdownPattern, RegexOptions.IgnoreCase);
		MatchCollection matches = rgx.Matches(inputTMP.text);
		foreach (Match match in matches)
		{
			inputTMP.text = inputTMP.text.Replace(match.Value, $"<style=Keyword>{match.Value}</style>");
		}
	}

	private void AddTags()
	{
		if (string.IsNullOrEmpty(tmp.text))
		{
			return;
		}

		tmp.text = tmp.text.Replace("`", "<br>");

		// [] and {} tags
		// alt [] text w/o formatting
		if (tmp.text.Contains("[[") && tmp.text.Contains("]]"))
		{
			tmp.text = tmp.text.Replace("[[", "<color=#49a0f8>");
			tmp.text = tmp.text.Replace("]]", "</color>");
		}
		if (tmp.text.Contains("[") && tmp.text.Contains("]"))
		{
			tmp.text = tmp.text.Replace("[", "<style=Card>");
			tmp.text = tmp.text.Replace("]", "</STYLE>");
		}

		if (tmp.text.Contains("{") && tmp.text.Contains("}"))
		{
			tmp.text = tmp.text.Replace("{", "<style=Keyword>");
			tmp.text = tmp.text.Replace("}", "</style>");
		}

		// double slash break
		tmp.text = tmp.text.Replace("//", "<b></b>");

		// Skill Sprite
		tmp.text = tmp.text.Replace("@", "<sprite name=skill>");

		// Countdown
		if (string.IsNullOrEmpty(countdownPattern)) return;

		Regex rgx = new Regex(countdownPattern, RegexOptions.IgnoreCase);
		MatchCollection matches = rgx.Matches(tmp.text);
		foreach (Match match in matches)
		{
			tmp.text = tmp.text.Replace(match.Value, $"<style=Keyword>{match.Value}</style>");
		}
	}
}

[System.Serializable]
public class UnityFloatEvent : UnityEvent<float>
{

}