using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class Bank : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;
        [SerializeField] private PlayerConfig playerConfig;
        
        public int money;
        public int _diamonds;

        public PlayerConfig Data => playerConfig;

        public int Money
        {
            get => money;
            set
            {
                playerConfig.Money = value;
                money = playerConfig.Money;
                _eventManager.UpdateTextMoney();
                _eventManager.MoneyWereChanged();
            }
        }
        
        public int Diamonds
        {
            get => _diamonds;
            set
            {
                playerConfig.Diamonds = value;
                _diamonds = playerConfig.Diamonds;
                _eventManager.UpdateTextDiamonds();
            }
        }

        private void Awake()
        {
            Money = playerConfig.Money;
            Diamonds = playerConfig.Diamonds;
        }

        public void ChangeValueGold(int value, Action onComplete = null)
        {
            if (Money + value >= 0)
            {
                Money += value;
                playerConfig.Money = Money;

                onComplete?.Invoke();
            }
            else
                Debug.LogWarning("Not enough money: " + Money);
            
            _eventManager.MoneyWereChanged();
        }
        
        public void ChangeValueDiamonds(int amount, Action onComplete = null)
        {
            if (Diamonds + amount >= 0)
            {
                Diamonds += amount;
                playerConfig.Diamonds = Diamonds;

                onComplete?.Invoke();
            }
            else
                Debug.LogWarning("You can't have negative diamonds: " + Diamonds);
            
            _eventManager.DiamondsWereChanged();
        }
    }
}