using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class TimerBehaviour : MonoBehaviour
    {
        [Inject] private MiniGameInitializer _controllerMiniGames;

        [SerializeField] private Button skipButton;
        [SerializeField] private Text timerText;

        private float _currentTime;
        private bool _isRunning;

        private void Awake()
        {
            skipButton.AddListener(Skip);
        }

        private void Start()
        {
            _currentTime = _controllerMiniGames.Data.secondsToOpenGame;
            _isRunning = true;
        }

        private void OnDestroy()
        {
            skipButton.RemoveListener(Skip);
        }

        private void Update()
        {
            if (_isRunning)
            {
                _currentTime -= Time.deltaTime;

                if (_currentTime <= 0)
                {
                    SetAvailableGame();
                    _isRunning = false;
                }
            
                UpdateText();    
            }
        }

        public void ShowText()
        {
            if (_controllerMiniGames.MiniGameAvailable) return;
            
            timerText.gameObject.Activate();
            timerText.gameObject.Deactivate(3);
            
            skipButton.gameObject.Activate();
            skipButton.gameObject.Deactivate(3);
        }

        public void Skip()
        {
            if (_isRunning)
            {
                _currentTime = 0;
                UpdateText();
                SetAvailableGame();
                _isRunning = false;
            }
        }

        public void Restart()
        {
            _isRunning = true;
            _currentTime = _controllerMiniGames.Data.secondsToOpenGame;
            _controllerMiniGames.ResetGameAvailability();
        }

        private void SetAvailableGame()
        {
            _controllerMiniGames.MiniGameAvailable = true;
        }

        private void UpdateText()
        {
            timerText.text = "Need to wait: " + FormatTimer(_currentTime);
        }
        
        private string FormatTimer(float seconds)
        {
            System.TimeSpan time = System.TimeSpan.FromSeconds(seconds);
            return string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
        }
    }
}