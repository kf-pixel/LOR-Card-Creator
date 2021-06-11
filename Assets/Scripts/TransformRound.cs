using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRound : MonoBehaviour
{
	public void Round(Vector2 pos)
	{
		transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y));
	}
}
