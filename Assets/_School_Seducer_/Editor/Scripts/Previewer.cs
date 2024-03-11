using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Utility;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Nutaku;
using Nutaku.Unity;
using UnityEngine.Events;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class Previewer : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;
        [Inject] private Bank _bank;
        [Inject] private IChatInitialization _chatInitializationModule;
        
        [Header("UI Data")] 
        [SerializeField] private GameObject previewerPanel;
        [SerializeField] private ChatSystem chatSystem;
        [SerializeField] private Image selectedGirlImage;
        [SerializeField] private TextMeshProUGUI greetingsText;

        [Header("Data")] 
        [SerializeField] private CharactersConfig charactersConfig;
	    [SerializeField] private Chat.Chat _chat;
        [SerializeField] private StoryResolver storyResolver;
        [SerializeField] private LevelChecker levelChecker;
        [SerializeField] private Map map;
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private Character[] _characters;

        [Header("Events")] 
        [SerializeField] private UnityEvent characterSelected;

        [Header("Options")] 
        [SerializeField] private bool showDebugParameters;
        
        public event Action<Character> CharacterSelectedEvent;

        public CharactersConfig CharactersConfig => charactersConfig;
        public Character[] Characters => _characters;
        public StoryResolver StoryResolver => storyResolver;
        public bool NeedPush { get; set; }
        public Character CurrentCharacter { get; set; }

        private СonversationData _lockedConversation;

        private СonversationData _currentConversation;
        private SwitcherInDialogue _switcher;

        private int _currentChatIndex;

        private void Awake()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android: SdkPlugin.Initialize(); break;
                case RuntimePlatform.WindowsPlayer:  break;
                case RuntimePlatform.WebGLPlayer: break;
            }

            RegisterCharacters();
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            storyResolver.Initialize();
        }

        public CharacterData GetCurrentCharacterData()
        {
            return CurrentCharacter.Data;
        }

        private void OnDestroy()
        {
            UnRegisterStartDialogue();
            UnRegisterCharacters();
            ResetCharacter();
        }

        public void SelectCurrentCharacter()
        {
            if (CurrentCharacter is null) return;
            
            CharacterSelectedEvent?.Invoke(CurrentCharacter);
            characterSelected?.Invoke();
            
            Debug.Log("Selected character: " + CurrentCharacter.name);
        }

        public void SelectCharacter(Character character)
        {
            CurrentCharacter = character;
            CharacterSelectedEvent?.Invoke(character);
            characterSelected?.Invoke();
            Debug.Log("Selected character: " + character.name);
        }

        public void OnCharacterSelected(Character character)
        {
            if (CurrentCharacter != null)
            {
                Debug.LogWarning("Current character: " + CurrentCharacter.name);
            }
            
            if (chatSystem != null) chatSystem.gameObject.Activate();
            previewerPanel.Activate();
            
            if (CurrentCharacter != character) _chat.ResetContent();

            CurrentCharacter = character;
            
            characterSelected?.Invoke();
            _eventManager.UpdateScrollChat();

            _currentConversation = CurrentCharacter.CurrentConversation;
            storyResolver.InitCharacterData(CurrentCharacter.Data);

            if (CurrentCharacter == null) 
	        	Debug.LogError("current character is null on selected");

            if (chatSystem != null) _chatInitializationModule.InstallCharacter(CurrentCharacter);

            CharacterSelectedEvent?.Invoke(character);

            map.CloseMap();
        }

        private void UnRegisterStartDialogue()
        {
            if (CurrentCharacter == null)
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
                _eventManager.ConversationEnded();
                Debug.Log("Money doesn't enough to continue...");
            }
        }       

        public void AddDiamondOnConversationEnd()
        {
            _bank.ChangeValueDiamonds(1);
        }

        public void AddLoyalty(int n)
        {
            CurrentCharacter.Data.ChangeLoyalty(n);
        }

        private void ResetCharacter()
        {
            CurrentCharacter = null;
            _currentConversation = null;
        }

        public void RegisterCharacter(Character character)
        {
            character.CharacterSelected += OnCharacterSelected;
        }

        public void UnregisterCharacter(Character character)
        {
            character.CharacterSelected -= OnCharacterSelected;
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

        public void SetLockedConversation(Character character)
        {
            CharacterData data = character.Data;
            List<СonversationData> conversations = data.allConversations;

            foreach (var conversation in conversations)
            { 
                //if (conversation.IsUnlocked(playerConfig.Experience)) _lockedConversation = null;
                
                if (conversation.isUnlocked == false)
                {
                    data.LockedConversation = conversation;
                    _lockedConversation = conversation;
                    
                    _eventManager.SelectCharacter(character);
                    break;
                }
            }

            //Debug.Log("<color=red>Locked conversation is null!</color>");
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