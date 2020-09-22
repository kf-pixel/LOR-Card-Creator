using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="string pairs", menuName ="New String Pairs")]
public class StringPairVariable : ScriptableObject
{
	public void Change()
	{
		foreach (StringPair sp in values)
		{
			//sp.replacedString = sp.replacedString.Replace("</color>", "");
		}
	}

	public List<StringPair> values = new List<StringPair>();
}

[System.Serializable]
public class StringPair
{
	public string inputString;
	public string replacedString;
}