using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class InteractTransform : MonoBehaviour
    {
        [SerializeField] public UnityEvent eventOnClick;
        public event Action ClickEvent;
        
        private void OnMouseDown()
        {
            ClickEvent?.Invoke();
            eventOnClick?.Invoke();
        }
    }
}