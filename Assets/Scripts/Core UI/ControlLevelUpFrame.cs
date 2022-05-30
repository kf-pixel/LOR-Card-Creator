using UnityEngine;

public class ControlLevelUpFrame : MonoBehaviour
{
	[SerializeField] private IntVariable cardType;
	[SerializeField] private StringVariable levelUpString;
	[SerializeField] private GameObject levelUpFrame, levelUpText;

	public void LevelUpFrameEnabler()
	{
        //if (cardType.value != 2 || cardType.value != 1) return;
		bool empty = !string.IsNullOrEmpty(levelUpString.value);
		levelUpFrame.SetActive(empty);
		levelUpText.SetActive(empty);
	}
}
