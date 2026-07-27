namespace Interactions.Domain.Tracking
{
	public readonly struct ActorState
	{
		public ActorState(int actorId, TeamSide team, BodyPose pose)
		{
			ActorId = actorId;
			Team = team;
			Pose = pose;
		}

		public int ActorId { get; }
		public TeamSide Team { get; }
		public BodyPose Pose { get; }
	}
}
