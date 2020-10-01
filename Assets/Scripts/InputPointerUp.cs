using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputPointerUp : MonoBehaviour
{
	public UnityEvent onPointerUp;

	private void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			onPointerUp.Invoke();
		}
	}
}
