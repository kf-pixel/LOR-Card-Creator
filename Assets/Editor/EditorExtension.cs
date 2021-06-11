using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorExtension
{
	public static T[] GetAllInstances<T>(string[] folders) where T : Object
	{
		string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, folders);  //FindAssets uses tags check documentation for more info
		T[] a = new T[guids.Length];
		for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
		{
			string path = AssetDatabase.GUIDToAssetPath(guids[i]);
			a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
		}

		return a;

	}
}
