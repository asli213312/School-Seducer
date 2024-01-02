using System;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class Bank : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;
        [SerializeField] private PlayerConfig playerConfig;
        
        public int _money;
        public int _diamonds;

        public int Money
        {
            get => _money;
            set
            {
                playerConfig.Money = value;
                _money = playerConfig.Money;
                _eventManager.UpdateTextMoney();
            }
        }
        
        public int Diamonds
        {
            get { return _diamonds; }
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
            _eventManager.ChangeValueMoneyEvent += ChangeValueMoney;
            _eventManager.ChangeValueDiamondsEvent += ChangeValueDiamonds;
        }

        private void OnDestroy()
        {
            _eventManager.ChangeValueDiamondsEvent -= ChangeValueDiamonds;
            _eventManager.ChangeValueMoneyEvent -= ChangeValueMoney;
        }

        public void ChangeValueMoney(int value)
        {
            if (Money + value >= 0)
            {
                Money += value;
                playerConfig.Money = Money;
            }
            else
                Debug.LogWarning("Not enough money: " + Money);
            
            _eventManager.UpdateTextMoney();
        }
        
        public void ChangeValueDiamonds(int amount)
        {
            if (Diamonds + amount >= 0)
            {
                Diamonds += amount;
                playerConfig.Diamonds = Diamonds;
            }
            else
                Debug.LogWarning("You can't have negative diamonds" + _diamonds);
            
            _eventManager.UpdateTextDiamonds();
        } 
    }
}