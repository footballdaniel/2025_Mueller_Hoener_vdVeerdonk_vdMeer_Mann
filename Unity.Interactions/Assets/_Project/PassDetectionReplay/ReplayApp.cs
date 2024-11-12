using System;
using System.IO;
using System.Linq;
using _Project.Interactions.Scripts.Common;
using _Project.Interactions.Scripts.Domain;
using _Project.PassDetectionReplay;
using Newtonsoft.Json;
using PassDetection;
using UnityEngine;
using UnityEngine.Video;
using Vector3 = System.Numerics.Vector3;

namespace PassDetectionReplay
{
    public class ReplayApp : MonoBehaviour
    {
        public LSTM_Model LSTM_Model;
        public ReplayUI ReplayUI;
        public VideoPlayer VideoPlayer;
        public string DataPath = "C:/Users/danie/Desktop/git/2025_Mueller_Hoener_Mann/Data/Pilot_4"; // Forward slashes
        Trial _trial;
        bool _isPlaying;
        float _currentTimeStamp;
        int _currentFrameIndex;
        float _lastPrediction;

        public int NumberOfFrames => _trial.NumberOfFrames;
        public int CurrentFrameIndex => _currentFrameIndex;

        public float ModelPrediction => _lastPrediction;

        void Start()
        {
                // Use Directory.GetFiles with forward slashes
                var jsonFiles = Directory.GetFiles(DataPath, "*.json");
                var jsonFile = jsonFiles[0];

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
                
                LSTM_Model.Bind(_trial);

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
            
            _lastPrediction = LSTM_Model.EvaluateTrialAtTime(_currentTimeStamp);
            
            
            
            // DominantFoot.transform.position = Vector3.Lerp(DominantFoot.transform.position, _trial.UserDominantFootPositions[_currentFrameIndex], t);
            // NonDominantFoot.transform.position = Vector3.Lerp(NonDominantFoot.transform.position, _trial.UserNonDominantFootPositions[_currentFrameIndex], t);
            // Opponent.transform.position = Vector3.Lerp(Opponent.transform.position, _trial.OpponentHipPositions[_currentFrameIndex], t);
            // UserHead.transform.position = Vector3.Lerp(UserHead.transform.position, _trial.UserHeadPositions[_currentFrameIndex], t);
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
    }
}
