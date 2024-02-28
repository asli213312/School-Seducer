using PuzzleGame.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGame.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ScoreCounter : MonoBehaviour
    {
        TextMeshProUGUI label;

        protected GameState currentGameState;

        public virtual int Value => currentGameState.Score;
        
        public void ResetScore() => currentGameState.Score = 0;

        void Start()
        {
            label = GetComponent<TextMeshProUGUI>();

            OnProgressUpdate();
            UserProgress.Current.ProgressUpdate += OnProgressUpdate;
        }

        void OnDestroy()
        {
            UserProgress.Current.ProgressUpdate -= OnProgressUpdate;

            if (currentGameState != null)
                currentGameState.StateUpdate -= OnStateUpdate;
        }

        void OnProgressUpdate()
        {
            var gameState = UserProgress.Current.GetGameState<GameState>(UserProgress.Current.CurrentGameId);

            if (currentGameState != null)
                currentGameState.StateUpdate -= OnStateUpdate;

            currentGameState = gameState;

            if (gameState == null)
                return;

            OnStateUpdate();
            gameState.StateUpdate += OnStateUpdate;
        }

        void OnStateUpdate()
        {
            label.text = "+" + Value;
        }
    }
}