using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionManager : MonoBehaviour
{
	[SerializeField] private GameObject resolutionButtonPrefab;
	[SerializeField] private List<Vector2> resolutions = new List<Vector2>();
	private List<GameObject> resolutionButtons = new List<GameObject>();
	private List<int> availableWidths = new List<int> { 1280, 1600, 1920, 2560, 3440, 3840};
	private void Start() => GetAvailableResolutions();
	private void GetAvailableResolutions()
	{
		resolutions.Clear();
		foreach (Resolution res in Screen.resolutions)
		{
			Vector2 resVector = new Vector2(res.width, res.height);

			if (resolutions.Contains(resVector) == true) continue;
			if ((resVector.x / resVector.y) <= 1.7f) continue;
			if (availableWidths.Contains((int)resVector.x) == false) continue;

			resolutions.Add(resVector);
		}
		GenerateResolutionButtons();
	}

	private void GenerateResolutionButtons()
	{
		for (int i = resolutionButtons.Count - 1; i >= 0 ; i--)
		{
			Destroy(resolutionButtons[i].gameObject);
		}

		for (int i = 0; i < resolutions.Count; i++)
		{
			GameObject buttonGameObject = Instantiate(resolutionButtonPrefab, transform);
			resolutionButtons.Add(buttonGameObject);

			TextMeshProUGUI tmp = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>();
			tmp.text = resolutions[i].x + "x" + resolutions[i].y;

			int localIndex = i;
			Button button = buttonGameObject.GetComponent<Button>();
			button.onClick.AddListener(()=> SetResolution(localIndex));
		}
	}

	public void SetResolution(int i)
	{
		Screen.SetResolution((int)resolutions[i].x, (int)resolutions[i].y, Screen.fullScreen);
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