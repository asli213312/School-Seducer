using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NaughtyAttributes;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    [Serializable]
    public class LocalizatorMessages : BaseLocalizator
    {
        [SerializeField, HideInInspector] private Translator _translator;

        public void ResetTranslations()
        {
            if (_translator.languages.Count > 0)
            {
                _translator.languages.Clear();
                Debug.Log("Reset translations is successfully!");
            }
        }

        public string GetTranslatedMessage(string languageCode, int indexMessage)
        {
            if (_translator.languages.Count == 0)
            {
                InitializeTranslation();
            }

            Translator.LanguagesMessage currentLanguage = _translator.languages.Find(x => x.languageCode == languageCode);

            if (currentLanguage != null && indexMessage >= 0 && indexMessage < currentLanguage.messages.Count)
            {
                return currentLanguage.messages[indexMessage].key;
            }
            else
            {
                Debug.LogWarning($"Invalid index '{indexMessage}' for language '{languageCode}'.");
                return "not translated";
            }
        }

        public List<Translator.Languages> GetLanguages(int indexMessage)
        {
            if (_translator.languages == null)
            {
                Debug.LogError("Languages is not initialized!");
                return null;
            }
            
            if (_translator.languages.Count == 0)
            {
                InitializeTranslation();
            }

            List<Translator.Languages> translatedMessages = new List<Translator.Languages>();

            foreach (var language in _translator.languages)
            {
                if (indexMessage >= 0 && indexMessage < language.messages.Count)
                {
                    Translator.Languages translatedMessage = new Translator.Languages();
                    translatedMessage.languageCode = language.languageCode;
                    translatedMessage.key = language.messages[indexMessage].key;

                    translatedMessages.Add(translatedMessage);
                }
                else
                {
                    Debug.LogWarning($"Invalid index '{indexMessage}' for language '{language.languageCode}'.");
                }
            }

            return translatedMessages;
        }

        protected override void InitializeTranslation()
        {
            if (translationJson != null)
            {
                string json = translationJson.text;
                Debug.Log("Original JSON content: " + json);
                
                byte[] utf8Bytes = Encoding.UTF8.GetBytes(json);
                
                TextAsset newTranslationJson = new TextAsset(Encoding.UTF8.GetString(utf8Bytes));
                
                translationJson = newTranslationJson;
                
                _translator = JsonUtility.FromJson<Translator>(translationJson.text);

                Debug.Log("JSON file installed");
            }
            else
            {
                Debug.LogError("Translation JSON is not assigned!");
            }

            Debug.Log("Initialize translation is completed");
            //LogTranslatorDetails();
        }
        
        private void LogTranslatorDetails()
        {
            if (_translator != null)
            {
                Debug.Log($"Translator Languages Count: {_translator.languages.Count}");

                foreach (var language in _translator.languages)
                {
                    Debug.Log($"Language Code: {language.languageCode}, Messages Count: {language.messages.Count}");

                    foreach (var message in language.messages)
                    {
                        Debug.Log($"Message Key: {message.key}");
                    }
                }
            }
            else
            {
                Debug.Log("Translator is null.");
            }
        }
    }
}