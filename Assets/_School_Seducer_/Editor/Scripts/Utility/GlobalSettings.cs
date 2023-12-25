using System;
using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class GlobalSettings : ScriptableObject, ILocalizedData
    {
        [SerializeField] private Enums.Language currentLanguage;

        [SerializeField] private bool showDebugParameters;
        [SerializeField, ShowIf(nameof(showDebugParameters))] private LocalizedGlobalMonoBehaviour localizatorRuntime;
        public static string GlobalCurrentLanguage { get; private set; }
        
        public static event Action LanguageChanged;

        private void OnValidate()
        {
            string languageCode = "";
            
            switch (this.currentLanguage)
            {
                case Enums.Language.EN:
                    languageCode = "en";
                    break;
                case Enums.Language.FR:
                    languageCode = "fr";
                    break;
            }
            
            GlobalCurrentLanguage = languageCode;
            localizatorRuntime.GlobalLanguageCodeRuntime = languageCode;
            
            LocalizedGlobalScriptableObject.UpdateLocalizedData();
            
            LanguageChanged?.Invoke();
            
            Debug.Log("Installed language: " + GlobalCurrentLanguage.ToUpper());
        }
    }
}