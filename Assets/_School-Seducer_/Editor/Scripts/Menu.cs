using Telegram.Bot.Types;
using UnityEngine;

namespace _BonGirl_.Editor.Scripts
{
    public class Menu : MonoBehaviour
    {
        private GameObject _currentPanel;
        private Vector3 _startPanelScale;

        public void SafeOpenPanel(GameObject panel)
        {
            if (_currentPanel == null)
            {
                OpenPanel(panel);
                return;
            }

            _currentPanel.transform.localScale = _startPanelScale;
        }

        public void SafeClosePanel(GameObject panel)
        {
            _currentPanel = panel;
            _startPanelScale = _currentPanel.transform.localScale;
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