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
        [SerializeField] private TextOptions mainTextOptions;
        [SerializeField] private List<Translator.LanguagesText> _localizedData;
        public List<Translator.LanguagesText> LocalizedData
        {
            get
            {
                List<Translator.LanguagesText> languagesList = _localizedData;
                foreach (var item in localizedData)
                {
                    languagesList.Add((Translator.LanguagesText)item);
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
            Translator.LanguagesText currentLanguageText = GetCurrentLanguage() as Translator.LanguagesText;

            if (currentLanguageText == null) return;

            if (currentLanguageText.key is not null)
            {
                if (text != null) text.text = currentLanguageText.key;
                else if (textPro != null) textPro.text = currentLanguageText.key;

                if (currentLanguageText.options.font != null)
                    currentLanguageText.options?.Install(textPro);
                else
                    mainTextOptions.Install(textPro);

                CurrentText = currentLanguageText.key;
                UpdateView();
            }
            else
                Debug.LogError("Localized key not found: " + currentLanguageText.key, gameObject);
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