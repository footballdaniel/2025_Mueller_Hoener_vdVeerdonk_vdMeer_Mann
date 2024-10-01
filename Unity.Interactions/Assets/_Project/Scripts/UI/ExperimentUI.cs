using System;
using App;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class ExperimentUI : MonoBehaviour
	{
		[SerializeField] TMP_Text _fpsText;
		[SerializeField] Button _nextTrialButton;

		void Update()
		{
			_fpsText.text = $"FPS: {Math.Round(1 / Time.deltaTime)} /n FPS Fixed: {Math.Round(1 / Time.fixedDeltaTime)}";
		}

		public void Set(ExperimentPresenter presenter)
		{
			_nextTrialButton.onClick.AddListener(presenter.NextTrial);
		}
	}
}