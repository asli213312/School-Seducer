using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class ScrollUtility : MonoBehaviour
    {
        private enum NotEnoughContentBehaviour { Center }

        [Header("Data")]
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private Transform startChecker;
        [SerializeField] private Transform midChecker;
        [SerializeField] private Transform endChecker;

        [Header("Options")]
        [SerializeField] private NotEnoughContentBehaviour isNotEnoughStrategy;
        [SerializeField] private bool isVertical;
        [SerializeField] private float scrollDuration;
        [SerializeField] private float scrollSpeed;
        [SerializeField] private int maxCountToSee;

        [Header("Events")]
        [SerializeField] private UnityEvent isEmptyContentEvent;
        [SerializeField] private UnityEvent canScrollToStartEvent;
        [SerializeField] private UnityEvent canScrollToEndEvent;
        [SerializeField] private UnityEvent isTouchingStartContentEvent;
        [SerializeField] private UnityEvent isTouchingEndContentEvent;
        [SerializeField] private UnityEvent isSmallCountToScrollEvent;
        [SerializeField] private UnityEvent isEnoughCountToScrollEvent;

        private const int CONTENT_STRATEGY_CENTER_OFFSET = 139;

        private int _initialContentLeftPadding;
        private HorizontalLayoutGroup _layout;

        private void Awake() 
        {
            _layout = scrollView.content.GetComponent<HorizontalLayoutGroup>();

            if (_layout != null)
            {
                _initialContentLeftPadding = _layout.padding.left;
            }
            else
            {
                Debug.LogError("HorizontalLayoutGroup component is missing on ScrollRect content.");
            }
        }

        private void OnEnable() 
        {
            CheckMaxCount();
            SelectContentBehaviour();
        }

        public void ScrollToVertical(bool toForward)
        {
            CheckMaxCount();

            if (isVertical)
            {
                StartCoroutine(ScrollVertical(scrollDuration, scrollSpeed, toForward, CheckBounds));
            }
        }

        public void ScrollToHorizontal(bool toForward)
        {
            CheckMaxCount();

            if (!isVertical)
            {
                StartCoroutine(ScrollHorizontal(scrollDuration, scrollSpeed, toForward, CheckBounds));
            }
        }

        public void CheckMaxCount() 
        {
            if (scrollView.content.childCount == 0) 
            {
                isEmptyContentEvent?.Invoke();
                return;
            }

            isEnoughCountToScrollEvent?.Invoke();

            if (scrollView.content.childCount <= maxCountToSee) 
            {
                isSmallCountToScrollEvent?.Invoke();
                return;
            }
        }

        public void SelectContentBehaviour() 
        {   
            switch (isNotEnoughStrategy) 
            {
                case NotEnoughContentBehaviour.Center:
                    _layout.padding.left = (scrollView.content.childCount == 1) 
                        ? CONTENT_STRATEGY_CENTER_OFFSET 
                        : _initialContentLeftPadding;
                break;            
            }
        }

        private void CheckBounds() 
        {
            if (IsTouchingOrInsideChild(startChecker, 0)) 
            {
                isTouchingStartContentEvent?.Invoke();
            }
            else 
            {
                canScrollToStartEvent?.Invoke();
            }

            if (IsTouchingOrInsideChild(endChecker, scrollView.content.childCount - 1))
            {
                isTouchingEndContentEvent?.Invoke();
            }
            else 
            {
                canScrollToEndEvent?.Invoke();
            }
        }

        private bool IsTouchingOrInsideChild(Transform checker, int index)
        {
            if (scrollView.content.childCount > 0)
            {
                Transform child = scrollView.content.GetChild(index);
                return IsTouching(checker, child);
            }
            return false;
        }

        private bool IsTouching(Transform checker, Transform target)
        {
            Collider2D checkerCollider = checker.GetComponent<Collider2D>();
            Collider2D targetCollider = target.GetComponent<Collider2D>();

            if (checkerCollider == null || targetCollider == null)
            {
                return false;
            }

            return checkerCollider.bounds.Intersects(targetCollider.bounds);
        }

        private IEnumerator ScrollVertical(float duration, float scrollSpeed, bool scrollForward, Action onFinished)
        {
            float elapsedTime = 0f;
            float scrollAmount = scrollSpeed * Time.deltaTime;

            while (elapsedTime < duration)
            {
                if (scrollForward)
                    scrollView.horizontalNormalizedPosition += scrollAmount;
                else
                    scrollView.horizontalNormalizedPosition -= scrollAmount;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            onFinished?.Invoke();
        }

        private IEnumerator ScrollHorizontal(float duration, float scrollSpeed, bool scrollForward, Action onFinished)
        {
            float elapsedTime = 0f;
            Vector2 scrollDirection = scrollForward ? Vector2.left : Vector2.right;

            while (elapsedTime < duration)
            {
                scrollView.content.anchoredPosition += scrollDirection * scrollSpeed * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            onFinished?.Invoke();
        }
    }
}