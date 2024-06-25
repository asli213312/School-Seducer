using System.Runtime.CompilerServices;
using UltEvents;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class UltEventUtility : MonoBehaviour
    {
        public void CancelExecutionDelayed(DelayedUltEventHolder ultEvent)
        {
            if (ultEvent.Event.HasCalls) ultEvent.Event.Clear();
        }
    }
}