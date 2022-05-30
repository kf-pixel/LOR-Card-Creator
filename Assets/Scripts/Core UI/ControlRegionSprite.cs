using UnityEngine;
using UnityEngine.UI;

public class ControlRegionSprite : MonoBehaviour
{
	[SerializeField] private Image img;
	[SerializeField] private IntVariable cardTypeIndex;
	[SerializeField] private IntVariable selectedRegion;
	[SerializeField] private SpritesVariable spritesList;

	[Header("Dual Region Data")]
	[SerializeField] private Image regionFrameImg;
	[SerializeField] private IntVariable otherRegion;
	[SerializeField] private SpritesVariable singleRegionFrames;
	[SerializeField] private SpritesVariable dualRegionFrames;

	private void OnEnable()
	{
		UpdateRegionSprite();
	}

	public void UpdateRegionSprite()
	{
		img.sprite = spritesList.values[selectedRegion.value];
		if (regionFrameImg != null)
		{
			regionFrameImg.sprite = selectedRegion.value == 13 ? singleRegionFrames.values[cardTypeIndex.value] : dualRegionFrames.values[cardTypeIndex.value];
			regionFrameImg.enabled = selectedRegion.value == 13 && otherRegion.value == 13 ? false : true;
		}
	}
}