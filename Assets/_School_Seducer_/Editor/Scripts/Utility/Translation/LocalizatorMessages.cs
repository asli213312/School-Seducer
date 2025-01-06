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

#if UNITY_EDITOR

        public void LocalizeAudioMessages(Chat.СonversationData conversationData)
        {
            if (conversationData == null)
            {
                Debug.LogWarning("ConversationData is null!");
                return;
            }

            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(conversationData);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogError("Could not find asset path for ConversationData.");
                return;
            }

            string directoryPath = System.IO.Path.GetDirectoryName(assetPath);
            if (directoryPath == null)
            {
                Debug.LogError("Could not determine the directory of the ConversationData.");
                return;
            }

            string[] audioDirectories = System.IO.Directory.GetDirectories(directoryPath, "Audio*");
            if (audioDirectories.Length == 0)
            {
                Debug.LogWarning($"No folders starting with 'Audio' found in {directoryPath}.");
                return;
            }

            string audioFolderPath = audioDirectories[0];
            string[] languageFolders = { "EN", "RU", "DE", "SP", "IT", "FR" };

            Dictionary<string, int> fileIndices = new Dictionary<string, int>();

            foreach (var languageFolder in languageFolders)
            {
                string languagePath = System.IO.Path.Combine(audioFolderPath, languageFolder);
                if (System.IO.Directory.Exists(languagePath))
                {
                    fileIndices[languageFolder] = 0;
                }
            }

            for (int i = 0; i < conversationData.Messages.Length; i++)
            {
                var message = conversationData.Messages[i];
                if (message.LocalizedAudioClips == null)
                {
                    message.LocalizedAudioClips = new List<Translator.LanguageAudioClip>();
                }

                foreach (var languageFolder in languageFolders)
                {
                    if (!fileIndices.ContainsKey(languageFolder)) continue;

                    string languagePath = System.IO.Path.Combine(audioFolderPath, languageFolder);
                    string[] audioFiles = System.IO.Directory.GetFiles(languagePath, "*.mp3");

                    int fileIndex = fileIndices[languageFolder];
                    if (fileIndex < audioFiles.Length)
                    {
                        string audioFilePath = audioFiles[fileIndex];
                        AudioClip audioClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(
                            UnityEditor.AssetDatabase.GUIDToAssetPath(UnityEditor.AssetDatabase.FindAssets(System.IO.Path.GetFileNameWithoutExtension(audioFilePath))[0])
                        );

                        if (audioClip != null)
                        {
                            message.LocalizedAudioClips.Add(new Translator.LanguageAudioClip
                            {
                                languageCode = languageFolder.ToLower(),
                                key = audioClip
                            });
                        }

                        fileIndices[languageFolder]++;
                    }
                }
            }

            Debug.Log("Localization of audio messages completed!");
        }

#endif


        public void RemoveRestrictedCharsRussian() 
        {
            if (translationJson == null) 
            {
                Debug.LogError("Translation JSON is not assigned for remove restricted chars!");
            }

            string json = translationJson.text;
            Debug.Log("Original JSON content: " + json);

            Translator translator = JsonUtility.FromJson<Translator>(json);

            if (translator?.languages == null)
            {
                Debug.LogError("Failed to deserialize translation JSON.");
                return;
            }

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

            string updatedJson = JsonUtility.ToJson(translator, true);
            Debug.Log("Updated JSON content: " + updatedJson);

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