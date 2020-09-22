using UnityEngine;

public class RectTransformScale : MonoBehaviour
{
	[SerializeField] private RectTransform rect;

	public void SetScale(float f)
	{
		// Set Image width x height
		float yAspectRatio = (float)rect.sizeDelta.y / (float)rect.sizeDelta.x;
		rect.sizeDelta = new Vector2(f, f * yAspectRatio);
	}
}