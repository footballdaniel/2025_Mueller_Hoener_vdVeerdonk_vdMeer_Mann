using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Domain;
using UnityEngine;

public class BallController
{
	public BallController(GameObject ball, List<FrameEvent> frameEvents, Trial trial)
	{
		_ball = ball;
		_frameEvents = frameEvents;
		_trial = trial;
		_dominantSide = _trial.DominantFoot;
	}

	public void Tick(int frameIndex)
	{
		var previousEvent = _frameEvents.LastOrDefault(e => e.FrameIndex <= frameIndex) ?? _frameEvents.First();
		var nextEvent = _frameEvents.FirstOrDefault(e => e.FrameIndex > frameIndex) ?? _frameEvents.Last();

		var ballAtPreviousEvent = GetBallPositionAtEvent(previousEvent);
		var ballAtNextEvent = GetBallPositionAtEvent(nextEvent);

		var t = previousEvent.FrameIndex == nextEvent.FrameIndex
			? 0 // Avoid division by zero when the events are the same
			: (frameIndex - previousEvent.FrameIndex) / (float)(nextEvent.FrameIndex - previousEvent.FrameIndex);

		_ball.transform.position = Vector3.Lerp(ballAtPreviousEvent, ballAtNextEvent, t);
	}

	Vector3 GetBallPositionAtEvent(FrameEvent frameEvent)
	{
		return frameEvent.Foot == Side.RIGHT
			? _dominantSide == Side.RIGHT ? _trial.UserDominantFootPositions[frameEvent.FrameIndex] : _trial.UserNonDominantFootPositions[frameEvent.FrameIndex]
			: _dominantSide == Side.RIGHT
				? _trial.UserNonDominantFootPositions[frameEvent.FrameIndex]
				: _trial.UserDominantFootPositions[frameEvent.FrameIndex];
	}

	readonly GameObject _ball;
	readonly Side _dominantSide;
	readonly List<FrameEvent> _frameEvents;
	readonly Trial _trial;
}