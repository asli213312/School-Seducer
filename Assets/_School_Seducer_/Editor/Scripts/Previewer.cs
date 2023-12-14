using System;
using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using DialogueEditor;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UltEvents;
using UnityEngine.Events;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class Previewer : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;
        [Inject] private Bank _bank;
        
        [Header("UI Data")] 
        [SerializeField] private GameObject previewerPanel;
        [SerializeField] private Image selectedGirlImage;
        [SerializeField] private Image _emote;
        [SerializeField] private TextMeshProUGUI greetingsText;

        [Header("Data")] 
	    [SerializeField] private Chat.Chat _chat;
        [SerializeField] private LevelChecker levelChecker;
        [SerializeField] private MiniGameInitializer miniGameInitializer;
        [SerializeField] private Map map;
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private Character[] _characters;

        [Header("Options")] 
        [SerializeField] private bool showDebugParameters;

        private Character _currentCharacter;
        private СonversationData _currentConversation;
        private SwitcherInDialogue _switcher;

        private int _currentChatIndex;

        public void Initialize(Chat.Chat chat)
        {
            foreach (var character in _characters)
            {
                character.Initialize(chat);
            }
        }

        private void Awake()
        {
            RegisterCharacters();
        }

        private void OnDestroy()
        {
            UnRegisterStartDialogue();
            UnRegisterCharacters();
            ResetCharacter();
        }

        private void OnCharacterSelected(Character character)
        {
            previewerPanel.Activate();

            _currentCharacter = character;
	        _currentConversation = _currentCharacter.Conversation;

            if (_currentCharacter == null) 
	        	Debug.LogError("current character is null on selected");
            
            Invoke(nameof(RegisterStartDialogue), 0.3f);
            if (!CheckLevelPlayer())
            {
                Debug.Log("Check for level is invoked");
                //startDialogueButton.AddListener(SkipDialog);
            }

            //_switcher.Initialize(_emote, selectedGirlImage);
            //Invoke(nameof(SetCurrentNodeID), 3f);

	        map.CloseMap();
	        _chat.InstallCurrentConversation(_currentConversation);

            //UpdateUI();
        }

        public void SkipDialog()
        {
            
        }

        private bool CheckLevelPlayer()
        {
            return playerConfig.Level >= _currentCharacter.Data.RequiredLevel;
        }

        private void RegisterStartDialogue()
        {
            if (_currentCharacter == null)
            {
                Debug.LogWarning("Character is null, cannot register options");
                //return;
            }
            
            _chat.StartDialogue(_currentCharacter.StartConversation);
        }

        private void UnRegisterStartDialogue()
        {
            if (_currentCharacter == null)
            {
                Debug.LogWarning("Character is null, cannot unregister options");
                return;
            }
        }

        public void ReduceMoneyPlayer()
        {
            if (_bank.Money >= _chat.Config.CoinsForMessage)
            {
                _bank.ChangeValueMoney(-_chat.Config.CoinsForMessage);
            }  
            else
            {
                _currentCharacter.EndConversation();
                Debug.Log("Money doesn't enough to continue...");
            }
        }       

        public void AddDiamondOnConversationEnd()
        {
            _bank.ChangeValueDiamonds(1);
        }

        private void UpdateUI()
        {
            greetingsText.text = "Hello, " + _currentCharacter.name;
            selectedGirlImage.sprite = _currentCharacter.SpriteRenderer.sprite;
        }

        public void AddLoyalty(int n)
        {
            _currentCharacter.Data.ChangeLoyalty(n);
        }

        private void ResetCharacter()
        {
            _currentCharacter = null;
            _currentConversation = null;
        }

        private void RegisterCharacters()
        {
            foreach (var character in _characters)
            {
                character.CharacterSelected += OnCharacterSelected;
                character.CharacterEnter += levelChecker.Enter;
                character.CharacterExit += levelChecker.Exit;
            }
        }

        private void UnRegisterCharacters()
        {
            foreach (var character in _characters)
            {
                character.CharacterSelected -= OnCharacterSelected;
                character.CharacterEnter -= levelChecker.Enter;
                character.CharacterExit -= levelChecker.Exit;
            }
        }
    }
}