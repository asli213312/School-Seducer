using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.UI;
using _School_Seducer_.Editor.Scripts.UI.Popups;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoGiftsModule : MonoBehaviour, IModule<InfoScreenSystem>, ICharacterSelected
    {
        [Inject] private MonoController _monoController;

        [SerializeField] private Push unlockedStoryPush;
        [SerializeField] private Push giftPresentPush;
        [SerializeField] private CharactersConfig config;
        [SerializeField] private Slider barExpCharacter;
        [SerializeField] private GiftView giftPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private Button presentButton;
        [SerializeField] private EventTrigger spinTrigger;
        
        private InfoScreenSystem _system;
        private Character _currentCharacter;
        private int _scoreCurrentGifts;
        
        public void InitializeCore(InfoScreenSystem system)
        {
            _system = system;
        }

        public void Initialize()
        {
            _system.Previewer.CharacterSelectedEvent += OnCharacterSelected;
            presentButton.AddListener(PresentGift);
        }

        public void OnCharacterSelected(Character character)
        {
            ResetContent();

            _currentCharacter = character;

            SetStatusContent(character);
        }

        private void SetStatusContent(Character character)
        {
            if (character.Data.gifts.Count > 0)
            {
                presentButton.gameObject.Activate();
                spinTrigger.gameObject.Deactivate();
                RenderContent(character);
            }
            else
            {
                spinTrigger.gameObject.Activate();
                presentButton.gameObject.Deactivate();
            }
        }

        public void Reset()
        {
            ResetContent();
            ResetConsumedGifts();
        }

        private void PresentGift()
        {
            if (_scoreCurrentGifts == 0) return;
            
            _system.Previewer.SetLockedConversation(_currentCharacter);
            
            _system.Previewer.StoryResolver.SetSlider(barExpCharacter);
            _system.Previewer.StoryResolver.UpdateStatusViews();
            _system.Previewer.StoryResolver.SetRolledConversation(_currentCharacter.Data.LockedConversation);
            _system.Previewer.StoryResolver.UpdateStatusViews();
            
            _currentCharacter.Data.ChangeExperience(_scoreCurrentGifts);
            _system.Previewer.StoryResolver.SetSliderValue(_currentCharacter.Data.experience);

            _system.Previewer.StoryResolver.UpdateStatusViews();
            _system.Previewer.StoryResolver.UpdateStatusViews();
            _monoController.UpdateMonoByName("EarnScoreGifts");
            
            if (_currentCharacter.Data.allConversations[^1].isUnlocked == false)
            {
                if (_system.Previewer.StoryResolver.StoryUnlocked)
                {
                    ShowUnlockStory();

                    if (_currentCharacter.Data.LockedConversation != _currentCharacter.Data.allConversations[^1])
                    {
                        _currentCharacter.Data.LockedConversation.isUnlocked = true;
                        _currentCharacter.Data.LockedConversation.isCompleted = false;
                        _currentCharacter.Data.experience = 0;
                    }

                    if (_currentCharacter.Data.allConversations[^1] == _currentCharacter.Data.LockedConversation)
                    {
                        _currentCharacter.Data.allConversations[^1].isUnlocked = true;
                        _currentCharacter.Data.allConversations[^1].isCompleted = false;
                    }
                        
                }
                else
                {
                    ShowGiftPush();
                }
                
                spinTrigger.gameObject.Activate();
                
                SetStatusContent(_currentCharacter);
            }
        }

        private void ShowGiftPush()
        {
            StartCoroutine(giftPresentPush.transform.DoLocalScaleAndUnscale(this, new Vector3(0.95f, 0.95f, 0.95f)));
        }

        private void ShowUnlockStory()
        {
            StartCoroutine(unlockedStoryPush.transform.DoLocalScaleAndUnscale(this, new Vector3(0.95f, 0.95f, 0.95f)));
        }

        private void RenderContent(Character character)
        {
            for (int i = 0; i < character.Data.gifts.Count; i++)
            {
                var giftData = character.Data.gifts[i];
                
                if (i > config.maxGifts - 1) break;
                
                var giftView = Instantiate(giftPrefab, content);
                giftView.Render(giftData);

                _scoreCurrentGifts += giftData.score;
            }

            _monoController.UpdateMonoByName("EarnScoreGifts");
            Debug.Log("Rendered all score gifts: " + _scoreCurrentGifts);
        }

        private void ResetContent()
        {
            if (content.childCount > 0)
            {
                for (int i = 0; i < content.childCount; i++)
                {
                    Destroy(content.GetChild(i).gameObject);
                }
            }
        }

        private void ResetConsumedGifts()
        {
            if (_currentCharacter != null && _currentCharacter.Data.gifts.Count > 0)
            {
                _currentCharacter.Data.gifts.Clear();
                _scoreCurrentGifts = 0;   
            }
        }

        private void OnDestroy()
        {
            _system.Previewer.CharacterSelectedEvent -= OnCharacterSelected;
            presentButton.RemoveListener(PresentGift);
        }
    }
}