using _Project.Interactions.Scripts.Domain;
using Interactions.Scripts.Domain;
using UnityEngine;

namespace Interactions.Scripts.Infra
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