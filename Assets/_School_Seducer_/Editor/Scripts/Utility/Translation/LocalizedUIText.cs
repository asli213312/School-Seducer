using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public class LocalizedUIText : LocalizedUIBase
    {
        [SerializeField] private Text text;
        [SerializeField] private TextMeshProUGUI textPro;
        [SerializeField] private List<Translator.Languages> _localizedData;
        public List<Translator.Languages> LocalizedData
        {
            get
            {
                List<Translator.Languages> languagesList = _localizedData;
                foreach (var item in localizedData)
                {
                    languagesList.Add((Translator.Languages)item);
                }
                return languagesList;
            }
        }
        public string CurrentText { get; private set; }
        public Text Text { get; private set; }

        protected override List<Translator.LanguagesBase> localizedData => _localizedData.Cast<Translator.LanguagesBase>().ToList();

        private void Start()
        {
            UpdateView();
        }

        public override void UpdateView()
        {
            CurrentText = GetCurrentText();
        }

        protected override void OnChangeLanguage()
        {
            Translator.Languages currentLanguage = GetCurrentLanguage() as Translator.Languages;

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