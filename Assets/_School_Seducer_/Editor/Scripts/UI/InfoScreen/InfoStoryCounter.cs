using System;
using System.Linq;
using _School_Seducer_.Editor.Scripts.Chat;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoStoryCounter : MonoBehaviour, IModule<InfoScreenSystem>, ICharacterSelected
    {
        [SerializeField] private Slider storiesBar;
        [SerializeField] private Slider addAmountNewStoryBar;
        [SerializeField] private TextMeshProUGUI newStoryBarText;
        [SerializeField] private TextMeshProUGUI storyCounterText;
        [SerializeField] private UnityEvent characterSelectedEvent;
        
        private InfoScreenSystem _system;
        private Character _currentCharacter;

        public void InitializeCore(InfoScreenSystem system)
        {
            _system = system;
        }

        public void Initialize()
        {
            _system.Previewer.CharacterSelectedEvent += OnCharacterSelected;
            _system.GiftsModule.OnPresentAction += UpdateStoryCounter;
        }

        public void OnCharacterSelected(Character character)
        {
            _currentCharacter = character;

            storiesBar.value = 0;
            _system.Previewer.SetLockedConversation(_currentCharacter);
            _system.Previewer.StoryResolver.SetLockedConversation(_currentCharacter.Data.LockedConversation);
            _system.Previewer.StoryResolver.UpdateStatusViews();

            _system.Previewer.StoryResolver.SetSlider(storiesBar);
            _system.Previewer.StoryResolver.SetSliderValue(_currentCharacter.Data.experience);

            UpdateStoryCounter();
            
            Invoke(nameof(UpdateAmountToStoryBar), .1f);
            Invoke(nameof(InvokeSelectCharacter), 0.5f);
        }

        public Slider GetMainBar() => storiesBar;

        public void ResetAmountToStoryBar()
        {
            addAmountNewStoryBar.value = 0;
            newStoryBarText.text = "";
        }

        private void UpdateStoryCounter()
        {
            int index = GetLastUnlockedConversationIndex();
            storyCounterText.text = index.ToString();
        }

        private void UpdateAmountToStoryBar()
        {
            int giftsScore = _system.GiftsModule.ScoreCurrentGifts;
            
            if (giftsScore == 0) 
            {
                addAmountNewStoryBar.maxValue = storiesBar.maxValue;
                addAmountNewStoryBar.value = 0;
                newStoryBarText.text = "";
                return;
            }
            
            addAmountNewStoryBar.maxValue = storiesBar.maxValue;
            addAmountNewStoryBar.value = storiesBar.value + giftsScore;
            newStoryBarText.text = "+" + giftsScore;
        }
        
        private void InvokeSelectCharacter() => characterSelectedEvent?.Invoke();

        private int GetLastUnlockedConversationIndex()
        {
            return _currentCharacter.Data.allConversations.FindLastIndex(x => x.isUnlocked) + 1;
        }
        
        private int GetCompletedStoriesCount()
        {
            return _currentCharacter.Data.allConversations.Count(x => x.isCompleted);
        }

        private void OnDestroy()
        {
            _system.Previewer.CharacterSelectedEvent -= OnCharacterSelected;
            _system.GiftsModule.OnPresentAction -= UpdateStoryCounter;
        }
    }
}