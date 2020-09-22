using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.Events
{
	[System.Serializable]
	public class UnityIntEvent : UnityEvent<int>
	{
	}

	public class IntEventListener : MonoBehaviour
	{
		[Tooltip("Event to register with.")]
		public IntEvent Event;

		[Tooltip("Response to invoke when Event is raised.")]
		public UnityIntEvent IntResponse;

		private void OnEnable()
		{
			Event.RegisterListener(this);
		}

		private void OnDisable()
		{
			Event.UnregisterListener(this);
		}

		public void OnEventRaised(int value)
		{
			IntResponse.Invoke(value);
		}
	}
}
