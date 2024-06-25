using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class Notificator : MonoBehaviour
    {
        [SerializeField] private List<NotifyPoint> notifyPoints = new();
        [SerializeField] private UnityEvent onNotifyEvent;

        public Action OnNotify;
        public bool ConditionToNotify { get; set; }
        private bool _isCompleted;

        private void Awake() => notifyPoints.ForEach(x => x.Initialize(this));

        private void Update()
        {
            if (_isCompleted && ConditionToNotify)
            {
                Reset();
                
                Debug.Log("Notified: " + gameObject.name, gameObject);
                
                ConditionToNotify = false;
                _isCompleted = false;
            }
        }

        public void Notify()
        {
            notifyPoints.ForEach(x => x.Execute());
            onNotifyEvent?.Invoke();
            OnNotify?.Invoke();
            _isCompleted = true;
        }

        public void Reset() => notifyPoints.ForEach(x => x.Reset());

        public NotifyPoint GetNotifyPointByIndex(int index)
        {
            if (index > notifyPoints.Count)
            {
                Debug.LogError("Notify point index is out of range.", gameObject);
                return null;
            }
            
            return notifyPoints[index];
        }

        public NotifyPoint GetNotifyPointById(string id)
        {
            NotifyPoint foundPoint = notifyPoints.FirstOrDefault(x => x.Id == id);
            
            if (foundPoint == null)
            {
                Debug.LogError("Notify point is null by id: " + id, gameObject);
                return null;
            }
            
            return foundPoint;
        }
    }
}