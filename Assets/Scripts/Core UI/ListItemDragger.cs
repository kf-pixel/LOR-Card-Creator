using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	public class ListItemDragger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler , IDragHandler, IEndDragHandler
	{
		[SerializeField] private RectTransform rt;
		[SerializeField] private bool canDrag = true;
		[SerializeField] private BoolVariable shiftInput, ctrlInput;

		[SerializeField] private ListItem listItem;
		[SerializeField] private UnityEvent onEnter;
		[SerializeField] private UnityEvent onExit;
		[SerializeField] private UnityEvent onDragBegin;
		[SerializeField] private UnityEvent onDragEnd;

		private Vector3 position;
		private bool dragging;
		private Vector3 mousePositionOnDragEnd;

		private void OnEnable()
		{
			PointerEventData pointerData = new PointerEventData(EventSystem.current);
			pointerData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointerData, results);

			foreach(RaycastResult ray in results)
			{
				if (ray.gameObject == this.gameObject)
				{
					onEnter.Invoke();
					break;
				}
			}
		}

		private void OnDisable()
		{
			onExit.Invoke();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			FrameRateManager.Instance.RequestFullFrameRate();
			onEnter.Invoke();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			FrameRateManager.Instance.RequestFullFrameRate();
			onExit.Invoke();
		}

		// Dragging

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!canDrag || shiftInput.value || ctrlInput.value) return;
			FrameRateManager.Instance.RequestFullFrameRate();
			dragging = true;
			onDragBegin.Invoke();
			position = transform.position;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!canDrag || !dragging) return;
			FrameRateManager.Instance.RequestFullFrameRate();
			Vector3 globalMousePos;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
			{
				rt.position = globalMousePos;
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (!dragging || !canDrag) return;
			FrameRateManager.Instance.RequestFullFrameRate();
			dragging = false;
			onDragEnd.Invoke();
			transform.position = position;

			Vector3 globalMousePos;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
			{
				listItem.SetOnDragEnd(globalMousePos);
			}

			onExit.Invoke();
		}
	}
}