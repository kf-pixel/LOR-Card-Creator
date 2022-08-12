using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] private ColourStyle style;
	private Image baseImage, childImage;
	private TextMeshProUGUI baseTMP, childTMP;
	private Color baseImageColor, baseTMPColor, childImageColor, childTMPColor;
	private Material baseMaterial, hoverMaterial;
	private Slider slider;
	private Toggle toggle;
	private TMP_Dropdown dropdown;
	[SerializeField] private bool enablePressing = true; // for the upload button specifically

	// Internal
	private bool hovering;
	private bool pressing;
	private bool toggleValue { get { return (toggle != null) ? toggle.isOn : false; } }
	private bool dropdownExpanded { get { return (dropdown != null) ? dropdown.IsExpanded : false; } }

	private void Awake()
	{
		TryGetComponent<Image>(out baseImage);
		TryGetComponent<TextMeshProUGUI>(out baseTMP);
		TryGetComponent<Slider>(out slider);
		TryGetComponent<Toggle>(out toggle);
		TryGetComponent<TMP_Dropdown>(out dropdown);
		childTMP = GetComponentInChildren<TextMeshProUGUI>();

		if (slider != null)
		{
			slider.fillRect.TryGetComponent<Image>(out baseImage);
			slider.handleRect.TryGetComponent<Image>(out childImage);
		}
		else if (toggle != null)
		{
			if (baseImage == null)
			{
				baseTMP = childTMP;
				childTMP = null;
			}
			else
			{
				toggle.targetGraphic?.TryGetComponent<Image>(out baseImage);
			}
			toggle.graphic?.TryGetComponent<Image>(out childImage);
		}
		else
		{
			childImage = GetComponentInChildren<Image>();
		}

		if (baseImage != null && style.hoverMaterial != null)
		{
			baseMaterial = baseImage?.material;
			hoverMaterial = style.hoverMaterial;
		}

		baseImageColor = baseImage ? baseImage.color : default;
		baseTMPColor = baseTMP ? baseTMP.color : default;
		childImageColor = childImage ? childImage.color : default;
		childTMPColor = childTMP ? childTMP.color : default;

		if (toggle != null)
		{
			toggle.onValueChanged.AddListener(delegate { ToggleHighlights(); });
			ToggleHighlights();
		}
	}

	private void OnDisable()
	{
		hovering = false;
		pressing = false;
		if (toggle != null)
		{
			toggle.onValueChanged.RemoveListener(delegate { ToggleHighlights(); });
			ToggleHighlights();
		}
		else
		{
			ResetHighlights();
		}
	}


	public void ResetHighlights()
	{
		if (baseImage != null) baseImage.color = baseImageColor;
		if (baseTMP != null) baseTMP.color = baseTMPColor;
		if (childImage != null) childImage.color = childImageColor;
		if (childTMP != null) childTMP.color = childTMPColor;

		if (hoverMaterial != null) baseImage.material = baseMaterial;

		if (dropdown != null)
		{
			if (dropdownExpanded == true)
			{
				HoverHighlights();
			}
		}
	}

	public void DropdownResetHighlights()
	{
		if (baseImage != null) baseImage.color = baseImageColor;
		if (baseTMP != null) baseTMP.color = baseTMPColor;
		if (childImage != null) childImage.color = childImageColor;
		if (childTMP != null) childTMP.color = childTMPColor;
	}

	public void HoverHighlights()
	{
		if (baseImage != null) baseImage.color = style.hover;
		if (baseTMP != null) baseTMP.color = style.hover;
		if (childImage != null) childImage.color = style.hover;
		if (childTMP != null) childTMP.color = style.hoverText;

		if (hoverMaterial != null) baseImage.material = hoverMaterial;
	}

	public void PressedHighlights()
	{
		if (baseImage != null) baseImage.color = style.pressed;
		if (baseTMP != null) baseTMP.color = style.pressed;
		if (childImage != null) childImage.color = style.pressed;
		if (childTMP != null) childTMP.color = style.pressedText;

		if (hoverMaterial != null) baseImage.material = hoverMaterial;
	}

	public void ToggleHighlights()
	{
		if (toggleValue)
		{
			if (childImage != null)
			{
				childImage.color = style.activeText;
				if (baseTMP != null)
				{
					baseTMP.color = style.activeText;
				}
			}
			if (childTMP != null)
			{
				baseImage.color = style.active;
				if (childTMP != null)
				{
					childTMP.color = style.activeText;
				}
			}

			if (hoverMaterial != null) baseImage.material = baseMaterial;
		}
		else if (hovering)
		{
			HoverHighlights();
		}
		else
		{
			ResetHighlights();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		FrameRateManager.Instance.RequestFullFrameRate();
		hovering = true;
		if (!pressing)
		{
			HoverHighlights();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		FrameRateManager.Instance.RequestFullFrameRate();
		hovering = false;

		if (toggle != null)
		{
			ToggleHighlights();
		}
		else if (!pressing)
		{
			ResetHighlights();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!enablePressing) return;
		PressedHighlights();
		pressing = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (toggle != null)
		{
			ToggleHighlights();
		}
		else if (hovering)
		{
			HoverHighlights();
		}
		else
		{
			ResetHighlights();
		}
		pressing = false;
	}
}
