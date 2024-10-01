using System.Collections.Generic;

namespace Domain.VideoRecorder
{
	public interface IRepository<T>
	{
		T Get(int id);
		IEnumerable<T> GetAll();
	}
}