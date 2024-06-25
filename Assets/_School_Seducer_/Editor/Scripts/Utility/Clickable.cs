using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class Clickable : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private UnityEvent onClickedEvent;
        [SerializeField] private bool needParameters;

        [SerializeField, ShowIf(nameof(needParameters))] private float delay;

        public event Action OnClicked;

        private void OnMouseDown()
        {
            Click();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Click();
        }

        private void Click()
        {
            if (delay > 0)
            {
                StartCoroutine(HandleClickWithDelay());
            }
            else
            {
                onClickedEvent?.Invoke();
                OnClicked?.Invoke();
            }
        }

        private IEnumerator HandleClickWithDelay()
        {
            yield return new WaitForSeconds(delay);
            onClickedEvent?.Invoke();
            OnClicked?.Invoke();
        }
    }
}