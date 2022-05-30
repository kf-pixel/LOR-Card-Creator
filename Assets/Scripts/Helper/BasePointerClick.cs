using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BasePointerClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public UnityEvent onClickEvent;
	public UnityEvent onAltClickEvent;
	private LORInputActions input;
	protected bool isHovering;
	protected bool shift;
	protected bool ctrl;

	protected virtual void Awake()
	{
		input = new LORInputActions();
	}

	protected virtual void OnEnable()
	{
		input.Enable();
		input.UI.Click.performed += ctx => ClickInput();
		input.UI.AltClick.performed += ctx => AltClickInput();
		
		input.UI.Shift.performed += ctx => shift = true;
		input.UI.Shift.canceled += ctx => shift = false;

		input.UI.Ctrl.performed += ctx => ctrl = true;
		input.UI.Ctrl.canceled += ctx => ctrl = false;
	}

	protected virtual void OnDisable()
	{
		input.Disable();
		isHovering = false;
		shift = false;
		ctrl = false;

		input.UI.Click.performed -= ctx => ClickInput();
		input.UI.AltClick.performed -= ctx => AltClickInput();

		input.UI.Shift.performed -= ctx => shift = true;
		input.UI.Shift.canceled -= ctx => shift = false;
		
		input.UI.Ctrl.performed -= ctx => ctrl = true;
		input.UI.Ctrl.canceled -= ctx => ctrl = false;
	}

	public void ClickInput()
	{
		if (isHovering)
		{
			OnClick();
			onClickEvent.Invoke();
		}
	}

	public void AltClickInput()
	{
		if (isHovering)
		{
			OnAltClick();
			onAltClickEvent.Invoke();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isHovering = true;
		OnEnter();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isHovering = false;
		OnExit();
	}

	protected virtual void OnEnter()
	{

	}

	protected virtual void OnExit()
	{

	}

	protected virtual void OnClick()
	{

	}

	protected virtual void OnAltClick()
	{

	}
}