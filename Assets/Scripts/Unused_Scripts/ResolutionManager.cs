using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ResolutionManager : MonoBehaviour
{
	[SerializeField] private Vector2[] resolutions;

	public void SetResolution(int i)
	{
		if (Screen.currentResolution.width == (int)resolutions[i].x) return;

		bool isFullscreen = Screen.fullScreen;
		Screen.SetResolution((int)resolutions[i].x, (int)resolutions[i].y, isFullscreen);
	}
}