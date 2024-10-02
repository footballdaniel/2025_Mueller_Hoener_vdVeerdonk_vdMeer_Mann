namespace App
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
	
	public interface IFactory<out T, in T1>
	{
		T Create(T1 args);
	}
	
	public class Factory<T, T1> : IFactory<T, T1>
	{
		public Factory(T component)
		{
			_component = component;
		}

		public T Create(T1 args)
		{
			return _component;
		}

		readonly T _component;
	}
}