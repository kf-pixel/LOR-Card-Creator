using UnityEngine;
using UnityEngine.EventSystems;

public class FrameRateHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		FrameRateManager.Instance.RequestFullFrameRate();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		FrameRateManager.Instance.RequestFullFrameRate();
	}
}
