using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : MonoBehaviour
{
    [FormerlySerializedAs("ReplayUI")] public UIReplay uiReplay;
    
    public FootballPlayer HomePlayerPrefab;
    public FootballPlayer AwayPlayerPrefab;
    public Ball BallPrefab;
    public GameObject ParticipantIndicatorPrefab;
    
    private List<FootballPlayer> _players;
    List<IReplayable> _replayables;
    private List<GameObject> _gameObjects;
    private int _currentIndex;
    private List<Interaction> _interactions;
    private Vector2 _fieldScale;

    void Start()
    {
        // Repository
        var files = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.json");
        var json = System.IO.File.ReadAllText(files[0]);
        _interactions = JsonConvert.DeserializeObject<List<Interaction>>(json);
        
        // Model
        _fieldScale = new Vector2(105, 68);
        _currentIndex = 0;
        _players = new List<FootballPlayer>();
        _replayables = new List<IReplayable>();
        _gameObjects = new List<GameObject>();
        
        // View
        uiReplay.Next += OnNext;
        uiReplay.Previous += OnPrevious;
    }

    void OnNext()
    {
        _currentIndex++;
        foreach (var gameObj in _gameObjects)
            Destroy(gameObj);
        
        var currentInteraction = _interactions[_currentIndex];
        Spawn(currentInteraction, _fieldScale);
    }
    
    void OnPrevious()
    {
        _currentIndex--;
        foreach (var gameObj in _gameObjects)
            Destroy(gameObj);
        
        var currentInteraction = _interactions[_currentIndex];
        Spawn(currentInteraction, _fieldScale);
    }
    

    private void Spawn(Interaction currentInteraction, Vector2 fieldScale)
    {
        foreach (var team in currentInteraction.Teams)
        {
            var ballTracking = currentInteraction.Tracking.FirstOrDefault(p => p.Id == "ball");
            var ballObject = Instantiate(BallPrefab);
            var ballTrajectoryProvider = new TrajectoryProvider(ballTracking, currentInteraction.Timestamps, fieldScale);
            ballObject.Trajectory = ballTrajectoryProvider;
            ballObject.transform.position = ballTrajectoryProvider.GetPosition(0);
            _replayables.Add(ballObject);
            _gameObjects.Add(ballObject.gameObject);
            
            foreach (var player in team.Players)
            {
                var tracking = currentInteraction.Tracking.FirstOrDefault(p => p.Id == player);
                
                if (tracking == null)
                {
                    Debug.LogWarning($"No tracking data for player {player}");
                    continue;
                }
                
                var playerPrefab = team.IsHome ? HomePlayerPrefab : AwayPlayerPrefab;
                var playerData = currentInteraction.Players.FirstOrDefault(p => p.Id == player);
                var playerObject = Instantiate(playerPrefab);
                _gameObjects.Add(playerObject.gameObject);
                var trajectoryProvider = new TrajectoryProvider(tracking, currentInteraction.Timestamps, fieldScale);
                playerObject.Trajectory = trajectoryProvider;
                playerObject.transform.position = trajectoryProvider.GetPosition(0);
                playerObject.name = playerData.FirstName + " " + playerData.LastName;
                
                if (player == currentInteraction.BallCarrier)
                {
                    var indicator = Instantiate(ParticipantIndicatorPrefab, playerObject.transform, false);
                    _gameObjects.Add(indicator);
                }
                
                _players.Add(playerObject);
                _replayables.Add(playerObject);
            }
            

        }
        
        
        _replayables.ForEach(r => r.Play());
    }
}