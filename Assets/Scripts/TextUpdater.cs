using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextUpdater : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI tmp;
	[SerializeField] private StringVariable reference;
	[SerializeField] private bool updateOnAwake = true;

	[Header("Text Formatting")]
	[SerializeField] private float textHeight;
	//[SerializeField] private float checkTextHeightDelay = 0.2f;
	[SerializeField] private UnityFloatEvent newTextHeightEvent;

	private void Awake()
	{
		if (updateOnAwake)
		{
			reference.value = tmp.text;
		}
		RefreshTextHeight();
	}

	private void OnEnable()
	{
		UpdateText();
	}

	public void UpdateText()
	{
		tmp.text = reference.value;
		AddTags();
		RefreshTextHeight();
	}

	public void RefreshTextHeight()
	{
		StartCoroutine(CoroutineTextHeight());
	}

	private IEnumerator CoroutineTextHeight()
	{
		//yield return new WaitForSeconds(checkTextHeightDelay);
		yield return new WaitForEndOfFrame();

		if (!System.String.IsNullOrEmpty(tmp.text))
		{
			textHeight = tmp.renderedHeight;
		}
		else
		{
			textHeight = 0f;
		}

		newTextHeightEvent.Invoke(textHeight);
	}

	private void AddTags()
	{
		if (tmp.text == null)
		{
			return;
		}

		if (tmp.text.Contains("[k]") && tmp.text.Contains("[/k]"))
		{
			tmp.text = tmp.text.Replace("[k]", "<color=#F0CC70>");
			tmp.text = tmp.text.Replace("[/k]", "</color>");
		}

		if (tmp.text.Contains("[c]") && tmp.text.Contains("[/c]"))
		{
			tmp.text = tmp.text.Replace("[c]", "<color=#4699ED>");
			tmp.text = tmp.text.Replace("[/c]", "</color>");
		}

		// [] and {} tags
		if (tmp.text.Contains("[") && tmp.text.Contains("]"))
		{
			tmp.text = tmp.text.Replace("[", "<color=#4699ED>");
			tmp.text = tmp.text.Replace("]", "</color>");
		}

		if (tmp.text.Contains("{") && tmp.text.Contains("}"))
		{
			tmp.text = tmp.text.Replace("{", "<color=#F0CC70>");
			tmp.text = tmp.text.Replace("}", "</color>");
		}

		// New line tag
		tmp.text = tmp.text.Replace("`", "<br>");

		// Skill Sprite
		tmp.text = tmp.text.Replace("@", "<sprite name=\"skill\">");

		// Italics
		tmp.text = tmp.text.Replace("(", "<i>(");
		tmp.text = tmp.text.Replace(")", ")</i>");
	}
}

[System.Serializable]
public class UnityFloatEvent : UnityEvent<float>
{

}