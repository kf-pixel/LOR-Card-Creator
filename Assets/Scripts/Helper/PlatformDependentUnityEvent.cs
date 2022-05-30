using UnityEngine;
using UnityEngine.Events;

public class PlatformDependentUnityEvent : MonoBehaviour
{
	[SerializeField] private UnityEvent editorStandaloneEvent, webGLEvent, mobileEvent;

	private void Awake()
	{
#if UNITY_ANDROID
		mobileEvent.Invoke();
		return;
#endif

#if UNITY_EDITOR
		editorStandaloneEvent.Invoke();
		return;
#endif

#if UNITY_STANDALONE
		editorStandaloneEvent.Invoke();
		return;
#endif

#if UNITY_WEBGL
		webGLEvent.Invoke();
		return;
#endif
	}
}
