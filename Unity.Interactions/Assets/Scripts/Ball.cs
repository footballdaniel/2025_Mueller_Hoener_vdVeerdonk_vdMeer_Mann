using UnityEngine;

public class Ball: MonoBehaviour, IReplayable
{
    public ITrajectory Trajectory { get; set; }
    
    private bool _isPlaying;
    private float _currentReplayTime;
    
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
    
    void Update()
    {
        if (!_isPlaying)
            return;
        
        _currentReplayTime += Time.deltaTime;
        transform.position = Trajectory.GetPosition(_currentReplayTime);
    }

}