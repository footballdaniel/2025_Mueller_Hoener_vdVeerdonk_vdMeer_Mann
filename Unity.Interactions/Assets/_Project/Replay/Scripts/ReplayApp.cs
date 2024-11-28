using System.Collections.Generic;
using System.IO;
using _Project.Interactions.Scripts.Common;
using _Project.Interactions.Scripts.Domain;
using Interactions.Scripts.Domain;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Video;

namespace _Project.Replay.Scripts
{
	public class ReplayApp : MonoBehaviour
	{
		public ReplayUI ReplayUI;
		public Goal LeftGoal;
		public Goal RightGoal;
		public GameObject Ball;
		public GameObject UserHead;
		public GameObject NonDominantFoot;
		public GameObject DominantFoot;
		public GameObject Opponent;

		public VideoPlayer VideoPlayer;
		public string DataPath = "C:/Users/danie/Desktop/git/2025_Mueller_Hoener_Mann/Data/Pilot_3"; // Forward slashes

		public int NumberOfFrames => _trial.NumberOfFrames;
		public int CurrentFrameIndex => _currentFrameIndex;

		void Start()
		{
			// Use Directory.GetFiles with forward slashes
			var csvFiles = Directory.GetFiles(DataPath, "*.csv");

			var csvFile = csvFiles[0];

			Debug.Log("Found csv file: " + csvFile);

			var jsonFile = Path.ChangeExtension(csvFile, ".json");
			var mp4File = Path.ChangeExtension(csvFile, ".mp4");

			var jsonFileExists = File.Exists(jsonFile);
			var mp4FileExists = File.Exists(mp4File);

			if (!jsonFileExists || !mp4FileExists)
			{
				Debug.LogError("Missing json or mp4 file for: " + csvFile);
				return;
			}

			VideoPlayer.url = mp4File;
			VideoPlayer.Prepare();

			var json = File.ReadAllText(jsonFile);

			var jsonSettings = new JsonSerializerSettings();
			jsonSettings.Converters.Add(new Vector3Converter());
			jsonSettings.Converters.Add(new SideEnumConverter());
			_trial = JsonConvert.DeserializeObject<Trial>(json, jsonSettings);

			_frameEvents = CsvParser.ParseCsv(csvFile);
			_ballController = new BallController(Ball, _frameEvents, _trial);
			_goalController = new GoalController(_frameEvents, LeftGoal, RightGoal);

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

			if (VideoPlayer.isPlaying)
				VideoPlayer.Pause();
			VideoPlayer.Play();


			var t = _currentTimeStamp * _trial.FrameRateHz - _currentFrameIndex;

			_goalController.Tick(_currentFrameIndex);
			_ballController.Tick(_currentFrameIndex, t);
		
			DominantFoot.transform.position = Vector3.Lerp(DominantFoot.transform.position, _trial.UserDominantFootPositions[_currentFrameIndex], t);
			NonDominantFoot.transform.position = Vector3.Lerp(NonDominantFoot.transform.position, _trial.UserNonDominantFootPositions[_currentFrameIndex], t);
			Opponent.transform.position = Vector3.Lerp(Opponent.transform.position, _trial.OpponentHipPositions[_currentFrameIndex], t);
			UserHead.transform.position = Vector3.Lerp(UserHead.transform.position, _trial.UserHeadPositions[_currentFrameIndex], t);
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

		BallController _ballController;
		int _currentFrameIndex;
		float _currentTimeStamp;
		List<FrameEvent> _frameEvents;
		GoalController _goalController;
		bool _isPlaying;

		int _lastFrameIndex = -1;
		Trial _trial;
		bool _videoReady;
	}
}