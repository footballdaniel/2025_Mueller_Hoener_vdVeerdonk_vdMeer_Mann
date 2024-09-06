using System.Linq;
using UnityEditor;

[CustomEditor(typeof(VideoCaptureRecorder))]
public class VideoCaptureRecorderEditor : Editor
{
    private VideoCaptureRecorder _videoCaptureRecorder;
    private int _selectedResolutionIndex;
    private int _selectedFrameRateIndex;
    private string[] _resolutionOptions;
    private string[] _frameRateOptions;

    private void OnEnable()
    {
        _videoCaptureRecorder = (VideoCaptureRecorder)target;
        UpdateResolutionOptions();
        UpdateFrameRateOptions();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Resolution dropdown
        EditorGUILayout.LabelField("Select Resolution", EditorStyles.boldLabel);
        _selectedResolutionIndex = EditorGUILayout.Popup("Resolution", _selectedResolutionIndex, _resolutionOptions);

        // Update selected resolution in MonoBehaviour
        if (_selectedResolutionIndex >= 0 && _selectedResolutionIndex < _videoCaptureRecorder.AvailableResolutions.Count)
        {
            _videoCaptureRecorder.SelectedResolution = _videoCaptureRecorder.AvailableResolutions[_selectedResolutionIndex];
            UpdateFrameRateOptions(); // Update frame rate options based on selected resolution
        }

        // Frame rate dropdown (dependent on the selected resolution)
        EditorGUILayout.LabelField("Select Frame Rate", EditorStyles.boldLabel);
        _selectedFrameRateIndex = EditorGUILayout.Popup("Frame Rate", _selectedFrameRateIndex, _frameRateOptions);

        // Update selected frame rate in MonoBehaviour
        if (_selectedFrameRateIndex >= 0 && _frameRateOptions != null && _frameRateOptions.Length > 0)
        {
            _videoCaptureRecorder.FrameRate = float.Parse(_frameRateOptions[_selectedFrameRateIndex]);
        }

        serializedObject.ApplyModifiedProperties();
    }

    // Helper function to update the resolution options dropdown
    private void UpdateResolutionOptions()
    {
        var availableResolutions = _videoCaptureRecorder.AvailableResolutions;
        _resolutionOptions = availableResolutions.Select(res => res.width + "x" + res.height).ToArray();
        
        // Set the selected resolution index based on the current selected resolution
        _selectedResolutionIndex = availableResolutions.IndexOf(_videoCaptureRecorder.SelectedResolution);
        if (_selectedResolutionIndex < 0) _selectedResolutionIndex = 0;
    }

    // Helper function to update the frame rate options dropdown based on the selected resolution
    private void UpdateFrameRateOptions()
    {
        var availableFrameRates = _videoCaptureRecorder.GetAvailableFrameratesFor(_videoCaptureRecorder.SelectedResolution);
        _frameRateOptions = availableFrameRates.Select(fps => fps.ToString()).ToArray();
        
        // Set the selected frame rate index based on the current selected frame rate
        _selectedFrameRateIndex = availableFrameRates.IndexOf(_videoCaptureRecorder.FrameRate);
        if (_selectedFrameRateIndex < 0) _selectedFrameRateIndex = 0;
    }
}
