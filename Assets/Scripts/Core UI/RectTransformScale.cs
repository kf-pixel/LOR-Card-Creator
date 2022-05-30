using UnityEngine;

public class RectTransformScale : MonoBehaviour
{
	[SerializeField] private RectTransform rect;
	[SerializeField] private Transform pivotPoint;

	[Header("Size Setting")]
	[SerializeField] private Vector2[] sizeValues;
	[SerializeField] private Vector2[] positionValues;

	public void SetScale(float f)
	{
		FrameRateManager.Instance.RequestFullFrameRate();

		// Set Image width x height
		float yAspectRatio = (float)rect.sizeDelta.y / (float)rect.sizeDelta.x;
		//rect.sizeDelta = new Vector2(f, f * yAspectRatio);
		ScaleAround(f, yAspectRatio);
	}

	public void ResetPosition()
	{
		FrameRateManager.Instance.RequestFullFrameRate();

		rect.anchoredPosition = new Vector3(0, 0);
	}

	public void ScaleAround(float newScale, float aspectRatio)
	{
		FrameRateManager.Instance.RequestFullFrameRate();

		Vector3 A = transform.position;
		Vector3 B = pivotPoint.transform.position;

		Vector3 C = A - B; // diff from object pivot to desired pivot/origin

		float RS = newScale / rect.sizeDelta.x; // relative scale factor

		// calc final position post-scale
		Vector3 FP = B + C * RS;

		// finally, actually perform the scale/translation
		rect.sizeDelta = new Vector2(newScale, newScale * aspectRatio);
		transform.position = FP;
	}

	public void SetSize(int i)
	{
		FrameRateManager.Instance.RequestFullFrameRate();

		rect.sizeDelta = sizeValues[i];
		if (positionValues[i] != Vector2.zero)
		{
			rect.anchoredPosition = positionValues[i];
		}
	}
}