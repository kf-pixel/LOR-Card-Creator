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
		Screen.SetResolution((int)resolutions[i].x, (int)resolutions[i].y, Screen.fullScreen);
	}

	public void ToggleFullscreen()
	{
		if (Screen.fullScreen)
		{
			Screen.fullScreenMode = FullScreenMode.Windowed;
		}
		else
		{
			Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
		}
	}

	public void SetNativeResolution()
	{
		if (Screen.resolutions == null) return;
		if (Screen.resolutions.Length > 0)
		{
			Screen.SetResolution(Screen.resolutions[Screen.resolutions.Length - 1].width, Screen.resolutions[Screen.resolutions.Length - 1].height, Screen.fullScreen);
		}
	}
}