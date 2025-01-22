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
		Vector3 _lastPosition;

		public void Bind(XRTracker tracker)
		{
			_tracker = tracker;
			_xrStatusText.SetText(_tracker.TrackerMappingName);
		}

		void Update()
		{
			var isMoving = _lastPosition != _tracker.Position;
			_xrStatusImage.color = isMoving ? Color.green : Color.red;
			_lastPosition = _tracker.Position;
		}
	}
}