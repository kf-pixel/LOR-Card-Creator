using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "new Sprites", menuName = "Sprite List")]
public class SpritesVariable : ScriptableObject
{
	public List<Sprite> values = new List<Sprite>();
}