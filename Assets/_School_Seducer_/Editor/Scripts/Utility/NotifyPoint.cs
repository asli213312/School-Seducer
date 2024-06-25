using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class NotifyPoint : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private UnityEvent activateEvent;
        [SerializeField] private UnityEvent deactivateEvent;
        public string Id => id;
        private Action _onActivateAction;
        private Action _onDeactivateAction;

        private Notificator _notificator;
        
        private bool _isActivated;

        public void Initialize(Notificator notificator) => _notificator = notificator;

        public void Execute()
        {
            _onActivateAction?.Invoke();
            activateEvent?.Invoke();
        }

        public void Reset()
        {
            _onDeactivateAction?.Invoke();
            deactivateEvent?.Invoke();
        }

        public void SetOnActivateAction(Action onActivateAction) => _onActivateAction = onActivateAction;

        public void SetOnDeactivateAction(Action onDeactivateAction) => _onDeactivateAction = onDeactivateAction;
    }
}