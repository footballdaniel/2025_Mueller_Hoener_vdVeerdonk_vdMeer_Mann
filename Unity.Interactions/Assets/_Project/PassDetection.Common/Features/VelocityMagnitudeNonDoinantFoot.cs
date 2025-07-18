﻿using System;
using System.Collections.Generic;
using _Project.PassDetection.Common;
using UnityEngine;

public class VelocityMagnitudeNonDominantFoot : Feature<InputData>
{
	public override IReadOnlyList<float> Calculate(InputData inputData)
	{
		var nonDominantPositions = inputData.UserNonDominantFootPositions;
		var timestamps = inputData.Timestamps;
		var values = new List<float>();

		for (var i = 1; i < nonDominantPositions.Count; i++)
		{
			var dt = timestamps[i] - timestamps[i - 1];

			if (dt == 0)
				dt = 1e-6f;

			var dx = (nonDominantPositions[i].x - nonDominantPositions[i - 1].x) / dt;
			var dy = (nonDominantPositions[i].y - nonDominantPositions[i - 1].y) / dt;
			var dz = (nonDominantPositions[i].z - nonDominantPositions[i - 1].z) / dt;

			var magnitude = Mathf.Sqrt(dx * dx + dy * dy + dz * dz);
			values.Add(magnitude);
		}

		values.Insert(0, 0.0f); // Insert a zero magnitude at the start
		return values;
	}
}