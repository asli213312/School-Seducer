using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoScrollersModule : MonoBehaviour, IInfoScrollersModule
    {
        [SerializeField] private InfoCharacterScroller characterScroller;

        public ReactiveProperty<Character> CurrentCharacter { get; private set; } = new ReactiveProperty<Character>();

        public Previewer Previewer => _system.Previewer;
        
        private InfoScreenSystem _system;

        public void InitializeCore(InfoScreenSystem system)
        {
            _system = system;
        }

        public void Initialize()
        {
            _system.Previewer.CharacterSelectedEvent += OnCharacterSelected;
            
            characterScroller.InitializeCore(this);
            characterScroller.Initialize(_system.Previewer.Characters);
        }

        public void OnCharacterSelected(Character character)
        {
            characterScroller.InstallCurrentPortrait(character.Data.info.portrait);
            CurrentCharacter.Value = character;
        }

        public void SelectCharacter(Character character) 
        {
            _system.Previewer.SelectCharacter(character);
        }

        private void OnDestroy()
        {
            _system.Previewer.CharacterSelectedEvent -= OnCharacterSelected;
        }
    }
}