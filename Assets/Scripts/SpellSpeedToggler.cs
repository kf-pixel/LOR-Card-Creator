using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


public class SpellSpeedToggler : MonoBehaviour
{
	[SerializeField] private IntVariable nullSpellSpeed;
	[SerializeField] private GameEvent nullSpeedEvent;
	[SerializeField] private Image nullSpeedImage;
	public UnityEvent onTrue;
	public UnityEvent onFalse;

    private void OnEnable()
    {
		CheckToggle();
	}

	public void SetTrue()
	{
		nullSpellSpeed.NewInt(1);
		nullSpeedImage.enabled = true;
		nullSpeedEvent.Raise();
	}

	public void SetFalse()
	{
		nullSpellSpeed.NewInt(0);
		nullSpeedImage.enabled = false;
		nullSpeedEvent.Raise();
	}

	public void CheckToggle()
	{
		if (nullSpellSpeed.value == 1)
		{
			onTrue.Invoke();
		}
		else
		{
			onFalse.Invoke();
		}
	}
}
