﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSafeArea : MonoBehaviour
{
	public Canvas canvas;
	private RectTransform panelSafeArea;
	private Rect currentSafeArea = new Rect();
	private ScreenOrientation currentOrientation = ScreenOrientation.Portrait;

    private void Start()
    {
		panelSafeArea = GetComponent<RectTransform>();

		currentOrientation = Screen.orientation;
		currentSafeArea = Screen.safeArea;
		ApplySafeArea();
	}

	private void ApplySafeArea()
	{
        if (panelSafeArea == null) return;

		Rect safeArea = Screen.safeArea;

		Vector2 anchorMin = safeArea.position;
		Vector2 anchorMax = safeArea.position + safeArea.size;

		anchorMin.x /= canvas.pixelRect.width;
		anchorMin.y /= canvas.pixelRect.height;

		anchorMax.x /= canvas.pixelRect.width;
		anchorMax.y /= canvas.pixelRect.height;

		panelSafeArea.anchorMin = anchorMin;
		panelSafeArea.anchorMax = anchorMax;

		currentOrientation = Screen.orientation;
		currentSafeArea = Screen.safeArea;
	}

    private void Update()
    {
		if ((currentOrientation != Screen.orientation || currentSafeArea != Screen.safeArea))
		{
			ApplySafeArea();
		}
    }

}
