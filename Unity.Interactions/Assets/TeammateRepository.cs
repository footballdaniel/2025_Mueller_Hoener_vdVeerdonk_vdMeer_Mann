using System.Collections.Generic;
using Domain.VideoRecorder;
using UnityEngine;

public class TeammateRepository : MonoBehaviour, IRepository<Teammate>
{
	[SerializeField] List<Teammate> _teammates = new();

	void OnValidate()
	{
		foreach (var teammate in GetComponentsInChildren<Teammate>())
			if (!_teammates.Contains(teammate))
				_teammates.Add(teammate);
	}

	public Teammate Get(int id)
	{
		return _teammates.Find(teammate => teammate.Id == id);
	}

	public IEnumerable<Teammate> GetAll()
	{
		return _teammates;
	}
}