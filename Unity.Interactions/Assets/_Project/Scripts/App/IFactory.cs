namespace App
{
	internal interface IFactory<T>
	{
		T Create();
	}
}