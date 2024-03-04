using System;
using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class SliderText : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;

        [SerializeField] private string divider;
        [SerializeField, Tooltip("False = value + divider + maxValue")] private bool onlyValue;
        [SerializeField] private Slider slider;

        public event Action MaxValueEvent; 
        
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            slider.onValueChanged.AddListener(OnValueChanged);
            
            OnValueChanged(0);
        }

        private void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(float value)
        {
            if (slider.value >= slider.maxValue) MaxValueEvent?.Invoke();
            
            if (onlyValue)
                _text.text = slider.value + divider;
            else
                _text.text = slider.value + divider + slider.maxValue;
        }
    }
}