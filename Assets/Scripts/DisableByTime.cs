using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableByTime : MonoBehaviour
{
	public float time = 1f;
	public bool playOnStart = false;
	public bool playOnEnable = true;

	public IEnumerator Disable()
	{
		yield return new WaitForSeconds(time);
		gameObject.SetActive(false);
	}

	public IEnumerator Disable(float t)
	{
		yield return new WaitForSeconds(t);
		gameObject.SetActive(false);
	}

	public void StartDisable()
	{
		if (gameObject.activeInHierarchy) StartCoroutine(Disable());
	}

	private void Start()
	{
		if (playOnStart) StartCoroutine(Disable());
	}

	private void OnEnable()
	{
		if (playOnEnable) StartCoroutine(Disable());
	}
}