using System;
using System.Linq;
using _School_Seducer_.Editor.Scripts.Chat;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoStoryCounter : MonoBehaviour, IModule<InfoScreenSystem>, ICharacterSelected
    {
        [SerializeField] private Slider storiesBar;
        
        private InfoScreenSystem _system;
        private Character _currentCharacter;
        
        public void InitializeCore(InfoScreenSystem system)
        {
            _system = system;
        }

        public void Initialize()
        {
            _system.Previewer.CharacterSelectedEvent += OnCharacterSelected;
        }

        public void OnCharacterSelected(Character character)
        {
            _currentCharacter = character;

            storiesBar.maxValue = character.Data.allConversations.Count;
            storiesBar.value = 0;
            storiesBar.value = GetCompletedStoriesCount();
        }
        
        private int GetCompletedStoriesCount()
        {
            return _currentCharacter.Data.allConversations.Count(x => x.isCompleted);
        }

        private void OnDestroy()
        {
            _system.Previewer.CharacterSelectedEvent -= OnCharacterSelected;
        }
    }
}