using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LOLChampionNamesAssets))]
public class EditorChampionAssetsGet : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		LOLChampionNamesAssets champList = target as LOLChampionNamesAssets;
		if (GUILayout.Button("Get All Champions"))
		{
			string[] foldersToSearch = new string[1] { "Assets/Resources/LOL API Data" };
			List<TextAsset> allAssets = new List<TextAsset>(EditorExtension.GetAllInstances<TextAsset>(foldersToSearch));
			//foreach (TextAsset t in allAssets)
			{
				//champList.AddChampion(new LOLChampionNamesAssets.ListNamesAssets(t.name, t));
			}
		}
	}
}
