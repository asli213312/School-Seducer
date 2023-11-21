using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class AutoScroll : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;
        
        [SerializeField] private float scrollSpeed = 5f;

        private Chat _chat;
        private ScrollRect _scrollRect;
        private bool _isAutoScrolling;
        private bool _isCoroutineRunning;

        private void Awake()
        {
            _eventManager.ChatMessagesIsStarted += StartOrStopCoroutine;
        }

        private void OnDestroy()
        {
            _eventManager.ChatMessagesIsStarted -= StartOrStopCoroutine;
        }

        private void OnValidate()
        {
            _chat ??= GetComponent<Chat>();
            _scrollRect ??= GetComponent<ScrollRect>();
        }

        private void StartOrStopCoroutine()
        {
            if (!_chat.IsMessagesEnded && !_isCoroutineRunning)
            {
                StartCoroutine(AutoScrollCoroutine());
            }
            else if (_chat.IsMessagesEnded && _isCoroutineRunning)
            {
                StopCoroutine(AutoScrollCoroutine());
                _isCoroutineRunning = false;
            }
        }

        private IEnumerator AutoScrollCoroutine()
        {
            _isCoroutineRunning = true;
            
            while (_chat.IsMessagesEnded == false)
            {
                yield return new WaitForSeconds(1f); // Ждем 3 секунды перед каждым автоматическим скроллом
                yield return new WaitForSeconds(1.5f);

                if (!_isAutoScrolling)
                {
                    float targetPosition = 0.1f;
                    float duration = scrollSpeed; // Длительность прокрутки в секундах

                    float startTime = Time.time;
                    float startPosition = _scrollRect.verticalNormalizedPosition;

                    while (Time.time < startTime + duration)
                    {
                        float t = (Time.time - startTime) / duration;
                        _scrollRect.verticalNormalizedPosition = Mathf.Lerp(startPosition, targetPosition, t);
                        yield return null;
                    }

                    // Устанавливаем конечную позицию, чтобы избежать небольших расхождений
                    _scrollRect.verticalNormalizedPosition = targetPosition;
                }
            }

            _isCoroutineRunning = false;
        }

        private bool IsScrollbarAtBottom()
        {
            if (_scrollRect.verticalScrollbar)
            {
                // Проверяем, находится ли ползунок в самом низу
                return Mathf.Approximately(_scrollRect.verticalScrollbar.value, 0f);
            }

            // В случае, если вертикального скроллбара нет, считаем, что он всегда внизу
            return true;
        }
    }
}