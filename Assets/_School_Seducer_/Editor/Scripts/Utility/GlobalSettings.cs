using System;
using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class GlobalSettings : ScriptableObject, ILocalizedData
    {
        [SerializeField] private Enums.Language currentLanguage;

        [SerializeField] private bool showDebugParameters;
        [SerializeField] private bool tutorialCompleted;
        [SerializeField] public bool soundEnabled;
        [SerializeField, ShowIf(nameof(showDebugParameters))] private LocalizedGlobalMonoBehaviour localizatorRuntime;
        public static string GlobalCurrentLanguage { get; private set; }
        public bool TutorialCompleted { get => tutorialCompleted; set => tutorialCompleted = value; }

        public static event Action LanguageChanged;

        private void OnValidate()
        {
            string languageCode = "";
            
            switch (currentLanguage)
            {
                case Enums.Language.EN: languageCode = "en"; break; 
                case Enums.Language.FR: languageCode = "fr"; break;
                case Enums.Language.RU: languageCode = "ru"; break;
                case Enums.Language.IT: languageCode = "it"; break;
                case Enums.Language.DE: languageCode = "de"; break;
            }
            
            GlobalCurrentLanguage = languageCode;
            localizatorRuntime.GlobalLanguageCodeRuntime = languageCode;
            
            LocalizedGlobalScriptableObject.UpdateLocalizedData();
            
            LanguageChanged?.Invoke();
            
            Debug.Log("Installed language: " + GlobalCurrentLanguage.ToUpper());
        }
    }
}