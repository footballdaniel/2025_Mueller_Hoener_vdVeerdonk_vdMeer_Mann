using System.Collections.Generic;
using System.IO;
using System.Linq;
using DefaultNamespace;

public class FrameEvent
{
	public int FrameIndex { get; set; }
	public Side Foot { get; set; }
	public EventType EventType { get; set; }
	public bool Success { get; set; }
	public GoalDirection GoalDirection { get; set; }
}

public enum EventType
{
	Touch,
	Pass
}

public enum GoalDirection
{
	None,
	Left,
	Right
}

public static class CsvParser
{
	public static List<FrameEvent> ParseCsv(string filePath)
	{
		return File.ReadAllLines(filePath)
			.Select(line => line.Split(','))
			.Select(parts => new FrameEvent
			{
				FrameIndex = int.Parse(parts[0]),
				Foot = ParseFoot(parts[1]),
				EventType = parts[1].Contains("P") ? EventType.Pass : EventType.Touch,
				Success = ParseSuccess(parts[1]),
				GoalDirection = ParseGoalDirection(parts[1])
			})
			.ToList();
	}

	static Side ParseFoot(string part)
	{
		// if contains p, parse 1st position
		if (part.Contains("P"))
			return part[1] == 'L' ? Side.LEFT : Side.RIGHT;

		// parse 0 position
		return part[0] == 'L' ? Side.LEFT : Side.RIGHT;
	}

	static GoalDirection ParseGoalDirection(string part)
	{
		if (!part.Contains("P"))
			return GoalDirection.None;

		return part[3] == 'L' ? GoalDirection.Left : GoalDirection.Right;
	}

	static bool ParseSuccess(string part)
	{
		// if it contains y return true, otherwise false
		if (part.Contains("Y"))
			return true;
		return false;
	}
}