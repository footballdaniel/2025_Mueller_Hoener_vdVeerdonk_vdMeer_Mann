using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions.UI
{
	public class XRTrackerStatus : MonoBehaviour
	{
		[SerializeField] Image _xrStatusImage;
		[SerializeField] TMP_Text _xrStatusText;
		XRTracker _tracker;

		public void Bind(XRTracker tracker)
		{
			_tracker = tracker;
			_xrStatusText.SetText(_tracker.TrackerMappingName);
		}

		void Update()
		{
			_xrStatusImage.color = _tracker.Position.magnitude > 0f ? Color.green : Color.red;
		}
	}
}
