using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using DG.Tweening;

public class FrameRateManager : MonoBehaviour
{
	public static FrameRateManager Instance { get; private set; }
	[SerializeField] private int targetFrames = 60;
	private int lastRequestedFrame = 0;
	private const int BUFFER_FRAMES = 2;
	private const int LOW_POWER_FRAME_INTERVAL = 30;
	private bool active = true;

	private void Awake()
	{
		// Singleton Pattern
		if (Instance != null && Instance != this)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}

		// Platform-dependent setup 

#if UNITY_ANDROID
        targetFrames = 30;
#endif
		Application.targetFrameRate = targetFrames;

#if UNITY_EDITOR
		active = false;
#endif
	}

	private void OnDisable()
	{
		OnDemandRendering.renderFrameInterval = 1;
	}

	public void RequestFullFrameRate()
	{
		if (Time.frameCount > lastRequestedFrame)
		{
			lastRequestedFrame = Time.frameCount;
		}
	}

	public void RequestOneSecondFullFrameRate()
	{
		lastRequestedFrame = Time.frameCount + LOW_POWER_FRAME_INTERVAL;
	}
	public void RequestHalfSecondFullFrameRate()
	{
		lastRequestedFrame = Time.frameCount + (int)(LOW_POWER_FRAME_INTERVAL / 2);
	}

	public void EnableFRManager()
	{
		active = true;
	}

	public void DisableFRManager()
	{
		active = false;
	}

	public void ClearEventSystem()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}

	private void Update()
	{
		if (!active)
		{
			OnDemandRendering.renderFrameInterval = 1;
			return;
		}

		if (Input.anyKey)
		{
			RequestFullFrameRate();
		}
		else if (EventSystem.current.currentSelectedGameObject != null)
		{
			if (EventSystem.current.currentSelectedGameObject.name.StartsWith("Item") || EventSystem.current.currentSelectedGameObject.name.StartsWith("Input"))
			{
				RequestHalfSecondFullFrameRate();
			}
		}
		else if (DOTween.IsTweening("0", true))
		{
			RequestFullFrameRate();
		}

		OnDemandRendering.renderFrameInterval = (Time.frameCount - lastRequestedFrame) < BUFFER_FRAMES ? 1 : targetFrames;
	}
}
