using App;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class InSituUI : UIScreen
	{
		[SerializeField] RawImage _cameraFeed;

		void Start()
		{
			_cameraFeed.gameObject.SetActive(false);
		}

		public void Bind(InSituTrialViewModel trialViewModel)
		{
			_cameraFeed.gameObject.SetActive(true);
			_cameraFeed.texture = trialViewModel.Frame;
		}
	}
}