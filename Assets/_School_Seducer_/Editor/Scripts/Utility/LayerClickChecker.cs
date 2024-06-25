using System;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class ClickChecker : MonoBehaviour
    {
        [SerializeField] private RectTransform clickableObj;
        [SerializeField] private UnityEvent OnClick;
        
        public void ResetListeners() => OnClick.RemoveAllListeners();
        public void ChangeClickableObj(RectTransform clickable) => clickableObj = clickable;

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && RectTransformUtility.RectangleContainsScreenPoint(clickableObj, Input.mousePosition, Camera.main))
            {
                 OnClick?.Invoke();
            }
        }
    }
}