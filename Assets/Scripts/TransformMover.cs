using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TransformMover : MonoBehaviour
{
	[SerializeField] private BoolVariable toggleAuto;
	[SerializeField] private float autoHeightDifference = -378;
	[SerializeField] private float maxAutoHeight = -230;
	[SerializeField] private Slider shadowSlider;

	public void MoveY(float y)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, y);
	}

	public void AutoHeight(float y)
	{
		if (toggleAuto.value == true) return;
		transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(y + autoHeightDifference, -1000, maxAutoHeight));
	}

	public void SetFromSlider()
	{
		transform.localPosition = new Vector3(transform.localPosition.x, shadowSlider.value);
	}
}