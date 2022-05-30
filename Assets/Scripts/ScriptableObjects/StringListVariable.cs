using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New StringListVariable", menuName = "String List Variable", order = 0)]
public class StringListVariable : ScriptableObject
{
	public List<string> values = new List<string>();

    private void OnEnable()
    {
		values.Clear();
	}
}