using Interactions.Scripts.Infra;
using PassDetection.Replay;
using Tactive.MachineLearning.Models;
using UnityEngine;

namespace _Project.PassDetection.Live
{
	public class PassDetectionApp : MonoBehaviour
	{
		[Header("Dependencies"), SerializeField] XRTracker _dominantFootTracker;
		[SerializeField] XRTracker _nonDominantFootTracker;
		[SerializeField] ModelAssetWithMetadata _lstmModelAsset;
		
		
		InputDataQueue _inputDataQueue;
		float _timeSinceLevelStart;
		float _updateTimer;
		LstmModel _lstmModel;

		void Start()
		{
			_inputDataQueue = new InputDataQueue();
			_timeSinceLevelStart = Time.time;
			_lstmModel = new LstmModel(_lstmModelAsset);
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
				var prediction = _lstmModel.Evaluate(_inputDataQueue.ToInputData());
				
				Debug.Log(prediction);

				_updateTimer -= deltaTime;
			}
		}
	}
}