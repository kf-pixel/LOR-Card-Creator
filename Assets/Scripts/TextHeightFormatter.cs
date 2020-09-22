using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class TextHeightFormatter : MonoBehaviour
{
	[SerializeField] private RectTransform rect_reference;
	[SerializeField] private bool inverse = false;
	[SerializeField] private float y_reference;
	[SerializeField] private float spacerDistance = 10f;
	[SerializeField] private UnityFloatEvent yTransformEvent;

	public void SetYPosition(float height)
	{
		float yp = y_reference;
		if (rect_reference != null)
			yp += rect_reference.localPosition.y;

		if (!inverse)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, yp + height + spacerDistance);
		}
		else
		{
			transform.localPosition = new Vector3(transform.localPosition.x, yp - height + spacerDistance);
		}
		yTransformEvent.Invoke(transform.localPosition.y);
	}

	public void SetYHeight(float height)
	{
		RectTransform thisRect = GetComponent<RectTransform>();

		if (!inverse)
		{
			thisRect.sizeDelta = new Vector2(thisRect.sizeDelta.x, spacerDistance + height);
		}
		else
		{
			thisRect.sizeDelta = new Vector2(thisRect.sizeDelta.x, spacerDistance - height);
		}
	}

	public void NewYReference(float f)
	{
		y_reference = f;
	}
}