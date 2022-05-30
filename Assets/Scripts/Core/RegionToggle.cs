using UnityEngine;
using UnityEngine.UI;

public class RegionToggle : BasePointerClick
{
	public int regionID;
	[SerializeField] private IntVariable region1;
	[SerializeField] private IntVariable region2;
	[SerializeField] private GameEvent refreshEvent;
	private Toggle toggle;

	[Header("For Custom User Regions")]
	[SerializeField] private Image checkmarkImg;

	protected override void Awake()
	{
		base.Awake();
		TryGetComponent<Toggle>(out toggle);
	}

	protected override void OnEnable()
    {
		base.OnEnable();
		Parse();
	}

	protected override void OnClick()
	{
		if (shift || ctrl) return;
		Activate(false);
	}

	protected override void OnAltClick()
	{
		Activate(true);
	}

	public void Parse()
	{
		if (regionID == region1.value)
		{
			toggle.isOn = true;
		}
        else if (regionID == region2.value && regionID != 13)
		{
			toggle.isOn = true;
		}
		else
		{
			toggle.isOn = false;
		}
	}

	public void Activate(bool asDual = false)
	{
		if (asDual == false)
		{
			region1.NewInt(regionID);
			region2.NewInt(13);
		}
		else // dual region input
		{
            if (regionID == region1.value) return;
			region2.NewInt(regionID);
		}
		refreshEvent.Raise();

		toggle.isOn = true;

		if (regionID == 13 && asDual)
		{
			toggle.isOn = false;
		}
	}

	public void UpdateSprites(Sprite newSprite) // load in custom regions
	{
		checkmarkImg.sprite = newSprite;
	}
}
