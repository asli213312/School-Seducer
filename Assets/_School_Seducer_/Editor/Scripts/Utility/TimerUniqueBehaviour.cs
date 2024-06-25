using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class TimerUniqueBehaviour : MonoBehaviour
    {
        [SerializeField] private TextBase timerText;
        
        public Action FinishedEvent; 
        public Action StartedEvent;

        public float CurrentTime { get; private set; }

        private TimeSpan _formatSpan;
        private string _format;
        private string _baseText = "";

        private float _startTime;
        private bool _isRunning;

        private void Start()
        {
            if (CurrentTime == 0)
            {
                Debug.LogError("Timer must have a value greater than 0");
                return;
            }
                
            _isRunning = true;
            StartedEvent?.Invoke();
        }

        private void Update()
        {
            if (_isRunning)
            {
                CurrentTime -= Time.deltaTime / 60.0f;

                if (CurrentTime <= 0)
                {
                    FinishedEvent?.Invoke();
                    _isRunning = false;
                }
            }
            
            UpdateText();
        }

        public void StartTimer()
        {
            _isRunning = true;
            StartedEvent?.Invoke();
        }

        public void InitializeTimeParameters(float currentTime, float startTime)
        {
            CurrentTime = currentTime <= 0 ? startTime : currentTime;
            _startTime = startTime;
        }

        public void InitializeFormat(TimeSpan formatSpan, string format, string baseText = null)
        {
            _formatSpan = formatSpan;
            _format = format;
            _baseText = baseText;
            
            UpdateText();
        }

        public void Skip()
        {
            if (_isRunning)
            {
                CurrentTime = 0;
                UpdateText();
                FinishedEvent?.Invoke();
                _isRunning = false;
            }
        }

        public void Restart()
        {
            _isRunning = true;
            CurrentTime = _startTime;
            
            StartedEvent?.Invoke();
        }

        private void UpdateText()
        {
            timerText.Text = _baseText + FormatTimer(CurrentTime);
        }
        
        private string FormatTimer(float minutes)
        {
            System.TimeSpan time = System.TimeSpan.FromMinutes(minutes);
            return string.Format($"{time.Hours}h {time.Minutes}min");
        }
    }
}