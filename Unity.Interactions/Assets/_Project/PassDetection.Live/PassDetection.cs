using Interactions.Scripts.Infra;
using PassDetection.Replay;
using Tactive.MachineLearning.Models;
using UnityEngine;

namespace _Project.PassDetection.Live
{
	public class PassDetection : MonoBehaviour
	{
		[Header("Dependencies"), SerializeField] XRTracker _dominantFootTracker;
		[SerializeField] XRTracker _nonDominantFootTracker;
		[SerializeField] ModelAssetWithMetadata _lstmModelAsset;
		[SerializeField] AudioClip _passSound;
		[SerializeField] PassDetectionUI _ui;

		public float Prediction => _prediction;
		public int PassCount => _passCount;

		void Start()
		{
			_inputDataQueue = new InputDataQueue();
			_timeSinceLevelStart = Time.time;
			_lstmModel = new LstmModel(_lstmModelAsset);
			_ui.Bind(this);
		}

		void Update()
		{
			var frameRateHz = 10f;
			var deltaTime = 1f / frameRateHz;
			_updateTimer += Time.fixedDeltaTime;
			var epsilon = 0.0001f;

			if (_updateTimer >= deltaTime - epsilon)
			{
				var time = Time.time - _timeSinceLevelStart;
				var dominantFootPosition = _dominantFootTracker.transform.position;
				var nonDominantFootPosition = _nonDominantFootTracker.transform.position;

				_inputDataQueue.EnQueue(dominantFootPosition, nonDominantFootPosition, time);
				_prediction = _lstmModel.Evaluate(_inputDataQueue.ToInputData());

				if (_prediction > 0.95f && Time.time - _lastPassTime >= 1f)
				{
					Debug.Log(_prediction);
					AudioSource.PlayClipAtPoint(_passSound, Camera.main.transform.position);

					_passCount++;
					_lastPassTime = Time.time; // Update the last pass time
				}

				_updateTimer -= deltaTime;
			}
		}

		InputDataQueue _inputDataQueue;
		float _lastPassTime;
		LstmModel _lstmModel;
		int _passCount;
		float _prediction;
		float _timeSinceLevelStart;
		float _updateTimer;
	}
}