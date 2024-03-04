using System;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using TMPro;
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

        [SerializeField] private TextMeshProUGUI gold;
        [SerializeField] private LocalizedUIText localizedDiamonds;
        [SerializeField] private LocalizedUIText localizedExp;

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
            gold.text = _bank.Money.ToString();
        }
    }
}