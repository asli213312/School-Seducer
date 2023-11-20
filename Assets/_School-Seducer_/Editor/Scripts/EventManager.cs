using System;
using System.Reflection.Emit;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    public class EventManager : MonoBehaviour
    {
        [SerializeField] private PlayerConfig playerConfig;
        public PlayerConfig PlayerConfig => playerConfig;

        public event Action<Location> LocationSelectedEvent;
        public event Action<Character> CharacterSelectedEvent;
        public event Action<int> ChangeValueMoneyEvent;
        public event Action<int> ChangeValueDiamondsEvent;

        public event Action UpdateTextMoneyEvent;
        public event Action UpdateTextDiamondsEvent;

        public void ChangeValueMoney(int value)
        {
            ChangeValueMoneyEvent?.Invoke(value);
            UpdateTextMoneyEvent?.Invoke();
        }
        
        public void ChangeValueDiamonds(int value)
        {
            ChangeValueDiamondsEvent?.Invoke(value);
            UpdateTextDiamondsEvent?.Invoke();
        }

        public void UpdateTextDiamonds()
        {
            UpdateTextDiamondsEvent?.Invoke();
        }

        public void UpdateTextMoney()
        {
            UpdateTextMoneyEvent?.Invoke();
        }
        
        public void SelectLocation(Location location)
        {
            LocationSelectedEvent?.Invoke(location);
        }

        public void SelectCharacter(Character character)
        {
            CharacterSelectedEvent?.Invoke(character);
        }
    }
}