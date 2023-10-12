using System;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class Bank : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;
        [SerializeField] private PlayerConfig playerConfig;
        
        private const int INITIAL_MONEY = 500;
        private int _money;

        public int Money
        {
            get { return _money; }
            set
            {
                playerConfig.Money = value;
                _money = playerConfig.Money;
                _eventManager.UpdateTextMoney();
            }
        }

        private void Awake()
        {
            Money = playerConfig.Money;
            Money = INITIAL_MONEY;
            _eventManager.ChangeValueMoneyEvent += ChangeValueMoney;
        }

        private void OnDestroy()
        {
            _eventManager.ChangeValueMoneyEvent -= ChangeValueMoney;
        }

        public void ChangeValueMoney(int value)
        {
            if (Money + value >= 0)
                Money += value;
            else
                Debug.LogWarning("Not enough money: " + Money);
        }
    }
}