using System;

namespace Interactions.Application.ViewModels
{
	public class Observable<T>
	{
		public event Action<T> ValueChanged;
		T _value;

		public Observable(T value)
		{
			_value = value;
		}

		public T Value
		{
			get => _value;
			set
			{
				if (!Equals(_value, value))
				{
					_value = value;
					ValueChanged?.Invoke(_value);
				}
			}
		}
	}
}