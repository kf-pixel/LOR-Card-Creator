using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpFrameControl : MonoBehaviour
{
	[SerializeField] private IntVariable cardType;
	[SerializeField] private StringVariable levelUpString;
	[SerializeField] private GameObject levelUpFrame, levelUpText;

	public void LevelUpFrameEnabler()
	{
        if (cardType.value != 2) return;
		bool empty = !string.IsNullOrEmpty(levelUpString.value);
		levelUpFrame.SetActive(empty);
		levelUpText.SetActive(empty);
	}
}
