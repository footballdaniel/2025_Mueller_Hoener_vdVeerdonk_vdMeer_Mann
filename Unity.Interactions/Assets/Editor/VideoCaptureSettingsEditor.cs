// using System.Linq;
// using Infra;
// using UnityEditor;
//
// [CustomEditor(typeof(WebcamRecorder))]
// public class VideoCaptureRecorderEditor : UnityEditor.Editor
// {
// 	void OnEnable()
// 	{
// 		_webcamRecorder = (WebcamRecorder)target;
// 		UpdateResolutionOptions();
// 		UpdateFrameRateOptions();
// 	}
//
// 	public override void OnInspectorGUI()
// 	{
// 		serializedObject.Update();
//
// 		// Resolution dropdown
// 		EditorGUILayout.LabelField("Select Resolution", EditorStyles.boldLabel);
// 		_selectedResolutionIndex = EditorGUILayout.Popup("Resolution", _selectedResolutionIndex, _resolutionOptions);
//
// 		// Update selected resolution in MonoBehaviour
// 		if (_selectedResolutionIndex >= 0 && _selectedResolutionIndex < _webcamRecorder.AvailableResolutions.Count)
// 		{
// 			_webcamRecorder.SelectedResolution = _webcamRecorder.AvailableResolutions[_selectedResolutionIndex];
// 			UpdateFrameRateOptions(); // Update frame rate options based on selected resolution
// 		}
//
// 		// Frame rate dropdown (dependent on the selected resolution)
// 		EditorGUILayout.LabelField("Select Frame Rate", EditorStyles.boldLabel);
// 		_selectedFrameRateIndex = EditorGUILayout.Popup("Frame Rate", _selectedFrameRateIndex, _frameRateOptions);
//
// 		// Update selected frame rate in MonoBehaviour
// 		if (_selectedFrameRateIndex >= 0 && _frameRateOptions != null && _frameRateOptions.Length > 0)
// 			_webcamRecorder.FrameRate = float.Parse(_frameRateOptions[_selectedFrameRateIndex]);
//
// 		serializedObject.ApplyModifiedProperties();
// 	}
//
// 	// Helper function to update the frame rate options dropdown based on the selected resolution
// 	void UpdateFrameRateOptions()
// 	{
// 		var availableFrameRates = _webcamRecorder.GetAvailableFrameratesFor(_webcamRecorder.SelectedResolution);
// 		_frameRateOptions = availableFrameRates.Select(fps => fps.ToString()).ToArray();
//
// 		// Set the selected frame rate index based on the current selected frame rate
// 		_selectedFrameRateIndex = availableFrameRates.IndexOf(_webcamRecorder.FrameRate);
// 		if (_selectedFrameRateIndex < 0) _selectedFrameRateIndex = 0;
// 	}
//
// 	// Helper function to update the resolution options dropdown
// 	void UpdateResolutionOptions()
// 	{
// 		var availableResolutions = _webcamRecorder.AvailableResolutions;
// 		_resolutionOptions = availableResolutions.Select(res => res.width + "x" + res.height).ToArray();
//
// 		// Set the selected resolution index based on the current selected resolution
// 		_selectedResolutionIndex = availableResolutions.IndexOf(_webcamRecorder.SelectedResolution);
// 		if (_selectedResolutionIndex < 0) _selectedResolutionIndex = 0;
// 	}
//
// 	string[] _frameRateOptions;
// 	string[] _resolutionOptions;
// 	int _selectedFrameRateIndex;
// 	int _selectedResolutionIndex;
// 	WebcamRecorder _webcamRecorder;
// }