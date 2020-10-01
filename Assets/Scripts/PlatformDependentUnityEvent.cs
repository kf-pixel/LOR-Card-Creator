using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlatformDependentUnityEvent : MonoBehaviour
{
	[SerializeField] private UnityEvent editorStandaloneEvent;
	[SerializeField] private UnityEvent webGLEvent;

	private void Awake()
	{
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
