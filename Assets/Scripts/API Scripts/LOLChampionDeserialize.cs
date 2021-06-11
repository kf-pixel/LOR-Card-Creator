using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiotAPI;
using System.IO;

public class LOLChampionDeserialize : MonoBehaviour
{
	public static Root JSONLoad(TextAsset jsonFile)
	{
		string json = jsonFile.text;

		// Try open file
		try
		{
			Root champ = JsonUtility.FromJson<Root>(json);
			return champ;
		}
		catch
		{
			Debug.Log("Failed to Load File");
			return null;
		}
	}
}
