using System.Collections.Generic;

namespace Interactions.Domain.VideoRecorder
{
	public interface IRepository<T>
	{
		T Get(int id);
		IEnumerable<T> GetAll();
		void Add(T entity);
	}
}