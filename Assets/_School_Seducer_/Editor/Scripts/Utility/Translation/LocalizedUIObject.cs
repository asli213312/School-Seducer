using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public class LocalizedUIObject : MonoBehaviour, IObservableCustom<MonoBehaviour>
    {
        [Inject] private LocalizedGlobalMonoBehaviour _localizer;

        [SerializeField] private Text text;
        [SerializeField] private TextMeshProUGUI textPro;
        [SerializeField] private List<Translator.Languages> localizedData;
        public List<Translator.Languages> LocalizedData => localizedData;

        public string CurrentText { get; private set; }
        public Text Text { get; private set; }

        private void Awake()
        {
            if (_localizer != null) _localizer.AddObserver(this);
        }

        private void Start()
        {
            CurrentText = GetCurrentText();
        }

        private void OnDestroy()
        {
            _localizer?.RemoveObserver(this);
        }
        
        public void OnObservableUpdate()
        {
            Translator.Languages currentLanguage = localizedData.Find(x => x.languageCode == _localizer.GlobalLanguageCodeRuntime);

            if (currentLanguage == null) return;    

            if (currentLanguage.key is not null)
            {
                if (text != null) text.text = currentLanguage.key;
                else if (textPro != null) textPro.text = currentLanguage.key;

                CurrentText = currentLanguage.key;
            }
            else
                Debug.LogError("Localized key not found: " + currentLanguage.key, gameObject);
        }

        private string GetCurrentText()
        {
            if (textPro is null)
            {
                Text = text; 
                return text.text;
            }
            
            return textPro.text;  
        } 
    }
}