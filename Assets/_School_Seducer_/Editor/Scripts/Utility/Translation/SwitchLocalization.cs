using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Utility.Translation
{
    public class SwitchLocalization : MonoBehaviour
    {
        [Inject] private LocalizedGlobalMonoBehaviour _localizer;

        public void ChangeLanguage(string languageCode)
        {
            string selectedLanguageCode = "";
            
            switch (languageCode)
            {
                case "en": selectedLanguageCode = "en"; break;
                case "fr": selectedLanguageCode = "fr"; break;
            }

            _localizer.GlobalLanguageCodeRuntime = selectedLanguageCode;
            
            _localizer.Notify();
            
            //LocalizedGlobalScriptableObject.UpdateLocalizedData();
            Debug.Log("Selected language: " + _localizer.GlobalLanguageCodeRuntime);
        }
    }
}