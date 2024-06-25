using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public class SwitchLocalization : MonoBehaviour
    {
        [Inject] private LocalizedGlobalMonoBehaviour _localizer;

        [SerializeField] private string languageCode;
        [SerializeField] private RectTransform checker;
        [SerializeField] private RectTransform checkPos;
        [SerializeField] private UnityEvent onSelect;

        public event Action LanguageChangedEvent;

        private void Start()
        {
            //SetChecker();
        }

        public void ChangeLanguage()
        {
            _localizer.GlobalLanguageCodeRuntime = languageCode;

            _localizer.Notify();

            SetChecker();
            
            onSelect?.Invoke();
            LanguageChangedEvent?.Invoke();
            
            //LocalizedGlobalScriptableObject.UpdateLocalizedData();
            Debug.Log("Selected language: " + _localizer.GlobalLanguageCodeRuntime);
        }

        private void SetChecker()
        {
            if (_localizer.GlobalLanguageCodeRuntime == languageCode)
                checker.position = checkPos.position;
        }
    }
}