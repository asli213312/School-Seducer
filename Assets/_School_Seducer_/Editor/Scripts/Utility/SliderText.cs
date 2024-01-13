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
        
        [SerializeField] private Slider slider;

        public Slider Slider => slider;

        public event Action MaxValueEvent; 
        
        private TextMeshProUGUI _text;

        private void Awake()
        {
            _eventManager.UpdateExperienceTextEvent += OnValueChanged;

            OnValueChanged();
        }
        
        private void OnValidate() 
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void OnDestroy()
        {
            _eventManager.UpdateExperienceTextEvent -= OnValueChanged;
        }

        private void OnValueChanged()
        {
            if (slider.value >= slider.maxValue) MaxValueEvent?.Invoke();
            
            _text.text = slider.value + "/" + slider.maxValue;
        }
    }
}