using System.Linq;
using App;
using Domain;
using UnityEngine;

public class OpponentFactoryMonobehaviour : MonoBehaviour, IFactory<IOpponent>
{
	[SerializeField] Opponent _opponentPrefab;
	
	[SerializeField] User _user;
	[SerializeField] TeammateRepository _teammateRepository;
	
	public IOpponent Create()
	{
		var teammate = _teammateRepository.GetAll();
		var go = Instantiate(_opponentPrefab);
		go.Set(_user, teammate.ToList());
		return go;
	}
}
