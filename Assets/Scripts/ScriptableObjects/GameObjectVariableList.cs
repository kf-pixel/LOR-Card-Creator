using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new GameObject List", menuName = "GameObject List")]
public class GameObjectVariableList : ScriptableObject
{
	public List<GameObject> value = new List<GameObject>();
}