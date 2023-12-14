using System;
using System.Collections;
using _School_Seducer_.Editor.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class AutoScroll : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;
        
        [SerializeField] private float scrollSpeed = 5f;
        [SerializeField] private float firstTargetValueAnim;

	    //[InfoBox("This delay determine how long need wait to scroll at bottom of scrollView")]
        [SerializeField] private float delayToBottomScroll = 0.7f;

        private Chat _chat;
        private ScrollRect _scrollRect;
        private bool _isAutoScrolling;
        private bool _isCoroutineRunning;

        private void OnValidate() 
	    {
	    	_scrollRect ??= GetComponent<ScrollRect>();
	    	_chat ??= GetComponent<Chat>();
	    }

	    private void Start()
	    {
		    if (_scrollRect != null)
		    {
			    _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
		    }
	    }

	    private void OnDestroy()
	    {
		    if (_scrollRect != null)
		    {
			    _scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
		    }
		    
		    //_eventManager.ChatMessagesIsStarted -= DisableAutoScrolling;
	    }

	    private void OnScrollValueChanged(Vector2 value)
	    {
		    if (!_isAutoScrolling && !_chat.IsMessagesEnded && !_isCoroutineRunning && !IsScrollbarAtBottom())
		    {
			    StartCoroutine(AutoScrollCoroutine());
		    }
	    }

	    private void StartOrStopCoroutine()
	    {
		    if (_isAutoScrolling && _isCoroutineRunning && _scrollRect.verticalNormalizedPosition <= 0f)
		    {
			    StopCoroutine(AutoScrollCoroutine());
			    _isCoroutineRunning = false;
			    _isAutoScrolling = false; // Дополнительно обнуляем флаг автопрокрутки
		    }
	    }

	    private IEnumerator AutoScrollCoroutine()
	    {
		    _isCoroutineRunning = true;
		    _isAutoScrolling = true;
		    
		    yield return AutoScrollAnimation(firstTargetValueAnim, scrollSpeed);
		    
		    Debug.Log("IsVeryBigMessage in AUTOSCROLL: " + _chat.IsVeryBigMessage);
		    Debug.Log("IsBigMessage in AUTOSCROLL: " + _chat.IsBigMessage);
		    
		    //if (_chat.IsVeryBigMessage)
				//yield return new WaitUntil(InputExtensions.CheckTap);
		    
		    //yield return new WaitForSeconds(delayToBottomScroll);

		    yield return AutoScrollAnimation(0f, scrollSpeed, true);

		    _isAutoScrolling = false;
		    _isCoroutineRunning = false;
	    }

	    private IEnumerator AutoScrollAnimation(float targetValue, float animationSpeed, bool needToBottom = false)
	    {
		    float elapsedTime = 0f;
		    float startValue = _scrollRect.verticalNormalizedPosition;

		    while (elapsedTime < animationSpeed)
		    {
			    _scrollRect.verticalNormalizedPosition = Mathf.Lerp(startValue, targetValue, elapsedTime / animationSpeed);
			    elapsedTime += Time.deltaTime;
			    
			    if (targetValue >= _scrollRect.verticalNormalizedPosition && needToBottom == false)
			    {
				    yield break;
			    }
			    
			    yield return null;
		    }
		    
		    if (needToBottom)
				_scrollRect.verticalNormalizedPosition = targetValue;
	    }
	    
	    private void DisableAutoScrolling() => _isAutoScrolling = false;

	    private bool IsScrollbarAtBottom()
	    {
		    if (_scrollRect.verticalScrollbar)
		    {
			    return Mathf.Approximately(_scrollRect.verticalScrollbar.value, 0f);
		    }

		    return true;
	    }
    }
}