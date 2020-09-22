using UnityEngine;
using UnityEngine.UI;

public class FloatSetAlpha : MonoBehaviour
{
	[SerializeField] private Image img;

	public void SetAlpha(float f)
	{
		img.color = new Color(img.color.r, img.color.g, img.color.b, f);
	}
}