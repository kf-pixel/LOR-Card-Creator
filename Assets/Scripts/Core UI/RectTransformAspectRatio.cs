using UnityEngine;
public class RectTransformAspectRatio : MonoBehaviour
{
	[SerializeField] private RectTransform rectTransform;
	[SerializeField] private float height;
	private void OnEnable()
    {
		ApplySizeRatio();
	}

	public void ApplySizeRatio()
	{
		float aspectRatio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
		float refAspect = 0.5625f;
		rectTransform.sizeDelta = new Vector2(0, height * (aspectRatio / refAspect));
	}
}
