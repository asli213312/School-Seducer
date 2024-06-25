using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoCharacterScroller : InfoBaseScroller, IModule<InfoScrollersModule>
    {
        [Inject] private MonoController _monoController;
        
        [SerializeField] private Button nextButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private Image selectorImage;

        private InfoScrollersModule _scrollersModule;
        private Sprite[] _portraits;
        private Character[] _characters;
        private Sprite _currentPortrait;

        public void InitializeCore(InfoScrollersModule system)
        {
            _scrollersModule = system;
        }

        public void Initialize(Character[] characters)
        {
            _characters = characters;
            _portraits = GetPortraits();
            
            nextButton.AddListener(Next);
            previousButton.AddListener(Previous);
        }

        public void InstallCurrentPortrait(Sprite newPortrait)
        {
            selectorImage.sprite = newPortrait;
            _currentPortrait = newPortrait;
        }

        protected override void Next()
        {
            for (int i = 0; i < _characters.Length; i++)
            {
                Character character = _characters[i];
                if (character.Data.info is null || character.Data.info.portrait is null) continue; 
                if (_currentPortrait != character.Data.info.portrait) continue;
                
                if (i + 1 < _characters.Length)
                {
                    if (_characters[i + 1].Data.isLocked) continue;
                    
                    _scrollersModule.SelectCharacter(_characters[i + 1]);
                    _monoController.UpdateMonoByName("Character Name Info");
                    _monoController.UpdateMonoByName("Character Name Chat");
                    
                    Debug.Log("Next character: " + _characters[i + 1].Data.name);
                    break;
                }
                
                Debug.LogWarning("No more next characters to display");
            }
        }

        protected override void Previous()
        {
            for (int i = _characters.Length - 1; i >= 0; i--)
            {
                Character character = _characters[i];
                if (character.Data.info is null || character.Data.info.portrait is null) continue; 
                if (_currentPortrait != character.Data.info.portrait) continue;
                
                if (i - 1 >= 0)
                {
                    if (_characters[i - 1].Data.isLocked) continue;
                    
                    _scrollersModule.SelectCharacter(_characters[i - 1]);
                    _monoController.UpdateMonoByName("Character Name Info");
                    _monoController.UpdateMonoByName("Character Name Chat");
                    
                    Debug.Log("Previous character: " + _characters[i - 1].Data.name);
                    break;
                }
                
                Debug.LogWarning("No more previous characters to display");
            }            
        }

        private Sprite[] GetPortraits()
        {
            List<Sprite> portraits = new();
            
            foreach (var character in _characters)
            {
                if (character.Data.info is null) continue;
                if (character.Data.info.portrait is null) continue;
                
                portraits.Add(character.Data.info.portrait);
            }

            return portraits.ToArray();
        }

        private void OnDestroy()
        {
            nextButton.RemoveListener(Next);
            previousButton.RemoveListener(Previous);
        }
    }
}