using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts;
using _School_Seducer_.Editor.Scripts.Mini_Games;
using UnityEngine;
using UnityEngine.UI;
using PuzzleGame.UI;
using Zenject;

public class MiniGameInitializer : MonoBehaviour
{
    [Inject] private EventManager _eventManager;
    [Inject] private Bank _bank;
    
    [SerializeField] private ScoreCounter scoreCounter;
    [SerializeField] private Button closeGameButton; 
    [SerializeField] private MiniGamesConfig data;
    [SerializeField] private List<GameObject> miniGames;

    public GameObject CurrentMiniGame { get; private set; }
    public MiniGamesConfig Data => data;
    public List<GameObject> MiniGames => miniGames;
    
    [field: SerializeField] public bool MiniGameAvailable { get; set; }

    private PlayerConfig _playerConfig;

    private void Awake()
    {
        _playerConfig = _eventManager.PlayerConfig;
    }

    public void TryActivateGame(GameObject currentGame)
    {
        if (currentGame == null)
        {
            Debug.LogWarning("Current mini game is null!");
            return;
        }

        if (MiniGameAvailable == false)
        {
            currentGame.Deactivate();
        }
        else
        {
            currentGame.Activate();
            closeGameButton.gameObject.Activate();
        }
    }

    public void ResetGameAvailability()
    {
        MiniGameAvailable = false;
        _bank.ChangeValueMoney(scoreCounter.Value);
        scoreCounter.ResetScore();
    }

    public void StartMiniGame()
    {
        
    }

    private void SpawnRandomMiniGame()
    {
        //int randomMiniGame = RandomIndex();
        //GameObject newMiniGame = Instantiate()
    }

    private int RandomIndex() => UnityEngine.Random.Range(0, MiniGames.Count);
}
