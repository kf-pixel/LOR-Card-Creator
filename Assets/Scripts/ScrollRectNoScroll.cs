using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollRectNoScroll : ScrollRect
{
	public override void OnScroll(PointerEventData eventData) { }

}
