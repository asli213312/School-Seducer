using UltEvents;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class ResponsibleUntilUpdate : MonoBehaviour
    {
        [SerializeField] private UltEvent objActivatedEvent;
        [SerializeField] private UltEvent objDeactivatedEvent;
        
        private bool _completed;

        private Transform _target;

        private void Update()
        {
            if (_target == null) return;

            if (_completed && _target.gameObject.activeSelf == false) _completed = true;
            else
            {
                objActivatedEvent?.Invoke();
                _target = null;
                _completed = false;
            }

            if (_completed && _target.gameObject.activeSelf) _completed = true;
            else
            {
                objDeactivatedEvent?.Invoke();
                _target = null;
                _completed = false;
            }
        }

        public void InvokeWaitActivated(Transform obj)
        {
            _target = obj;
            _completed = true;
        }

        public void InvokeWaitDeactivated(Transform obj)
        {
            _target = obj;
            _completed = true;
        }
    }
}