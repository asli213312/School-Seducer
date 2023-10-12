using System;
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

        public event Action UpdateTextMoneyEvent;

        public void ChangeValueMoney(int value)
        {
            ChangeValueMoneyEvent?.Invoke(value);
            UpdateTextMoney();
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