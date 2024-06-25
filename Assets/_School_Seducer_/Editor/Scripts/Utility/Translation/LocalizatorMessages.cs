using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NaughtyAttributes;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
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

        public void RemoveRestrictedCharsRussian() 
        {
            if (translationJson == null) 
            {
                Debug.LogError("Translation JSON is not assigned for remove restricted chars!");
            }

            string json = translationJson.text;
            Debug.Log("Original JSON content: " + json);

            // Десериализация JSON
            Translator translator = JsonUtility.FromJson<Translator>(json);

            // Проверка, что данные десериализованы правильно
            if (translator?.languages == null)
            {
                Debug.LogError("Failed to deserialize translation JSON.");
                return;
            }

            // Поиск и замена символа 'ё' на 'е' в ключах русского языка
            foreach (var language in translator.languages)
            {
                if (language.languageCode == "ru")
                {
                    foreach (var message in language.messages)
                    {
                        message.key = message.key.Replace('ё', 'е');
                    }
                }
            }

            // Сериализация обратно в JSON
            string updatedJson = JsonUtility.ToJson(translator, true);
            Debug.Log("Updated JSON content: " + updatedJson);

            //string path = AssetDatabase.GetAssetPath(translationJson);
            //File.WriteAllText(path, updatedJson);

            // Обновление файла в редакторе
            //AssetDatabase.Refresh();

            Debug.Log("JSON file has been updated and saved.");    
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

        public List<Translator.LanguagesText> GetLanguages(int indexMessage)
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

            List<Translator.LanguagesText> translatedMessages = new List<Translator.LanguagesText>();

            foreach (var language in _translator.languages)
            {
                if (indexMessage >= 0 && indexMessage < language.messages.Count)
                {
                    Translator.LanguagesText translatedMessage = new Translator.LanguagesText();
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