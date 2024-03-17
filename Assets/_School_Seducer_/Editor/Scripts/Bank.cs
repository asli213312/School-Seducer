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

        public int experience;
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
        
        public int Experience
        {
            get => experience;
            set
            {
                playerConfig.Experience = value;
                experience = playerConfig.Experience;
                _eventManager.UpdateTextExperience();
            }
        }

        private void Awake()
        {
            Experience = playerConfig.Experience;
            Money = playerConfig.Money;
            Diamonds = playerConfig.Diamonds;
        }

        public void ChangeValueGold(int value)
        {
            if (Money + value >= 0)
            {
                Money += value;
                playerConfig.Money = Money;
            }
            else
                Debug.LogWarning("Not enough money: " + Money);
            
            _eventManager.MoneyWereChanged();
        }
        
        public void ChangeValueDiamonds(int amount)
        {
            if (Diamonds + amount >= 0)
            {
                Diamonds += amount;
                playerConfig.Diamonds = Diamonds;
            }
            else
                Debug.LogWarning("You can't have negative diamonds: " + Diamonds);
            
            _eventManager.DiamondsWereChanged();
        }
        
        public void ChangeValueExperience(int amount)
        {
            if (Experience + amount >= 0)
            {
                Experience += amount;
                playerConfig.Experience = Experience;
            }
            else
                Debug.LogWarning("You can't have negative experience: " + Experience);
            
            _eventManager.ExperienceWasChanged();
        }
    }
}