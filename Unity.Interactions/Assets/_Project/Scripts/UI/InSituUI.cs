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

		public void Bind(InSituViewModel viewModel)
		{
			_cameraFeed.gameObject.SetActive(true);
			_cameraFeed.texture = viewModel.Frame;
		}
	}
}