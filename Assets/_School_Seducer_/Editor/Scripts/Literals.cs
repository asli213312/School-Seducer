using System;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class Literals : MonoBehaviour
    {
        [Inject] private Bank _bank;
        [Inject] private EventManager _eventManager;

        [SerializeField] private LocalizedUIObject localizedMoney;
        [SerializeField] private LocalizedUIObject localizedDiamonds;
        [SerializeField] private LocalizedUIObject localizedExp;

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
            localizedDiamonds.Text.text = $"{localizedDiamonds.CurrentText} " + _bank.Diamonds;
        }

        private void UpdateMoneyText()
        {
            localizedMoney.Text.text = $"{localizedMoney.CurrentText} " + _bank.Money;
        }
    }
}