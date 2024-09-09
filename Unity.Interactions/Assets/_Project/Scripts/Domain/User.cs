using UnityEngine;

public interface IUser
{
	Vector2 Position { get; }
}


public class User : MonoBehaviour, IUser
{
	public Vector2 Position => new(transform.position.x, transform.position.z);
}