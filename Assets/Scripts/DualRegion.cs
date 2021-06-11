using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DualRegion : MonoBehaviour
{
	public BoolVariable shiftInput;
	public BoolVariable dualRegionSetting;
	public GameObject originalRegionGroup, dualRegionGroup;

	private void ActivateDualRegion(bool active)
	{
		originalRegionGroup.SetActive(!active);
		dualRegionGroup.SetActive(active);
	}
	private void Update()
	{
		if (dualRegionSetting.value)
		{
			ActivateDualRegion(shiftInput.value);
		}
	}
}
