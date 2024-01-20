using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using UnityEngine;
using Zenject;

namespace _BonGirl_.Editor.Scripts
{
    public class Menu : MonoBehaviour
    {
        [Inject] private LocalizedGlobalMonoBehaviour _localizer;
        private GameObject _currentPanel;
        private Vector3 _startPanelScaled;
       

        public void SelectLanguage(string languageCode)
        {
            string selectedLanguageCode = "";
            
            switch (languageCode)
            {
                case "en": selectedLanguageCode = "en"; break;
                case "fr": selectedLanguageCode = "fr"; break;
            }
            
            _localizer.GlobalLanguageCodeRuntime = selectedLanguageCode;
            
            _localizer.Notify();
            Debug.Log("Selected language runtime: " + _localizer.GlobalLanguageCodeRuntime.ToUpper());
        }

        private void Start()
        {
            Time.timeScale = 1;
        }
        
        public void ResetPause() => Time.timeScale = 1;

        public void SafeOpenPanel(GameObject panel)
        {
            if (_currentPanel == null)
            {
                OpenPanel(panel);
                return;
            }

            _currentPanel.transform.localScale = _startPanelScaled;
        }

        public void SafeClosePanel(GameObject panel)
        {
            _currentPanel = panel;
            _startPanelScaled = _currentPanel.transform.localScale;
            panel.transform.localScale = new Vector3(0, 0, 0);
        }

        public void OpenPanel(GameObject panel)
        {
            panel.SetActive(true);
        }

        public void ClosePanel(GameObject panel)
        {
            panel.SetActive(false);
        }
    }
}
