using System.Collections.Generic;
using UnityEngine;

public class FootballPlayer : MonoBehaviour, IReplayable
{
    public ITrajectory Trajectory { get; set; }
    
    private float _currentReplayTime;
    private bool _isPlaying;
    
    
    void Update()
    {
        if (!_isPlaying)
            return;
        
        _currentReplayTime += Time.deltaTime;
        
        transform.position = Trajectory.GetPosition(_currentReplayTime);
        
    }

    public void Play()
    {
        _isPlaying = true;
    }

    public void Pause()
    {
        _isPlaying = false;
    }

    public void JumpTo(float time)
    {
        _currentReplayTime = time;
    }
}

public interface ITrajectory
{
    Vector3 GetPosition(float time);
}

public interface IReplayable
{
    void Play();
    void Pause();
    void JumpTo(float time);
}

public class TrajectoryProvider : ITrajectory
{
    private readonly Trajectory _trajectory;
    private readonly int _deltaTimeMilliseconds;
    private readonly Vector2 _fieldScale;
    private bool _isPlaying;
    private readonly List<int> _timestamps;

    public TrajectoryProvider(Trajectory trajectory, List<int> timestamps, Vector2 fieldFieldScale)
    {
        _timestamps = timestamps;
        _trajectory = trajectory;
        _fieldScale = fieldFieldScale;
        _deltaTimeMilliseconds = timestamps[1] - timestamps[0];
    }

    public Vector3 GetPosition(float time)
    {
        var timeToMilliseconds = time * 1000;
        
        if (timeToMilliseconds <= 0)
            return new Vector3(_trajectory.X[0] * _fieldScale.x, 0, _trajectory.Y[0] * _fieldScale.y);

        var previousIndex = (int)timeToMilliseconds / _deltaTimeMilliseconds;
        var nextIndex = previousIndex + 1;
        
        if (nextIndex >= _timestamps.Count)
            return new Vector3(_trajectory.X[_trajectory.X.Count - 1] * _fieldScale.x, 0, _trajectory.Y[_trajectory.Y.Count - 1] * _fieldScale.y);
        
        var percentElapsed = (timeToMilliseconds - previousIndex * _deltaTimeMilliseconds) / _deltaTimeMilliseconds;

        var previous = new Vector3(_trajectory.X[previousIndex], 0, _trajectory.Y[previousIndex]);
        var next = new Vector3(_trajectory.X[nextIndex], 0, _trajectory.Y[nextIndex]);
        
        var interpolated = Vector3.Lerp(previous, next, percentElapsed);
        
        var scaled = new Vector3(interpolated.x * _fieldScale.x, 0, interpolated.z * _fieldScale.y);
        
        return scaled;
    }

  
}

