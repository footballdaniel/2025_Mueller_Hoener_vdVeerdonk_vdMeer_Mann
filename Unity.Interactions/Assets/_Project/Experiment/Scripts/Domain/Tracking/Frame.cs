using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Domain.Tracking
{
	public sealed record Frame(TimeSpan Time, IReadOnlyList<ActorState> Actors, Vector3 BallPosition);
}
