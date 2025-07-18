using System.Collections.Generic;
using System.IO;
using _Project.PassDetection.Common;
using _Project.PassDetectionReplay;
using Interactions.Common;
using Interactions.Domain;
using Newtonsoft.Json;
using Tactive.MachineLearning.Models;
using UnityEngine;
using UnityEngine.Video;

namespace PassDetection.Replay
{
	public class ReplayApp : MonoBehaviour
	{
		public ModelAssetWithMetadata ModelAsset;
		public ReplayUI ReplayUI;
		public VideoPlayer VideoPlayer;

		public int NumberOfFrames => _trial.NumberOfFrames;
		public int CurrentFrameIndex => _currentFrameIndex;

		public float ModelPrediction => _lastPrediction;

		public List<string> AvailableRecordings => _availableRecordings;

		void Start()
		{
			var dataPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Data/Pilot_4"));

			var jsonFiles = Directory.GetFiles(dataPath, "*.json");
			var jsonFile = jsonFiles[1];

			Debug.Log("Found json file: " + jsonFile);
			var mp4File = Path.ChangeExtension(jsonFile, ".mp4");

			var mp4FileExists = File.Exists(mp4File);

			if (!mp4FileExists)
			{
				Debug.LogError("Missing json or mp4 file for: " + jsonFile);
				return;
			}

			VideoPlayer.url = mp4File;
			VideoPlayer.Prepare();

			var json = File.ReadAllText(jsonFile);
			var jsonSettings = new JsonSerializerSettings();
			jsonSettings.Converters.Add(new Vector3Converter());
			jsonSettings.Converters.Add(new SideEnumConverter());
			_trial = JsonConvert.DeserializeObject<Trial>(json, jsonSettings);

			_lstmModel = new LstmModel(ModelAsset);

			ReplayUI.Set(this);
		}

		void Update()
		{
			if (_isPlaying)
			{
				_currentTimeStamp += Time.deltaTime;
				var recordingFrameRate = _trial.FrameRateHz;
				_currentFrameIndex = Mathf.FloorToInt(_currentTimeStamp * recordingFrameRate);
			}

			_currentFrameIndex = Mathf.Clamp(_currentFrameIndex, 0, _trial.NumberOfFrames - 1);
			VideoPlayer.frame = _currentFrameIndex;

			var toTake = 10;

			var next10Timestamps = _trial.Timestamps.GetRange(_currentFrameIndex, toTake);
			var next10Positions = _trial.UserDominantFootPositions.GetRange(_currentFrameIndex, toTake);
			var next10NonDominantPositions = _trial.UserNonDominantFootPositions.GetRange(_currentFrameIndex, toTake);

			var inputData = new InputData(next10Positions, next10NonDominantPositions, next10Timestamps);

			if (VideoPlayer.isPlaying)
				VideoPlayer.Pause();
			VideoPlayer.Play();

			_lastPrediction = _lstmModel.Evaluate(inputData);
		}


		public void ShowFrame(int value)
		{
			_currentFrameIndex = value;
			var frameRate = _trial.FrameRateHz;
			_currentTimeStamp = _currentFrameIndex / (float)frameRate;
		}

		public void TogglePlayPause()
		{
			_isPlaying = !_isPlaying;
		}

		void OnDestroy()
		{
			_lstmModel.Dispose();
		}

		int _currentFrameIndex;
		float _currentTimeStamp;
		bool _isPlaying;
		float _lastPrediction;
		LstmModel _lstmModel;
		Trial _trial;
		List<string> _availableRecordings;
	}
}