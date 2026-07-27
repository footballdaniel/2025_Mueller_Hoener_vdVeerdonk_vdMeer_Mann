using System;
using System.Collections.Generic;
using Interactions.Domain;
using Interactions.Domain.Tracking;
using UnityEngine;

namespace _Project.Replay.Scripts
{
	/// Builds every frame once at construction over one flat position buffer —
	/// GetFrameAt is an index lookup returning a stored instance, so querying
	/// in the replay loop allocates nothing.
	public class TrialTrackingSource : ITrackingSource
	{
		public const int UserActorId = 0;
		public const int OpponentActorId = 1;

		public TrialTrackingSource(Trial trial, List<FrameEvent> frameEvents)
		{
			_trial = trial;
			_frameEvents = frameEvents;
			BuildFrames();
		}

		public Frame GetFrameAt(TimeSpan time)
		{
			return _frames.Length == 0 ? EmptyFrame : _frames[FrameIndexAt(time)];
		}

		public bool IsBallInPlay(TimeSpan time)
		{
			return _ballInPlay.Length == 0 || _ballInPlay[FrameIndexAt(time)];
		}

		int FrameIndexAt(TimeSpan time)
		{
			return Mathf.Clamp(Mathf.FloorToInt((float)time.TotalSeconds * _trial.FrameRateHz), 0, _frames.Length - 1);
		}

		void BuildFrames()
		{
			var frameCount = _trial.NumberOfFrames;
			_frames = new Frame[frameCount];
			_ballInPlay = new bool[frameCount];
			var positions = new Vector3[frameCount * ActorCount * BoneCount];

			var previousEventIndex = -1;
			for (var i = 0; i < frameCount; i++)
			{
				var userOffset = (i * ActorCount + UserActorId) * BoneCount;
				var opponentOffset = (i * ActorCount + OpponentActorId) * BoneCount;

				positions[userOffset + (int)Bone.Hips] = At(_trial.UserHipPositions, i);
				positions[userOffset + (int)Bone.Head] = At(_trial.UserHeadPositions, i);
				positions[userOffset + (int)DominantFootBone] = At(_trial.UserDominantFootPositions, i);
				positions[userOffset + (int)NonDominantFootBone] = At(_trial.UserNonDominantFootPositions, i);
				positions[opponentOffset + (int)Bone.Hips] = At(_trial.OpponentHipPositions, i);

				while (previousEventIndex + 1 < _frameEvents.Count && _frameEvents[previousEventIndex + 1].FrameIndex <= i)
					previousEventIndex++;

				var actors = new[]
				{
					new ActorState(UserActorId, TeamSide.Home, new BodyPose(positions, offset: userOffset)),
					new ActorState(OpponentActorId, TeamSide.Away, new BodyPose(positions, offset: opponentOffset))
				};

				_frames[i] = new Frame(TimeSpan.FromSeconds(i / (float)_trial.FrameRateHz), actors, BallPositionAt(i, previousEventIndex));
				_ballInPlay[i] = _frameEvents.Count == 0 || _frameEvents[Mathf.Max(previousEventIndex, 0)].EventType != EventType.Pass;
			}
		}

		Vector3 BallPositionAt(int index, int previousEventIndex)
		{
			if (_frameEvents.Count == 0)
				return Vector3.zero;

			var previousEvent = _frameEvents[Mathf.Max(previousEventIndex, 0)];
			var nextEvent = previousEventIndex + 1 < _frameEvents.Count ? _frameEvents[previousEventIndex + 1] : _frameEvents[^1];

			var ballAtPreviousEvent = FootPositionAtEvent(previousEvent);
			var ballAtNextEvent = FootPositionAtEvent(nextEvent);

			var t = previousEvent.FrameIndex == nextEvent.FrameIndex
				? 0f
				: (index - previousEvent.FrameIndex) / (float)(nextEvent.FrameIndex - previousEvent.FrameIndex);

			return Vector3.Lerp(ballAtPreviousEvent, ballAtNextEvent, t);
		}

		Vector3 FootPositionAtEvent(FrameEvent frameEvent)
		{
			return frameEvent.Foot == _trial.DominantFoot
				? At(_trial.UserDominantFootPositions, frameEvent.FrameIndex)
				: At(_trial.UserNonDominantFootPositions, frameEvent.FrameIndex);
		}

		static Vector3 At(List<Vector3> channel, int index)
		{
			return channel != null && index >= 0 && index < channel.Count ? channel[index] : Vector3.zero;
		}

		Bone DominantFootBone => _trial.DominantFoot == Side.LEFT ? Bone.LeftFoot : Bone.RightFoot;
		Bone NonDominantFootBone => _trial.DominantFoot == Side.LEFT ? Bone.RightFoot : Bone.LeftFoot;

		const int ActorCount = 2;
		static readonly int BoneCount = Enum.GetValues(typeof(Bone)).Length;
		static readonly Frame EmptyFrame = new(TimeSpan.Zero, Array.Empty<ActorState>(), Vector3.zero);

		bool[] _ballInPlay;
		Frame[] _frames;
		readonly List<FrameEvent> _frameEvents;
		readonly Trial _trial;
	}
}
