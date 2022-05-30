using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.Events
{
	[System.Serializable]
	public class UnityStringEvent : UnityEvent<string>
	{
	}

	public class StringEventListener : MonoBehaviour
	{
		[Tooltip("Event to register with.")]
		public StringEvent Event;

		[Tooltip("Response to invoke when Event is raised.")]
		public UnityStringEvent StringResponse;

		private void OnEnable()
		{
			Event.RegisterListener(this);
		}

		private void OnDisable()
		{
			Event.UnregisterListener(this);
		}

		public void OnEventRaised(string value)
		{
			StringResponse.Invoke(value);
		}
	}
}
