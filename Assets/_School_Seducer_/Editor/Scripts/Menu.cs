using System;
using System.Collections;
using _School_Seducer_.Editor.Scripts;
using _School_Seducer_.Editor.Scripts.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using UnityEngine;
using UnityEngine.UI;
using UltEvents;
using Zenject;

namespace _BonGirl_.Editor.Scripts
{
    public class Menu : MonoBehaviour
    {
    	[Inject] private Bank _bank;
        [Inject] private LocalizedGlobalMonoBehaviour _localizer;
        private GameObject _currentPanel;
        private GameObject _selectedPanel;
        private Vector3 _startPanelScaled;

        private void Start()
        {
            Time.timeScale = 1;
        }

        public void GiveGold(int amount) 
        {
        	_bank.ChangeValueGold(amount);
        }

        public void GiveDiamonds(int amount) 
        {
        	_bank.ChangeValueDiamonds(amount);
        }

        public void GiveGold(int amount, UltEventHolder onSuccess) 
        {
        	_bank.ChangeValueGold(amount, () => onSuccess?.Invoke());
        }

        public void GiveDiamonds(int amount, UltEventHolder onSuccess) 
        {
        	_bank.ChangeValueDiamonds(amount, () => onSuccess?.Invoke());
        }

        public void SubtractGold(int amount, UltEventHolder onSuccess) 
        {
        	_bank.ChangeValueGold(-amount, () => onSuccess?.Invoke());
        }

        public void SubtractDiamonds(int amount, UltEventHolder onSuccess) 
        {
        	_bank.ChangeValueDiamonds(-amount, () => onSuccess?.Invoke());
        }
        
        public void ResetPause() => Time.timeScale = 1;

        public void SetCurrentPanel(GameObject panel)
        {
            _selectedPanel = panel;

            Debug.Log("Panel selectedd: " + _selectedPanel.name);
        }

        public void PressButton(Button button) => button.onClick?.Invoke();

        public void EnableByChangePositionX(float xPosition)
        {
            if (_selectedPanel == null)
            {
                Debug.LogWarning("Select panel to change position!");
                return;
            }

            _selectedPanel.transform.position = new Vector3(xPosition, _selectedPanel.transform.position.y, _selectedPanel.transform.position.z);
        }

        public void ChangePositionSelectedByPoint(Transform point)
        {
            if (_selectedPanel == null)
            {
                Debug.LogWarning("Select panel to change position!");
                return;
            }
            
            _selectedPanel.transform.position = point.position;
        }

        public void StartScaleSelected(float scaleValue)
        {
            if (_selectedPanel == null)
            {
                Debug.LogWarning("Select panel to upscale!");
                return;
            }

            StartCoroutine(_selectedPanel.transform.DoLocalScaleAndUnscale(this, new Vector3(scaleValue, scaleValue, scaleValue)));
        }

        public void AlphaClosePanel(Graphic graphic)
        {
            Color color = graphic.color;
            graphic.color = new Color(color.r, color.g, color.b, 0);
        }

        public void AlphaOpenPanel(Graphic graphic)
        {
            Color color = graphic.color;
            graphic.color = new Color(color.r, color.g, color.b, 1);
        }

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
