using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageColourChanger : MonoBehaviour
{
	[SerializeField] private Image img;
	[SerializeField] private TextMeshProUGUI tmp;
	[SerializeField] private IntVariable intReference;
	[SerializeField] private Color[] colors;

	private void Start()
	{
		if (intReference == null) return;
		ChangeColour();
	}

	public void ChangeColour()
	{
		img.color = new Color(colors[intReference.value].r, colors[intReference.value].g, colors[intReference.value].b, img.color.a);
	}

	public void ChangeColour(int i)
	{
		img.color = colors[i];
		if (tmp != null)
		{
			tmp.color = colors[i];
		}
	}
}