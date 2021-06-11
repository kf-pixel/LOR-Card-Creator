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
	[SerializeField] private Transform autoHeightReference; // takes the Card Title Transform local y position

	public void MoveY(float y)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, y);
		AutoHeight(autoHeightReference.localPosition.y);
	}

	public void AutoHeight(float y)
	{
		if (toggleAuto.value == true) return;
		transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Clamp(y + autoHeightDifference, -1000, maxAutoHeight));
	}

	public void SetFromSlider() // used when toggling the manualshadowfade
	{
		MoveY(shadowSlider.value);
	}
}