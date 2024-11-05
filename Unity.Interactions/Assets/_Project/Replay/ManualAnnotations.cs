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
				Foot = parts[1][0] == 'R' ? Side.RIGHT : Side.LEFT,
				EventType = parts[1].Contains("P") ? EventType.Pass : EventType.Touch,
				Success = !parts[1].Contains("N"),
				GoalDirection = parts[1].Contains("L") ? GoalDirection.Left : GoalDirection.None
			})
			.ToList();
	}
}