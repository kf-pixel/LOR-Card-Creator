
namespace UnityEngine.Events
{
	public class Hook_OnEnable : MonoBehaviour
	{
		[SerializeField] private UnityEvent enableEvent;
		[SerializeField] private UnityEvent disableEvent;

		private void OnEnable()
		{
			enableEvent.Invoke();
		}

		private void OnDisable()
		{
			disableEvent.Invoke();
		}
	}
}