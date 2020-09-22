using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new Float", menuName = "Float Variable")]
public class FloatVariable : ScriptableObject
{
	[System.NonSerialized] public float value;
	public UnityEvent valueChangedEvent;

	public void NewInt(float i)
	{
		value = i;
		valueChangedEvent.Invoke();
	}

	public void StringToFloat(string s)
	{
		if (s == null) return;
		var sf = 0f;
		var ss = float.TryParse(s, out sf);
		value = sf;
		valueChangedEvent.Invoke();
	}

	public void RectToFloatX(RectTransform r)
	{
		value = r.localPosition.x;
	}

	public void RectToFloatY(RectTransform r)
	{
		value = r.localPosition.y;
	}
}