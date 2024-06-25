using GameAnalyticsSDK;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    public class GameAnalyticsManager : MonoBehaviour
    {
        private void Start()
        {
            GameAnalytics.Initialize();
        }

        public void InvokeDesignEvent(string eventName)
        {
            GameAnalytics.NewDesignEvent(eventName);
        }

        public void InvokeDesignEventValue(string eventName, float value)
        {
            GameAnalytics.NewDesignEvent(eventName);
            Debug.Log("GA_EVENT invoked: " + eventName);
        }
    }
}