using System;
using UnityEngine;

namespace Interactions.Domain
{
	[Serializable]
	public class BallEvent
	{
		public string Name { get; }
		public float Timestamp { get; }
		public Vector3 Position { get; }
		
		public BallEvent(string name, float timestamp, Vector3 position)
		{
			Name = name;
			Timestamp = timestamp;
			Position = position;
		}
	}
}