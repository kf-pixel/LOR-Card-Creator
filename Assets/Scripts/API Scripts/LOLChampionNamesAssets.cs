using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LOLChampionNamesAssets", menuName = "LOLChampionNamesAssets File")]
public class LOLChampionNamesAssets : ScriptableObject
{
	public List<ListNamesAssets> championNamesAssets;

	public void AddChampion(ListNamesAssets a)
	{
		championNamesAssets.Add(a);
	}
}

[System.Serializable]
public class ListNamesAssets
{
	public List<string> names;
	public TextAsset asset;

	public ListNamesAssets()
	{
		names = new List<string>();
		asset = null;
	}

	public ListNamesAssets(string firstName, TextAsset ta)
	{
		names = new List<string>();
		names.Add(firstName);
		asset = ta;
	}
}

