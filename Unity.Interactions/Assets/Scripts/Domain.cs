using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Player
{
	public string Id { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string TeamId { get; set; }
}

public class Team
{
	public string Id { get; set; }
	public string TeamName { get; set; }
	public bool IsHome { get; set; }
	public List<string> Players { get; set; }
}

public class Trajectory
{
	public string Id { get; set; }
	public List<float> X { get; set; }
	public List<float> Y { get; set; }
}

public class Point
{
	public float X { get; set; }
	public float Y { get; set; }
}

public class Pass
{
	public string PlayerId { get; set; }
	public string ReceiverId { get; set; }
	public bool Success { get; set; }
	public bool HighBall { get; set; }
	public bool OpenPlay { get; set; }
	public int StartTime { get; set; }
	public int EndTime { get; set; }
	public Point Origin { get; set; }
	public Point Destination { get; set; }
}

public class MatchInformation
{
	public float PitchLength { get; set; }
	public float PitchWidth { get; set; }
	public int MatchDay { get; set; }
	public DateTime Date { get; set; }
	public int ScoreHome { get; set; }
	public int ScoreAway { get; set; }
	public int StartFirstHalf { get; set; }
	public int EndFirstHalf { get; set; }
	public int StartSecondHalf { get; set; }
	public int EndSecondHalf { get; set; }
}

public class Match
{
	public string Id { get; set; }
	public MatchInformation Metadata { get; set; }
	public List<Team> Teams { get; set; }
	public List<Player> Players { get; set; }
	public Trajectory Ball { get; set; }
	public List<Trajectory> Tracking { get; set; }
	public List<int> Timestamps { get; set; }
	public List<Pass> Events { get; set; }
}

public class Situation
{
	public string BallCarrier { get; set; }
	public List<Team> Teams { get; set; }
	public List<Player> Players { get; set; }
	public List<Trajectory> Tracking { get; set; }
	public List<int> Timestamps { get; set; }
}

public class Interaction
{
	[JsonProperty("ball_carrier")]
	public string BallCarrier { get; set; }
	public List<Team> Teams { get; set; }
	public List<Player> Players { get; set; }
	public List<Trajectory> Tracking { get; set; }
	public List<int> Timestamps { get; set; }
	[JsonProperty("direct_opponent")]
	public string DirectOpponent { get; set; }
	[JsonProperty("direct_opponent_weights")]
	public Dictionary<string, float> DirectOpponentWeights { get; set; }
	[JsonProperty("relative_trajectory")]
	public Trajectory RelativeTrajectory { get; set; }
	[JsonProperty("ball_carrier_velocity")]
	public Trajectory BallCarrierVelocity { get; set; }
	[JsonProperty("direct_opponent_velocity")]
	public Trajectory DirectOpponentVelocity { get; set; }
}
