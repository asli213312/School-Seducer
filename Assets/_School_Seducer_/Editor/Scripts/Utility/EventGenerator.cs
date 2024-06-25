using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    [System.Serializable]
    public class EventData
    {
        public string eventId;
        public UnityEvent unityEvent = new UnityEvent();
    }
    
    public class EventGenerator : MonoBehaviour
    {
        [SerializeField]
        public List<EventData> eventsData = new List<EventData>();
        
        public void CreateEvent(string eventId)
        {
            EventData newEventData = new EventData();
            newEventData.eventId = eventId;
            eventsData.Add(newEventData);
        }
        
        public void InvokeEventById(string eventId)
        {
            EventData eventData = eventsData.Find(data => data.eventId == eventId);
            if (eventData != null)
            {
                eventData.unityEvent?.Invoke();
            }
            else
            {
                Debug.LogError("Event with id " + eventId + " not found.");
            }
        }
    }
}