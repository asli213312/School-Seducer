using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class MainUI : MonoBehaviour
    {
        [Inject] private Bank _bank;
        [Inject] private EventManager _eventManager;
        
        [SerializeField] private Text moneyText;
        [SerializeField] private Text diamondsText;

        private void Awake()
        {
            _eventManager.UpdateTextMoneyEvent += UpdateMoneyText;
            _eventManager.UpdateTextDiamondsEvent += UpdateDiamondsText;
        }

        private void OnDestroy()
        {
            _eventManager.UpdateTextMoneyEvent -= UpdateMoneyText;
            _eventManager.UpdateTextDiamondsEvent -= UpdateDiamondsText;
        }

        private void Start()
        {
            UpdateDiamondsText();
            UpdateMoneyText();
        }

        private void UpdateDiamondsText()
        {
            diamondsText.text = "Diamonds: " + _bank.Diamonds;
        }

        private void UpdateMoneyText()
        {
            moneyText.text = "Money: " + _bank.Money;
        }
    }
}