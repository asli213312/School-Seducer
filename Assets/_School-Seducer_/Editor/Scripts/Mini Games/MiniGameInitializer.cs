using System;
using System.Collections;
using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts;
using UnityEngine;
using Zenject;
using Random = System.Random;

public class MiniGameInitializer : MonoBehaviour
{
    [Inject] private EventManager _eventManager;
    [SerializeField] private List<GameObject> miniGames;
    
    private PlayerConfig _playerConfig;
    public List<GameObject> MiniGames => miniGames;

    private void Awake()
    {
        _playerConfig = _eventManager.PlayerConfig;
    }
    
    public void StartMiniGame()
    {
        StartCoroutine(SpawnRandomMiniGame());
    }

    private IEnumerator SpawnRandomMiniGame()
    {
        bool miniGameResult = false;

        GameObject ballInHolePrefab = miniGames[0];
        
        Vector3 offsetPositionPrefab = new Vector2(0, -2);
        GameObject ballInHoleInstance = Instantiate(ballInHolePrefab, Vector3.down + offsetPositionPrefab, Quaternion.identity);
        Debug.Log("this mini game: " + ballInHoleInstance.gameObject.name, ballInHoleInstance.gameObject);
        
        IMiniGame ballInHole = ballInHoleInstance.GetComponent<BallInHole>();
        ballInHole.OnMiniGameFinished += result => miniGameResult = result;
        
        while (!miniGameResult)
        {
            yield return null;
            if (ballInHole.IsGameFinished)
                break;
        }

        if (miniGameResult)
        {
            Debug.Log("Mini game good result is: " + miniGameResult);
            _eventManager.ChangeValueMoney(_playerConfig.AddMoneyAtClick);
            _eventManager.UpdateTextMoney();
        }
                
        Destroy(ballInHoleInstance);
    }

    private int RandomIndex() => UnityEngine.Random.Range(0, MiniGames.Count);
}
