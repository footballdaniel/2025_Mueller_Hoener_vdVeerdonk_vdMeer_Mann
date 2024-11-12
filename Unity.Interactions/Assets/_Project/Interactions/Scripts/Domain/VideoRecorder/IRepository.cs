using System.Collections.Generic;

namespace _Project.Interactions.Scripts.Domain.VideoRecorder
{
	public interface IRepository<T>
	{
		T Get(int id);
		IEnumerable<T> GetAll();
	}
}