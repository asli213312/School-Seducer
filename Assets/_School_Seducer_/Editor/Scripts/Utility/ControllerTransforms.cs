using System;
using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class ControllerTransforms : MonoBehaviour
    {
        [SerializeField] private UnityEvent onClickTransform;
        [SerializeField] private List<InteractTransform> transforms = new();

        private void Awake()
        {
            transforms.ForEach(x => x.ClickEvent += DeactivateTransforms);
            transforms.ForEach(x => x.eventOnClick.AddListener(InvokeEvents));
        }

        private void OnDestroy()
        {
            transforms.ForEach(x => x.ClickEvent -= DeactivateTransforms);
            transforms.ForEach(x => x.eventOnClick.RemoveListener(InvokeEvents));
        }

        public void ActivateTransforms()
        {
            transforms.ForEach(x => x.gameObject.Activate());
        }
        public void DeactivateTransforms() => transforms.ForEach(x => x.gameObject.Deactivate());
        private void InvokeEvents() => transforms.ForEach(x => x.eventOnClick.AddListener(onClickTransform.Invoke));
    }
}