using System;
using System.Collections;
using _School_Seducer_.Editor.Scripts.Utility.Components;
using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class ResponsibleWaitUntil : MonoBehaviour
    {
        [SerializeField] private UnityEvent objActivatedEvent;
        [SerializeField] private UnityEvent objDeactivatedEvent;
        [SerializeField] private bool needParameters;
        [SerializeField, ShowIf(nameof(needParameters))] private UnityEvent onClickableEvent;
        [SerializeField, ShowIf(nameof(needParameters))] private UnityEvent delayedDeactivateFailedEvent;
        [SerializeField, ShowIf(nameof(needParameters))] private ObjectMembersHighlighter objectHighlighter;
        [SerializeField, ShowIf(nameof(needParameters))] private bool showPatroller;
        [SerializeField, ShowIf(nameof(ShowedPatroller))] private Patroller patroller;
        [SerializeField, ShowIf(nameof(ShowedPatroller))] private UnityEvent patrollerCounterCompletedEvent;

        private bool ShowedPatroller() => needParameters && showPatroller;

        private float _delay;
        private UltEventHolder _currentEventHandler;

        public void SelectEventHandler(UltEventHolder eventHandler) => _currentEventHandler = eventHandler;

        public void InvokeWaitClicked(Clickable clickable)
        {
            clickable.OnClicked += Handler;

            void Handler()
            {
                onClickableEvent.Invoke();
                clickable.OnClicked -= Handler;
            }
        }

        public void InvokeWaitUntilObjectHighlighterValueChanged(bool value)
        {
            if (objectHighlighter == null)
            {
                return;
            }

            StartCoroutine(WaitUntilObjectHighLighterValueChanged(value));
        }

        public void InvokeWaitPatrollerCounterCompleted(int needCount)
        {
            if (patroller == null) return;
            if (patroller.CurrentCounter == 999)
            {
                Debug.Log("<color=red>RESPONSIBLE WAIT UNTIL: </color>patroller counter is null to invoke!", gameObject);
                return;
            }

            StartCoroutine(WaitUntilPatrollerCounterCompleted(needCount));
        }

        public void InvokeWaitDelayedDeactivated(Transform obj, float delay)
        {
            StartCoroutine(WaitUntilDelayedDeactivated(obj, delay));
        }

        public void InvokeWaitDeactivated(Transform obj)
        {
            StartCoroutine(WaitUntilDeactivated(obj));
        }

        public void InvokeWaitActivated(Transform obj)
        {
            StartCoroutine(WaitUntilActivated(obj));
        }
        
        private IEnumerator WaitUntilObjectHighLighterValueChanged(bool needValue)
        {
            yield return new WaitUntil(() => objectHighlighter.Value == needValue);

            _currentEventHandler?.Invoke();
        }

        private IEnumerator WaitUntilPatrollerCounterCompleted(int needCount)
        {
            yield return new WaitUntil(() => patroller.CurrentCounter == needCount);
            
            if (_currentEventHandler == null)
                patrollerCounterCompletedEvent?.Invoke();
            else
                _currentEventHandler?.Invoke();
        }

        private IEnumerator WaitUntilDelayedDeactivated(Transform obj, float delay)
        {
            _delay = delay;
            
            while (_delay > 0)
            {
                yield return new WaitForSeconds(1);
                _delay -= 1;
                Debug.Log("wait until DELAY: " + _delay, gameObject);
                
                if (obj.gameObject.activeSelf == false)
                {
                    delayedDeactivateFailedEvent?.Invoke();
                    Debug.Log("<color=red>RESPONSIBLE WAIT UNTIL</color>: delayed deactivate failed!", gameObject);
                    yield break;       
                }
            }

            Debug.Log("<color=red>RESPONSIBLE WAIT UNTIL</color>: delayed deactivate completed!", gameObject);
            objDeactivatedEvent?.Invoke();
        }

        private IEnumerator WaitUntilDeactivated(Transform obj)
        {
            yield return new WaitUntil(() => obj.gameObject.activeSelf == false);
            objDeactivatedEvent?.Invoke();
        }

        private IEnumerator WaitUntilActivated(Transform obj)
        {
            yield return new WaitUntil(() => obj.gameObject.activeSelf);
            objActivatedEvent?.Invoke();
        }
    }
}