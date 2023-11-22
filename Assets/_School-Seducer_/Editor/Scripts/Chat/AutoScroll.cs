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
	    	//_eventManager.ChatMessagesIsEnded += DisableAutoScrolling;
	    }
        
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
		    else if (_isAutoScrolling && _isCoroutineRunning && _chat.IsMessagesEnded) 
		    {
		    	StopCoroutine(AutoScrollCoroutine());
		    	_isCoroutineRunning = false;
		    }
	    }
	    
	    private void StartOrStopCoroutine()
	    {
		    //if (!_chat.IsMessagesEnded && !_isCoroutineRunning && !IsScrollbarAtBottom())
		    //{
			//    StartCoroutine(AutoScrollCoroutine());
		    //}
		    //else if (_chat.IsMessagesEnded && _isCoroutineRunning)
		    //{
			//    StopCoroutine(AutoScrollCoroutine());
			//    _isCoroutineRunning = false;
		    //}
	    }

	    private IEnumerator AutoScrollCoroutine()
	    {
	    	_isCoroutineRunning = true;
		    _isAutoScrolling = true;
		    
		    if (_isAutoScrolling) 
		    {
		    	float elapsedTime = 0f;
			    float startValue = _scrollRect.verticalNormalizedPosition;
			    float targetValue = 0.1f;

			    while (elapsedTime < scrollSpeed)
			    {
				    _scrollRect.verticalNormalizedPosition = Mathf.Lerp(startValue, targetValue, elapsedTime / scrollSpeed);
				    elapsedTime += Time.deltaTime;
				    yield return null;
			    }

			    _scrollRect.verticalNormalizedPosition = targetValue;
		    }
	    	
		    
		    _isAutoScrolling = false;
		    _isCoroutineRunning = false;
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