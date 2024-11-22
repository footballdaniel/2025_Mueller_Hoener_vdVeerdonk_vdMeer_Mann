using System.Collections.Generic;
using System.Linq;
using _Project.Interactions.Scripts.Domain;
using UnityEngine;

public class Feature
{
	public string Name { get; }
	public List<float> Values { get; }

	public Feature(string name, List<float> values)
	{
		Name = name;
		Values = values;
	}
}


public class ZeroedPositionDominantFootCalculator
{
	public static int Size => 3;
	
	public static List<Feature> Calculate(Trial trial, float elapsedTime)
	{
		var indices = trial.Timestamps
			.Select((timestamp, index) => new { timestamp, index })
			.Where(x => x.timestamp <= elapsedTime)
			.Select(x => x.index)
			.ToList();

		var lastIndices = indices.Count >= 10
			? indices.Skip(indices.Count - 10).ToList()
			: Enumerable.Repeat(indices.FirstOrDefault(), 10 - indices.Count).Concat(indices).ToList();

		var dominantPositions = lastIndices.Select(i => trial.UserDominantFootPositions[i]).ToList();
		var origin = dominantPositions.First();
		var zeroedPositions = dominantPositions.Select(p => p - origin).ToList();

		var xValues = zeroedPositions.Select(pos => pos.x).ToList();
		var yValues = zeroedPositions.Select(pos => pos.y).ToList();
		var zValues = zeroedPositions.Select(pos => pos.z).ToList();

		return new List<Feature>
		{
			new Feature("zeroed_position_dominant_foot_x", xValues),
			new Feature("zeroed_position_dominant_foot_y", yValues),
			new Feature("zeroed_position_dominant_foot_z", zValues),
		};
	}
}

public class OffsetDominantFootToNonDominantFootCalculator
{
	public static int Size => 3;

	public static List<Feature> Calculate(Trial trial, float elapsedTime)
	{
		var indices = trial.Timestamps
			.Select((timestamp, index) => new { timestamp, index })
			.Where(x => x.timestamp <= elapsedTime)
			.Select(x => x.index)
			.ToList();

		var lastIndices = indices.Count >= 10
			? indices.Skip(indices.Count - 10).ToList()
			: Enumerable.Repeat(indices.FirstOrDefault(), 10 - indices.Count).Concat(indices).ToList();

		var dominantPositions = lastIndices.Select(i => trial.UserDominantFootPositions[i]).ToList();
		var nonDominantPositions = lastIndices.Select(i => trial.UserNonDominantFootPositions[i]).ToList();
		var offsets = dominantPositions.Zip(nonDominantPositions, (dom, nonDom) => nonDom - dom).ToList();

		var xValues = offsets.Select(pos => pos.x).ToList();
		var yValues = offsets.Select(pos => pos.y).ToList();
		var zValues = offsets.Select(pos => pos.z).ToList();

		return new List<Feature>
		{
			new Feature("offset_dominant_foot_to_non_dominant_foot_x", xValues),
			new Feature("offset_dominant_foot_to_non_dominant_foot_y", yValues),
			new Feature("offset_dominant_foot_to_non_dominant_foot_z", zValues),
		};
	}
}

public class VelocitiesDominantFootCalculator
{
	public static int Size => 3;

	public static List<Feature> Calculate(Trial trial, float elapsedTime)
	{
		var indices = trial.Timestamps
			.Select((timestamp, index) => new { timestamp, index })
			.Where(x => x.timestamp <= elapsedTime)
			.Select(x => x.index)
			.ToList();

		var lastIndices = indices.Count >= 10
			? indices.Skip(indices.Count - 10).ToList()
			: Enumerable.Repeat(indices.FirstOrDefault(), 10 - indices.Count).Concat(indices).ToList();

		var dominantPositions = lastIndices.Select(i => trial.UserDominantFootPositions[i]).ToList();
		var timestamps = lastIndices.Select(i => trial.Timestamps[i]).ToList();
		var velocities = new List<Vector3> { Vector3.zero };

		for (int i = 1; i < dominantPositions.Count; i++)
		{
			var dt = timestamps[i] - timestamps[i - 1];
			dt = dt == 0 ? 1e-6f : dt;
			var dx = (dominantPositions[i] - dominantPositions[i - 1]) / dt;
			velocities.Add(dx);
		}

		var xValues = velocities.Select(v => v.x).ToList();
		var yValues = velocities.Select(v => v.y).ToList();
		var zValues = velocities.Select(v => v.z).ToList();

		return new List<Feature>
		{
			new Feature("velocities_dominant_foot_x", xValues),
			new Feature("velocities_dominant_foot_y", yValues),
			new Feature("velocities_dominant_foot_z", zValues),
		};
	}
}

public class VelocitiesNonDominantFootCalculator
{
	public static int Size => 3;

	public static List<Feature> Calculate(Trial trial, float elapsedTime)
	{
		var indices = trial.Timestamps
			.Select((timestamp, index) => new { timestamp, index })
			.Where(x => x.timestamp <= elapsedTime)
			.Select(x => x.index)
			.ToList();

		var lastIndices = indices.Count >= 10
			? indices.Skip(indices.Count - 10).ToList()
			: Enumerable.Repeat(indices.FirstOrDefault(), 10 - indices.Count).Concat(indices).ToList();

		var nonDominantPositions = lastIndices.Select(i => trial.UserNonDominantFootPositions[i]).ToList();
		var timestamps = lastIndices.Select(i => trial.Timestamps[i]).ToList();
		var velocities = new List<Vector3> { Vector3.zero };

		for (int i = 1; i < nonDominantPositions.Count; i++)
		{
			var dt = timestamps[i] - timestamps[i - 1];
			dt = dt == 0 ? 1e-6f : dt;
			var dx = (nonDominantPositions[i] - nonDominantPositions[i - 1]) / dt;
			velocities.Add(dx);
		}

		var xValues = velocities.Select(v => v.x).ToList();
		var yValues = velocities.Select(v => v.y).ToList();
		var zValues = velocities.Select(v => v.z).ToList();

		return new List<Feature>
		{
			new Feature("velocities_non_dominant_foot_x", xValues),
			new Feature("velocities_non_dominant_foot_y", yValues),
			new Feature("velocities_non_dominant_foot_z", zValues),
		};
	}
}
