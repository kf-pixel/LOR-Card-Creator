using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace OneOfs
{
	public class BrushImage : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI brush;

		public void ToggleImage(string s)
		{
			brush.enabled = string.IsNullOrEmpty(s) ? false : true;
		}
	}
}