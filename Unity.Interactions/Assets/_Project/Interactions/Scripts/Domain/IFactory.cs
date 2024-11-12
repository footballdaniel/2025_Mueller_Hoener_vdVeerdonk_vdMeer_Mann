namespace _Project.Interactions.Scripts.Domain
{
	public interface IFactory<T>
	{
		T Create();
	}

	public class Factory<T> : IFactory<T>
	{
		public Factory(T component)
		{
			_component = component;
		}

		public T Create()
		{
			return _component;
		}

		readonly T _component;
	}
}