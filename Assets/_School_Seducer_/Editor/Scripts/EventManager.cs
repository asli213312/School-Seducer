using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts
{
    public class EventManager : MonoBehaviour
    {
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] public UnityEvent startGame; 
        public PlayerConfig PlayerConfig => playerConfig;
        private bool IsMessageReceived { get; set; }

        public event Action<Location> LocationSelectedEvent;
        public event Action<Character> CharacterSelectedEvent;
        public event Action<int> ChangeValueMoneyEvent;
        public event Action<int> ChangeValueDiamondsEvent;
        public event Action<int> ChangeValueExperienceEvent;
        public event Action ChatConversationEnded;
        public event Action ChatMessagesIsEnded;
        public event Action ChatMessagesIsStarted;
        public event Action UpdateScrollEvent;
        public event Action WheelFortuneCharacterRolled;
        public event Action UpdateExperienceTextEvent;
        public event Action UpdateTextMoneyEvent;
        public event Action UpdateTextDiamondsEvent;

        public void MoneyWereChanged()
        {
            ChangeValueMoneyEvent?.Invoke(playerConfig.Money);
            UpdateTextMoneyEvent?.Invoke();
        }
        
        public void DiamondsWereChanged()
        {
            ChangeValueDiamondsEvent?.Invoke(playerConfig.Diamonds);
            UpdateTextDiamondsEvent?.Invoke();
        }
        
        public void ExperienceWasChanged()
        {
            ChangeValueExperienceEvent?.Invoke(playerConfig.Experience);
            UpdateExperienceTextEvent?.Invoke();
        }

        public void InvokeStartEvent()
        {
            startGame.Invoke();
        }

        public bool IsChatMessageReceived()
        {
            return IsMessageReceived;
        }

        public void ChatMessageReceived(bool received)
        { 
            IsMessageReceived = received;
        }

        public void UpdateScrollChat()
        {
            UpdateScrollEvent?.Invoke();
        }

        public void ConversationEnded()
        {
            ChatConversationEnded?.Invoke();
        }

        public void ChatMessagesStarted()
        {
            ChatMessagesIsStarted?.Invoke();
        }

        public void ChatMessagesEnded()
        {
            ChatMessagesIsEnded?.Invoke();
        }

        public void UpdateTextDiamonds()
        {
            UpdateTextDiamondsEvent?.Invoke();
        }

        public void UpdateTextMoney()
        {
            UpdateTextMoneyEvent?.Invoke();
        }
        
        public void UpdateTextExperience()
        {
            UpdateExperienceTextEvent?.Invoke();
        }

        public void UpdateRolledCharacter() => WheelFortuneCharacterRolled?.Invoke();
        
        public void SelectLocation(Location location)
        {
            LocationSelectedEvent?.Invoke(location);
        }

        public void SelectCharacter(Character character)
        {
            CharacterSelectedEvent?.Invoke(character);
        }
        
        public void InvokeDelayedBoolAction(float delay, Action<bool> onComplete, bool boolParameter)
        {
            StartCoroutine(DelayToBoolAction(delay, onComplete, boolParameter));
        }

        public void InvokeDelayedVoidAction(float delay, Action onComplete)
        {
            StartCoroutine(DelayToVoidAction(delay, onComplete));
        }
        
        private IEnumerator DelayToBoolAction(float delay, Action<bool> onComplete, bool boolParameter)
        {
            yield return new WaitForSeconds(delay);
            onComplete?.Invoke(boolParameter);
        }

        private IEnumerator DelayToVoidAction(float delay, Action onComplete)
        {
            yield return new WaitForSeconds(delay);
            onComplete?.Invoke();
        }
    }
}