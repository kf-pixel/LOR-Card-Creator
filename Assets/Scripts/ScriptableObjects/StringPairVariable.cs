using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="string pairs", menuName ="New String Pairs")]
public class StringPairVariable : ScriptableObject
{
	public List<StringPair> values = new List<StringPair>();

	public void Change()
	{
		foreach (StringPair sp in values)
		{
			sp.replacedString = sp.replacedString.Replace("<color=#fad65a>", "");
		}
	}
}

[System.Serializable]
public class StringPair
{
	public string inputString;
	public string replacedString;
	public bool matchCase = false;
	public bool matchExactOnly = false;
}