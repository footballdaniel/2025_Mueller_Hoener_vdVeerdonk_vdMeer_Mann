using Interactions.Domain;
using UnityEngine;

namespace Interactions.Infra
{
	public class OpponentFactoryMonobehaviour : MonoBehaviour, IFactory<IOpponent>
	{
		[SerializeField] Opponent _opponentPrefab;

		[SerializeField] User _user;

		public IOpponent Create()
		{
			var go = Instantiate(_opponentPrefab);
			go.Set(_user);
			return go;
		}
	}
}