using UnityEngine;
using UnityEngine.UI;

public class ImageColourChanger : MonoBehaviour
{
	[SerializeField] private Image img;
	[SerializeField] private IntVariable intReference;
	[SerializeField] private Color[] colors;

	private void Start()
	{
		ChangeColour();
	}

	public void ChangeColour()
	{
		img.color = new Color(colors[intReference.value].r, colors[intReference.value].g, colors[intReference.value].b, img.color.a);
	}
}