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
        
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Chat chat;
        
        [SerializeField] private float scrollSpeed = 5f;
        [SerializeField] private float firstTargetValueAnim;

	    //[InfoBox("This delay determine how long need wait to scroll at bottom of scrollView")]
        //[SerializeField] private float delayToBottomScroll = 0.7f;
        
        private bool _isAutoScrolling;
        private bool _isCoroutineRunning;

        private void Start()
	    {
		    if (scrollRect != null)
		    {
			    _eventManager.UpdateScrollEvent += StartScroll;
			    scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
		    }
	    }

	    private void OnDestroy()
	    {
		    if (scrollRect != null)
		    {
			    _eventManager.UpdateScrollEvent -= StartScroll;
			    scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
		    }
	    }

	    private void OnScrollValueChanged(Vector2 value)
	    {
		    if (!_isAutoScrolling && _eventManager.IsChatMessageReceived() && !_isCoroutineRunning && !IsScrollbarAtBottom())
		    {
			    StartScroll();
		    }
	    }

	    private void StartOrStopCoroutine()
	    {
		    if (_isAutoScrolling && _isCoroutineRunning && scrollRect.verticalNormalizedPosition <= 0f)
		    {
			    StopCoroutine(AutoScrollCoroutine());
			    _isCoroutineRunning = false;
			    _isAutoScrolling = false;
		    }
	    }

	    private void StartScroll()
	    {
		    StartCoroutine(AutoScrollCoroutine());
	    }

	    private IEnumerator AutoScrollCoroutine()
	    {
		    _isCoroutineRunning = true;
		    _isAutoScrolling = true;
		    
		    yield return AutoScrollAnimation(firstTargetValueAnim, scrollSpeed);

		    yield return AutoScrollAnimation(0f, scrollSpeed, true);

		    _isAutoScrolling = false;
		    _isCoroutineRunning = false;
	    }

	    private IEnumerator AutoScrollAnimation(float targetValue, float animationSpeed, bool needToBottom = false)
	    {
		    float elapsedTime = 0f;
		    float startValue = scrollRect.verticalNormalizedPosition;

		    while (elapsedTime < animationSpeed)
		    {
			    scrollRect.verticalNormalizedPosition = Mathf.Lerp(startValue, targetValue, elapsedTime / animationSpeed);
			    elapsedTime += Time.deltaTime;
			    
			    if (targetValue >= scrollRect.verticalNormalizedPosition && needToBottom == false)
			    {
				    yield break;
			    }
			    
			    yield return null;
		    }
		    
		    if (needToBottom)
			    scrollRect.verticalNormalizedPosition = targetValue;
	    }
	    
	    private void DisableAutoScrolling() => _isAutoScrolling = false;

	    private bool IsScrollbarAtBottom()
	    {
		    if (scrollRect.verticalScrollbar)
		    {
			    return Mathf.Approximately(scrollRect.verticalScrollbar.value, 0f);
		    }

		    return true;
	    }
    }
}