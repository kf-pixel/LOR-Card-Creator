using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ControlSpellSpeed : MonoBehaviour
{
	[SerializeField] private IntVariable nullSpellSpeed;
	[SerializeField] private GameEvent nullSpeedEvent;
	[SerializeField] private Image spellSpeedCover;
	public UnityEvent onTrue;
	public UnityEvent onFalse;

    private void OnEnable()
    {
		Refresh();
	}

	public void Toggle(bool toggleOn)
	{
		if (toggleOn)
		{
			On();
		}
		else
		{
			Off();
		}
	}

	public void On()
	{
		nullSpellSpeed.NewInt(1);
		spellSpeedCover.enabled = true;
		nullSpeedEvent.Raise();
	}

	public void Off()
	{
		nullSpellSpeed.NewInt(0);
		spellSpeedCover.enabled = false;
		nullSpeedEvent.Raise();
	}

	public void Refresh()
	{
		if (nullSpellSpeed.value == 1)
		{
			onTrue.Invoke();
			spellSpeedCover.enabled = true;
		}
		else
		{
			onFalse.Invoke();
			spellSpeedCover.enabled = false;
		}
	}
}
